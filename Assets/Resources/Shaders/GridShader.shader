Shader "Grid"
{
    Properties 
    {
        _PrimaryColor ("Color of main grid lines", Color) = (1,1,1,1)
        _SecondaryColor ("Color of small grid lines", Color) = (1,1,1,1)
        _PrimaryGridScale ("Primary grid scale", Float) = 1.0
        _SecondaryGridScale ("Secondary grid scale", Float) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        CGINCLUDE
        #include "UnityCG.cginc"

        struct vertex
        {
            float4 pos : POSITION;
            float2 uv  : TEXCOORD0;
        };

        struct v2f
        {
            float4 pos       : SV_POSITION;
            float2 uv        : TEXCOORD0;
            float3 world_pos : TEXCOORD1;
        };

        v2f vert(vertex v, uint instanceID: SV_InstanceID)
        {
            v2f f = (v2f)0;

            f.pos = UnityObjectToClipPos(v.pos);
            f.world_pos = mul(unity_ObjectToWorld, v.pos);
            f.uv = v.uv;

            return f;
        }

        float4 _PrimaryColor;
        float4 _SecondaryColor;
        float _PrimaryGridScale;
        float _SecondaryGridScale;

        // Based on: http://asliceofrendering.com/scene%20helper/2020/01/05/InfiniteGrid/
        float4 grid(float3 world_pos, float scale, float3 color)
        {
            float2 coord = world_pos.xy;
            coord *= scale;

            float2 derivative = fwidth(coord);
            float2 grid = abs(frac(coord - 0.5) - 0.5) / derivative;
            float l = min(grid.x, grid.y);

            float4 _color = float4(color, 1.0 - min(l, 1.0));

            return _color;
        }

        float4 frag(v2f i) : SV_Target
        {
            float4 small_grid = grid(i.world_pos, 1.0 / _SecondaryGridScale, _SecondaryColor);
            float4 large_grid = grid(i.world_pos, 1.0 / _PrimaryGridScale, _PrimaryColor);

            float4 grid = 0;
            if (small_grid.a > 0 && large_grid.a > 0)
            {
                grid = large_grid;
            }
            else
            {
                grid = small_grid * smoothstep(0, 1, 1 - unity_OrthoParams.x / 30);
            }

            grid.a *= saturate((0.5 - distance(i.uv, float2(0.5, 0.5))) * 2);

            return grid;
        }
        ENDCG

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}