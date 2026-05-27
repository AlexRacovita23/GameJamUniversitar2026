using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParticleSystem))]
public class RockCollapseParticleSystem : MonoBehaviour
{
    public enum RockRenderMode { Mesh3D, FlatBillboard, Impostor }

    [Header("References")]
    public Material rockMaterial;
    public Transform playerTransform;
    public RockMeshLibrary meshLibrary;
    public RockImpostorBaker impostorBaker;

    [Header("Spawn Position")]
    public float heightAbovePlayer = 1f;
    public float distanceInFrontOfPlayer = 1f;

    [Header("Collapse Settings")]
    [Range(1, 50)] public int chunksPerSecond = 10;
    public Vector2 rockSizeRange = new Vector2(0.3f, 1.2f);
    [Range(0.5f, 5f)] public float gravityMultiplier = 2f;
    [Range(1f, 10f)] public float rockLifetime = 4f;
    [Range(0.1f, 5f)] public float collapseWidth = 1.5f;
    [Range(0f, 3f)] public float upwardVariance = 0.5f;

    [Header("Rotation")]
    public bool enableRotation = true;
    [Range(0f, 180f)] public float maxRotationSpeed = 90f;

    [Header("Render Mode")]
    public RockRenderMode renderMode = RockRenderMode.Mesh3D;

