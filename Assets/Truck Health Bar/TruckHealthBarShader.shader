Shader "Unlit/HealthbarBorderShader"{

	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Health("Health", Range(0,1)) = 1
		_BorderSize("Border Size", Range(0,1)) = 0
	}

		SubShader{

			Tags { "RenderType" = "Opaque" }

			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Health;
				float _BorderSize;

				struct MeshData {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};


				Interpolators vert(MeshData v) {
					Interpolators o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				float4 frag(Interpolators i) : SV_Target{

					float2 coords = i.uv;

					coords.x *= 8;

					float2 middleLinePoint = float2(clamp(coords.x, .5f, 7.5f), .5f);

					float sdf = distance(coords, middleLinePoint) * 2 - 1;

					clip(-sdf);

					float borderSdf = sdf + _BorderSize;

					float pd = fwidth(borderSdf); // Screen space partial derivative

					float borderMask = 1 - saturate(borderSdf / pd);

					float healthbarMask = i.uv.x < _Health;

					float4 col = tex2D(_MainTex, float2(_Health, i.uv.y));

					return float4(col * healthbarMask * borderMask);
				}
				ENDCG
			}
	}
}
