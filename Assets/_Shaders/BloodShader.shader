Shader "Unlit/BloodShader"{

	Properties{
		_BloodPatternTex("Blood Pattern Texture", 2D) = "white" {}
	}

		SubShader{

			Tags { "RenderType" = "Transparent" }

			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _BloodPatternTex;

				struct MeshData {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators {
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};


				Interpolators vert(MeshData v) {
					Interpolators o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				float4 frag(Interpolators i) : SV_Target{

					float4 finalCol = tex2D(_BloodPatternTex, i.uv);

					float bloodMask = i.uv < finalCol;

					clip(bloodMask - .1f);

					return finalCol;
				}
				ENDCG
			}
		}
}