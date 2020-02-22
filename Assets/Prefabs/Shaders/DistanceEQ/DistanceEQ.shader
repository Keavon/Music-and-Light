Shader "Unlit/DistanceEQ"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            ///////////////////////////////////////
            //
            // Audio Ring
            // an experiment in Audio Viz
            //
            // Using distance to set sample points
            // for the audio channel.
            // - doing multitap but still having some
            // sdf issues.. 

#define MAX_STEPS 	255
#define MAX_DIST	250.
#define MIN_DIST	.001

#define PI  		3.1415926
#define PI2 		6.2831853
#define mod(x, y) (x-y*floor(x/y))

            const float csize = .95;
            const float size = 2.5;
            const float rep_half = size / 2.;

            // Sample Freq - Geoff - shadertoy.com/view/lt33Df
            float sampleFreq(float freq) {
                return tex2D(iChannel0, float2(freq, 0.)).x;
            }

            //iq of hsv2rgb
            float3 hsv2rgb(in float3 c) {
                float3 rgb = clamp(abs(mod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return c.z * lerp(float3(1.0), rgb, c.y);
            }

            float2x2 r2(float a) {
                float c = cos(a); float s = sin(a);
                return float2x2(c, s, -s, c);
            }

            float3 get_mouse(float3 ro) {
                float x = iMouse.xy == float2(0) ? .0 :
                    -(iMouse.y / iResolution.y * 1. - .5) * PI;
                float y = iMouse.xy == float2(0) ? .0 :
                    -(iMouse.x / iResolution.x * 1. - .5) * PI;
                float z = 0.0;

                //ro.zy *= r2(x);
                ro.xz *= r2(y);
                return ro;
            }
            // @iq for all the things sdf!
            float smin(in float a, in float b, float k) {
                float h = max(k - abs(a - b), 0.);
                return min(a, b) - h * h / (k * 4.);
            }

            float box(float3 p, float3 b) {
                float3 q = abs(p) - b;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - .15;
            }

            float3 map(float3 pos) {
                pos.xz *= r2(iTime * .06);
                float d = 1000.0;
                float audioHeight = 0.;

                float3 res = float3(d, 0., 0.);
                float3 center = float3(0., 0., 0.);

                float3 bt = pos - center;
                bt += float3(1., 0, 2.);
                float3 bi = float3(
                    floor((bt.x) / size),
                    bt.y,
                    floor((bt.z) / size)
                );
                //@dilla showed me this...
                for (int i = -1; i <= 1; ++i) {

                    float grid_x = floor(abs(mod(bt.x, 0.)) / size);
                    grid_x += abs(float(i) * .5);

                    for (int j = -1; j <= 1; ++j) {

                        float grid_z = floor(abs(mod(bt.z, 0.)) / size);
                        grid_z += abs(float(j) * .5);

                        bt.xz = mod(bt.xz, size) - .5 * size;
                        bt.xz += float2(float(i), float(j)) * size;

                        // Audio Bar Stuff
                        float dmm = (distance(bi.xz, float2(.5)));
                        float amp = 15.;
                        float freq = dmm * 0.05;
                        float smp = .5 + amp * (sampleFreq(freq));
                        //audioHeight = floor(smp)*csize; // make like steps
                        audioHeight = smp;

                        float d4 = box(bt - float3(0., -2.3 + audioHeight, 0.), float3(csize, 1., csize));
                        d = min(d, d4);
                        if (d4 < res.x) res = float3(d4, 1., audioHeight);
                    }
                }

                float d5 = pos.y;

                if (d5 < res.x) res = float3(d5, 2., audioHeight);

                return res;
            }

            float3 get_normal(in float3 p) {
                float d = map(p).x;
                float2 e = float2(.09, .0);
                float3 n = d - float3(
                    map(p - e.xyy).x,
                    map(p - e.yxy).x,
                    map(p - e.yyx).x
                );
                return normalize(n);
            }

            float3 get_ray(in float3 ro, in float3 rd) {
                float m = -1.;
                float t = .0;
                float a = .0;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 pos = ro + t * rd;

                    float3 h = map(pos);
                    m = h.y;
                    a = h.z;
                    if (abs(h.x) < (.000001 * t))
                        break;
                    t += h.x * .25; //so sorry
                    if (t > MAX_DIST)
                        break;
                }
                if (t > MAX_DIST) m = -1.;

                return float3(abs(t), m, a);
            }

            float get_diff(float3 p, float3 lpos) {
                float3 l = normalize(lpos - p);
                float3 n = get_normal(p);
                float dif = clamp(dot(n, l), 0., 1.);

                float shadow = get_ray(p + n * MIN_DIST * 2., l).x;
                if (shadow < length(p - lpos)) {
                    dif *= .2;
                }
                return dif;
            }

            float3 render(in float3 ro, in float3 rd, in float2 uv) {
                float3 color = float3(.0);
                float3 fadeColor = float3(.2, .2, .25);
                float3 ray = get_ray(ro, rd);
                float t = ray.x;
                float m = ray.y;
                float a = ray.z;

                if (t < MAX_DIST) {
                    float3 p = ro + t * rd;

                    float3 n = get_normal(p);
                    float ad = a * .05;
                    float3 tint;
                    if (m == 1.) tint = hsv2rgb(float3(.75 - ad, ad * 8., .5));
                    if (m == 2.) tint = float3(.3);

                    float wv = 5. + 5. * sin(iTime * .5);
                    float3 lpos1 = float3(35.5 - wv, 22.5, 0.1 - wv);
                    float3 lpos2 = float3(5., 25. + wv, -38.);
                    float3 diff1 = float3(.8) * get_diff(p, lpos1);
                    float3 diff2 = float3(.8) * get_diff(p, lpos2);
                    float3 diff = diff1 + diff2;

                    float3 shade = float3(1.5 - t * .005);

                    color += tint * diff;

                }
                //iq - saw it in a tutorial once
                color = lerp(color, fadeColor, 1. - exp(-0.0000045 * t * t * t));
                return pow(color, float3(0.4545));
            }

            float3 ray(in float3 ro, in float3 lp, in float2 uv) {
                // set vectors to solve intersection
                float3 cf = normalize(lp - ro);
                float3 cp = float3(0., 1., 0.);
                float3 cr = normalize(cross(cp, cf));
                float3 cu = normalize(cross(cf, cr));

                // center of the screen
                float3 c = ro + cf * .87;

                float3 i = c + uv.x * cr + uv.y * cu;
                // intersection point
                return i - ro;
            }

            void mainImage(out float4 fragColor, in float2 fragCoord) {
                // pixel screen coordinates
                float2 uv = (2. * fragCoord.xy - iResolution.xy) /
                    max(iResolution.x, iResolution.y);
                // ray origin / look at point
                float sw = 15. + 15. * sin(iTime * .25);
                float sx = 25. + 25. * cos(iTime * .15);
                float3 lp = float3(0., 4., 0.);
                float3 ro = float3(0., 45. - (sw * .5), -5. - sx);

                // get any camera movment
                ro = get_mouse(ro);
                float3 rd = ray(ro, lp, uv);
                float3 col = render(ro, rd, uv);

                fragColor = float4(col, 1.0);
            

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