    private ParticleSystem _ps;
    private ParticleSystemRenderer _psr;
    private PlayerInputActions _input;
    private bool _isActive;
    private RockRenderMode _appliedMode = (RockRenderMode)(-1);

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _psr = GetComponent<ParticleSystemRenderer>();
        _input = new PlayerInputActions();
    }

    private void Start()
    {
        if (meshLibrary == null)
            meshLibrary = FindFirstObjectByType<RockMeshLibrary>();

        if (meshLibrary == null)
            meshLibrary = new GameObject("RockMeshLibrary_Auto").AddComponent<RockMeshLibrary>();

        if (impostorBaker == null)
            impostorBaker = FindFirstObjectByType<RockImpostorBaker>();

        if (impostorBaker != null)
            impostorBaker.OnBakeComplete += OnBakeReady;

        ApplyParticleSettings();

        bool impostorReady = renderMode != RockRenderMode.Impostor
                          || (impostorBaker != null && impostorBaker.IsBaked);
        if (impostorReady)
            ApplyRenderMode();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.ToggleSandstorm.performed += OnToggle;
        _input.Player.ChangeRenderMode.performed += OnChangeRenderMode;
    }

    private void OnDisable()
    {
        _input.Player.ToggleSandstorm.performed -= OnToggle;
        _input.Player.ChangeRenderMode.performed -= OnChangeRenderMode;
    }

    private void OnDestroy()
    {
        if (impostorBaker != null)
            impostorBaker.OnBakeComplete -= OnBakeReady;
    }

    private void Update()
    {
        if (renderMode == _appliedMode) return;

        if (renderMode == RockRenderMode.Impostor &&
            (impostorBaker == null || !impostorBaker.IsBaked))
            return;

        ApplyRenderMode();
    }

    private void OnToggle(InputAction.CallbackContext ctx)
    {
        if (_isActive) StopCollapse();
        else StartCollapse();
    }
    
    private void OnChangeRenderMode(InputAction.CallbackContext ctx) {
        switch (renderMode) {
            case RockRenderMode.Mesh3D:
                renderMode = RockRenderMode.FlatBillboard;
                break;
            case RockRenderMode.FlatBillboard:
                renderMode = RockRenderMode.Impostor;
                break;
            case RockRenderMode.Impostor:
                renderMode = RockRenderMode.Mesh3D;
                break;
        }
        
        ApplyRenderMode();
    }

    private void StartCollapse()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("[RockCollapse] playerTransform not assigned.");
            return;
        }

        Vector3 fwd = playerTransform.forward;
        fwd.y = 0f;
        fwd.Normalize();

        transform.position = playerTransform.position
                           + Vector3.up * heightAbovePlayer
                           + fwd * distanceInFrontOfPlayer;

        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ApplyParticleSettings();
        ApplyRenderMode();
        _ps.Play();
        _isActive = true;
    }

    private void StopCollapse()
    {
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _isActive = false;
    }

    private void OnBakeReady() => ApplyRenderMode();

    public void SetRenderMode(RockRenderMode mode)
    {
        renderMode = mode;
        ApplyRenderMode();
    }

    private void ApplyRenderMode()
    {
        if (_psr == null) return;

        switch (renderMode)
        {
            case RockRenderMode.Mesh3D:
                if (meshLibrary == null || meshLibrary.Meshes == null || meshLibrary.Meshes.Length == 0)
                {
                    Debug.LogWarning("[RockCollapse] Mesh3D: mesh library not ready.");
                    return;
                }
                _psr.renderMode = ParticleSystemRenderMode.Mesh;
                _psr.SetMeshes(meshLibrary.Meshes);
                _psr.material = rockMaterial;
                SetVertexStreamsDefault();
                break;

            case RockRenderMode.FlatBillboard:
                _psr.renderMode = ParticleSystemRenderMode.Billboard;
                _psr.material = rockMaterial;
                SetVertexStreamsDefault();
                break;

            case RockRenderMode.Impostor:
                if (impostorBaker == null)
                {
                    Debug.LogWarning("[RockCollapse] Impostor: no RockImpostorBaker in scene.");
                    return;
                }
                if (!impostorBaker.IsBaked || impostorBaker.ImpostorMaterial == null)
                {
                    Debug.LogWarning("[RockCollapse] Impostor: bake not complete yet.");
                    return;
                }
                _psr.renderMode = ParticleSystemRenderMode.Billboard;
                _psr.material = impostorBaker.ImpostorMaterial;
                SetVertexStreamsImpostor();
                break;
        }

        _appliedMode = renderMode;

        if (_isActive)
        {
            ApplyParticleSettings();
        }
    }

    private void SetVertexStreamsImpostor()
    {
        _psr.SetActiveVertexStreams(new System.Collections.Generic.List<ParticleSystemVertexStream>
        {
            ParticleSystemVertexStream.Position,
            ParticleSystemVertexStream.UV,
            ParticleSystemVertexStream.Color,
            ParticleSystemVertexStream.Center,
            ParticleSystemVertexStream.Rotation3D,
        });
    }

    private void SetVertexStreamsDefault()
    {
        _psr.SetActiveVertexStreams(new System.Collections.Generic.List<ParticleSystemVertexStream>
        {
            ParticleSystemVertexStream.Position,
            ParticleSystemVertexStream.UV,
            ParticleSystemVertexStream.Color,
        });
    }

    private void ApplyParticleSettings()
    {
        var main = _ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.maxParticles = 500;
        main.startLifetime = new ParticleSystem.MinMaxCurve(rockLifetime * 0.8f, rockLifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(rockSizeRange.x, rockSizeRange.y);
        main.gravityModifier = gravityMultiplier;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        float rad = enableRotation ? maxRotationSpeed * Mathf.Deg2Rad : 0f;

        bool impostorMode = renderMode == RockRenderMode.Impostor;
        bool mesh3DMode = renderMode == RockRenderMode.Mesh3D;

        if (mesh3DMode)
        {
            main.startRotation3D = true;
            main.startRotationX = new ParticleSystem.MinMaxCurve(-rad, rad);
            main.startRotationY = new ParticleSystem.MinMaxCurve(-rad, rad);
            main.startRotationZ = new ParticleSystem.MinMaxCurve(-rad, rad);
        }
        else
        {
            main.startRotation3D = false;
            main.startRotation = new ParticleSystem.MinMaxCurve(-rad, rad);
        }

        var rot = _ps.rotationOverLifetime;
        if (!enableRotation)
        {
            rot.enabled = false;
        }
        else if (mesh3DMode)
        {
            rot.enabled = true;
            rot.separateAxes = true;
            rot.x = new ParticleSystem.MinMaxCurve(-rad, rad);
            rot.y = new ParticleSystem.MinMaxCurve(-rad, rad);
            rot.z = new ParticleSystem.MinMaxCurve(-rad, rad);
        }
        else
        {
            rot.enabled = true;
            rot.separateAxes = false;
            rot.z = new ParticleSystem.MinMaxCurve(-rad, rad);
        }

        var emission = _ps.emission;
        emission.enabled = true;
        emission.rateOverTime = chunksPerSecond;

        var shape = _ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(collapseWidth, 0.1f, collapseWidth);

        var vel = _ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.World;
        vel.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        vel.y = new ParticleSystem.MinMaxCurve(-upwardVariance, upwardVariance);
        vel.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);

        var col = _ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.7f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        var fwd = playerTransform.forward; fwd.y = 0f; fwd.Normalize();
        var pos = playerTransform.position
                + Vector3.up * heightAbovePlayer
                + fwd * distanceInFrontOfPlayer;
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.4f);
        Gizmos.DrawWireCube(pos, new Vector3(collapseWidth, 4f, collapseWidth));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerTransform.position, pos);
    }
}