Shader "Unlit/HighlightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorStart("Color Start", Color) = (0,0,0,0)
        _ColorEnd ("Color End" , Color) = (0,0,0,0)
        _T("T", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorStart;
            float4 _ColorEnd;
            float _T;

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };



            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float InverseLerp(float a, float b, float v) {

                return (v - a) / (b - a);
            }


            fixed4 frag(Interpolators i) : SV_Target
            {
                float4 col = lerp(_ColorStart, _ColorEnd, _T);
                
                return col;
            }
            ENDCG
        }
    }
}
