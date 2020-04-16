Shader "Unlit/Bilinear Interpolation" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_v0 ("Vertex 0", Vector) = (0,0,0,0)
		_v1 ("Vertex 1", Vector) = (0,0,0,0)
		_v2 ("Vertex 2", Vector) = (0,0,0,0)
		_v3 ("Vertex 3", Vector) = (0,0,0,0)
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// http://www.reedbeta.com/blog/quadrilateral-interpolation-part-2/

			struct vert_data {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 q : TEXCOORD0;
				float2 b1 : TEXCOORD1;
				float2 b2 : TEXCOORD2;
				float2 b3 : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _v0;
			float4 _v1;
			float4 _v2;
			float4 _v3;

			float wedge2D(float2 v, float2 w) {
				return v.x * w.y - v.y * w.x;
			}

			v2f vert(vert_data v) {
				v2f o;

				// Set up inverse bilinear interpolation
				o.q = v.vertex.xy - _v0.xy;
				o.b2 = _v1.xy - _v0.xy;
				o.b1 = _v2.xy - _v0.xy;
				o.b3 = _v0.xy - _v1.xy - _v2.xy + _v3.xy;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				// Set up quadratic formula
				float A = wedge2D(i.b2, i.b3);
				float B = wedge2D(i.b3, i.q) - wedge2D(i.b1, i.b2);
				float C = wedge2D(i.b1, i.q);

				// Solve for v
				float2 uv;
				if (abs(A) < 0.001) {
					// Linear form
					uv.x = -C / B;
				}
				else {
					// Quadratic form. Take positive root for CCW winding with V-up
					float discrim = B * B - 4 * A * C;
					uv.x = 0.5 * (-B + sqrt(discrim)) / A;
				}

				// Solve for u, using largest-magnitude component
				float2 denom = i.b1 + uv.x * i.b3;
				if (abs(denom.x) > abs(denom.y)) {
					uv.y = (i.q.x - i.b2.x * uv.x) / denom.x;
				}
				else {
					uv.y = (i.q.y - i.b2.y * uv.x) / denom.y;
				}

				// Sample the texture
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}

			ENDCG
		}
	}
}
