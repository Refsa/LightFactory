Shader "Unlit/Background"
{
    Properties { }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 world_pos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.world_pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            void Unity_Posterize_float3(float3 In, float3 Steps, out float3 Out)
            {
                Out = floor(In / (1 / Steps)) * (1 / Steps);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 wp = i.world_pos.xy;

                float3 noise = snoise(wp * 0.5) * vfbm(wp * 10) * iqnoise(wp * 2, 1, 1);
                noise *= float3(fbm(wp + _Time.y * 0.1), fbm(wp + _Time.y * 0.01), fbm(wp + _Time.y * 0.001));

                // Unity_Posterize_float3(noise, 16, noise);
                noise = smoothstep(0, 1, noise);

                return float4(noise, 1.0);
            }
            ENDCG
        }
    }
}
