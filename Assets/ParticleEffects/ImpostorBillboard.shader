Shader "ParticleEffects/ImpostorBillboard"
{
    Properties
    {
        _MainTex       ("Impostor Atlas",   2D)         = "white" {}
        _FramesPerAxis ("Frames Per Axis",  Float)      = 4
        _Cutoff        ("Alpha Cutoff",     Range(0,1)) = 0.1
        _AmbientTint   ("Ambient Tint",     Color)      = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float     _FramesPerAxis;
            float     _Cutoff;
            fixed4    _AmbientTint;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 center   : TEXCOORD1;  // xyz = particle world-space centre
                float4 rotation : TEXCOORD2;  // xyz = euler angles in radians (custom stream)
            };

            struct v2f
            {
                float4 pos     : SV_POSITION;
                float2 atlasUV : TEXCOORD0;
                float4 color   : COLOR;
                UNITY_FOG_COORDS(1)
            };

            float3x3 EulerToMatrix(float3 e)
            {
                float cx = cos(e.x), sx = sin(e.x);
                float cy = cos(e.y), sy = sin(e.y);
                float cz = cos(e.z), sz = sin(e.z);

                float3x3 Rx = float3x3(
                    1,   0,   0,
                    0,  cx, -sx,
                    0,  sx,  cx
                );
                float3x3 Ry = float3x3(
                     cy, 0, sy,
                      0, 1,  0,
                    -sy, 0, cy
                );
                float3x3 Rz = float3x3(
                    cz, -sz, 0,
                    sz,  cz, 0,
                     0,   0, 1
                );

                return mul(Rx, mul(Ry, Rz));
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.color = v.color;

                float3 worldCenter = v.center.xyz;

                float3 viewDir = normalize(worldCenter - _WorldSpaceCameraPos);

                float3x3 rot    = EulerToMatrix(v.rotation.xyz);
                float3x3 invRot = transpose(rot);
                float3   localViewDir = mul(invRot, viewDir);

                float theta = atan2(localViewDir.z, localViewDir.x);
                float phi   = acos(clamp(-localViewDir.y, -1.0, 1.0));

                float fu = frac(theta / (2.0 * UNITY_PI) + 0.5);
                float fv = clamp(phi / (UNITY_PI * 0.5), 0.0, 1.0);

                float frameSize = 1.0 / _FramesPerAxis;
                float frameCol  = clamp(floor(fu * _FramesPerAxis), 0.0, _FramesPerAxis - 1.0);
                float frameRow  = clamp(floor(fv * _FramesPerAxis), 0.0, _FramesPerAxis - 1.0);

                float2 tileOrigin = float2(frameCol * frameSize, frameRow * frameSize);
                o.atlasUV = tileOrigin + v.uv * frameSize;

                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.atlasUV) * i.color * _AmbientTint;
                clip(col.a - _Cutoff);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }

    FallBack "Transparent/Cutout/Diffuse"
}
