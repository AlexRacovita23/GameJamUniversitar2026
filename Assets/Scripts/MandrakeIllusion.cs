using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MandrakeIllusion : MonoBehaviour
{
    public float timeToDisappear = 5f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float walkRadius = 10f;
    [SerializeField] private float arrivalThreshold = 0.5f;

    private NavMeshAgent agent;
    private bool isWalking;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        StartCoroutine(MoveNPC());
    }

    private void Update()
    {
        timeToDisappear -= Time.deltaTime;
        if (timeToDisappear <= 0f)
        {
            IllusionsController.Instance?.NotifyNPCDestroyed();
            Destroy(gameObject);
        }
    }

    private IEnumerator MoveNPC()
    {
        while (true)
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
            yield return new WaitForSeconds(waitTime);

            Vector2 randomDirection = Random.insideUnitCircle * walkRadius;
            Vector3 targetPosition = new Vector3(randomDirection.x, 0, randomDirection.y) + transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(targetPosition, out hit, walkRadius, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            isWalking = true;
            animator.SetBool("isWalking", isWalking);

            while ((agent.remainingDistance > arrivalThreshold || agent.pathPending) && timeToDisappear > 0.2f)
            {
                yield return null;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            SanitySystem.Instance?.DecreaseSanity(5f);
            IllusionsController.Instance?.NotifyNPCDestroyed();
            AudioManager.Instance?.PlayFinalCue();
            Destroy(gameObject);
        }
    }   
}
