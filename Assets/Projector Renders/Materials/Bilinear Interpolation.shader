Shader "Unlit/Bilinear Interpolation" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_v0 ("Vertex 0", Vector) = (0,0,0,0)
		_v1 ("Vertex 1", Vector) = (0,0,0,0)
		_v2 ("Vertex 2", Vector) = (0,0,0,0)
		_v3 ("Vertex 3", Vector) = (0,0,0,0)
		_f0 ("Feather 0", Vector) = (0,0,0,0)
		_f1 ("Feather 1", Vector) = (0,0,0,0)
		_f2 ("Feather 2", Vector) = (0,0,0,0)
		_f3 ("Feather 3", Vector) = (0,0,0,0)
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

				float2 t_q : TEXCOORD4;
				float2 t_b1 : TEXCOORD5;
				float2 t_b2 : TEXCOORD6;
				float2 t_b3 : TEXCOORD7;

				float2 r_q : TEXCOORD8;
				float2 r_b1 : TEXCOORD9;
				float2 r_b2 : TEXCOORD10;
				float2 r_b3 : TEXCOORD11;

				float2 b_q : TEXCOORD12;
				float2 b_b1 : TEXCOORD13;
				float2 b_b2 : TEXCOORD14;
				float2 b_b3 : TEXCOORD15;

				float2 l_q : TEXCOORD16;
				float2 l_b1 : TEXCOORD17;
				float2 l_b2 : TEXCOORD18;
				float2 l_b3 : TEXCOORD19;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _v0;
			float4 _v1;
			float4 _v2;
			float4 _v3;
			float4 _f0;
			float4 _f1;
			float4 _f2;
			float4 _f3;

			float wedge2D(float2 v, float2 w) {
				return v.x * w.y - v.y * w.x;
			}

			float2 bilinear(float2 q, float2 b1, float2 b2, float2 b3) {
				// Set up quadratic formula
				float A = wedge2D(b2, b3);
				float B = wedge2D(b3, q) - wedge2D(b1, b2);
				float C = wedge2D(b1, q);

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
				float2 denom = b1 + uv.x * b3;
				if (abs(denom.x) > abs(denom.y)) {
					uv.y = (q.x - b2.x * uv.x) / denom.x;
				}
				else {
					uv.y = (q.y - b2.y * uv.x) / denom.y;
				}
				
				return uv;
			}

			bool insideTriangle(float2 pt, float2 a, float2 b, float2 c) {
				float area = 0.5 *(-b.y*c.x + a.y*(-b.x + c.x) + a.x*(b.y - c.y) + b.x*c.y);
				float s = 1 / (2 * area) * (a.y*c.x - a.x*c.y + (c.y - a.y)*pt.x + (a.x - c.x)*pt.y);
				float t = 1 / (2 * area) * (a.x*b.y - a.y*b.x + (a.y - b.y)*pt.x + (b.x - a.x)*pt.y);
				return (s >= 0) && (t >= 0) && (1 - s - t >= 0);
			}

			v2f vert(vert_data v) {
				v2f o;

				// Texture coordinates
				o.q = v.vertex.xy - _v0.xy;
				o.b1 = _v2.xy - _v0.xy;
				o.b2 = _v1.xy - _v0.xy;
				o.b3 = _v0.xy - _v1.xy - _v2.xy + _v3.xy;

				// Top feathering
				float2 tl = _v2.xy;
				float2 tr = _v3.xy;
				float2 br = _f3.xy;
				float2 bl = _f2.xy;
				float2 actualVertex = v.vertex.xy;
				if (distance(v.vertex.xy, _v0.xy) < 0.0001) actualVertex = bl;
				else if (distance(v.vertex.xy, _v1.xy) < 0.0001) actualVertex = br;
				o.t_q = actualVertex - bl;
				o.t_b1 = tl - bl;
				o.t_b2 = br - bl;
				o.t_b3 = bl - br - tl + tr;

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				float2 uv = bilinear(i.q, i.b1, i.b2, i.b3);
				float2 top = bilinear(i.t_q, i.t_b1, i.t_b2, i.t_b3);

				float2 p0 = float2(0, 0);
				float2 p1 = i.b1;
				float2 p2 = i.b2;
				float2 p3 = i.b1 + i.b2 + i.b3;

				// Sample the texture
				fixed4 col = tex2D(_MainTex, uv);
				// if (top.x >= 0 && top.x <= 1 && top.y >= 0 && top.y <= 1) col = float4(abs(top.y), 1, 1, 1);
				return col;
			}

			ENDCG
		}
	}
}
