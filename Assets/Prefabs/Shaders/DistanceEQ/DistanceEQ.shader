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
            //*******************

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
#define vec3(n) float3(n, n, n)


            static const float csize = .95;
            static const float size = 2.5;
            static const float rep_half = 2.5 / 2.;

            // Sample Freq - Geoff - shadertoy.com/view/lt33Df
            float sampleFreq(float freq) {
                return texture(iChannel0, vec2(freq, 0.)).x;
            }

            //iq of hsv2rgb
            vec3 hsv2rgb(in vec3 c) {
                vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return c.z * mix(vec3(1.0), rgb, c.y);
            }

            mat2 r2(float a) {
                float c = cos(a); float s = sin(a);
                return mat2(c, s, -s, c);
            }

            vec3 get_mouse(vec3 ro) {
                float x = iMouse.xy == vec2(0) ? .0 :
                    -(iMouse.y / iResolution.y * 1. - .5) * PI;
                float y = iMouse.xy == vec2(0) ? .0 :
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

            float box(vec3 p, vec3 b) {
                vec3 q = abs(p) - b;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - .15;
            }

            vec3 map(vec3 pos) {
                pos.xz *= r2(iTime * .06);
                float d = 1000.0;
                float audioHeight = 0.;

                vec3 res = vec3(d, 0., 0.);
                vec3 center = vec3(0., 0., 0.);

                vec3 bt = pos - center;
                bt += vec3(1., 0, 2.);
                vec3 bi = vec3(
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
                        bt.xz += vec2(float(i), float(j)) * size;

                        // Audio Bar Stuff
                        float dmm = (distance(bi.xz, vec2(.5)));
                        float amp = 15.;
                        float freq = dmm * 0.05;
                        float smp = .5 + amp * (sampleFreq(freq));
                        //audioHeight = floor(smp)*csize; // make like steps
                        audioHeight = smp;

                        float d4 = box(bt - vec3(0., -2.3 + audioHeight, 0.), vec3(csize, 1., csize));
                        d = min(d, d4);
                        if (d4 < res.x) res = vec3(d4, 1., audioHeight);
                    }
                }

                float d5 = pos.y;

                if (d5 < res.x) res = vec3(d5, 2., audioHeight);

                return res;
            }

            vec3 get_normal(in vec3 p) {
                float d = map(p).x;
                vec2 e = vec2(.09, .0);
                vec3 n = d - vec3(
                    map(p - e.xyy).x,
                    map(p - e.yxy).x,
                    map(p - e.yyx).x
                );
                return normalize(n);
            }

            vec3 get_ray(in vec3 ro, in vec3 rd) {
                float m = -1.;
                float t = .0;
                float a = .0;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    vec3 pos = ro + t * rd;

                    vec3 h = map(pos);
                    m = h.y;
                    a = h.z;
                    if (abs(h.x) < (.000001 * t))
                        break;
                    t += h.x * .25; //so sorry
                    if (t > MAX_DIST)
                        break;
                }
                if (t > MAX_DIST) m = -1.;

                return vec3(abs(t), m, a);
            }

            float get_diff(vec3 p, vec3 lpos) {
                vec3 l = normalize(lpos - p);
                vec3 n = get_normal(p);
                float dif = clamp(dot(n, l), 0., 1.);

                float shadow = get_ray(p + n * MIN_DIST * 2., l).x;
                if (shadow < length(p - lpos)) {
                    dif *= .2;
                }
                return dif;
            }

            vec3 render(in vec3 ro, in vec3 rd, in vec2 uv) {
                vec3 color = vec3(.0);
                vec3 fadeColor = vec3(.2, .2, .25);
                vec3 ray = get_ray(ro, rd);
                float t = ray.x;
                float m = ray.y;
                float a = ray.z;

                if (t < MAX_DIST) {
                    vec3 p = ro + t * rd;

                    vec3 n = get_normal(p);
                    float ad = a * .05;
                    vec3 tint;
                    if (m == 1.) tint = hsv2rgb(vec3(.75 - ad, ad * 8., .5));
                    if (m == 2.) tint = vec3(.3);

                    float wv = 5. + 5. * sin(iTime * .5);
                    vec3 lpos1 = vec3(35.5 - wv, 22.5, 0.1 - wv);
                    vec3 lpos2 = vec3(5., 25. + wv, -38.);
                    vec3 diff1 = vec3(.8) * get_diff(p, lpos1);
                    vec3 diff2 = vec3(.8) * get_diff(p, lpos2);
                    vec3 diff = diff1 + diff2;

                    vec3 shade = vec3(1.5 - t * .005);

                    color += tint * diff;

                }
                //iq - saw it in a tutorial once
                color = mix(color, fadeColor, 1. - exp(-0.0000045 * t * t * t));
                return pow(color, vec3(0.4545));
            }

            vec3 ray(in vec3 ro, in vec3 lp, in vec2 uv) {
                // set vectors to solve intersection
                vec3 cf = normalize(lp - ro);
                vec3 cp = vec3(0., 1., 0.);
                vec3 cr = normalize(cross(cp, cf));
                vec3 cu = normalize(cross(cf, cr));

                // center of the screen
                vec3 c = ro + cf * .87;

                vec3 i = c + uv.x * cr + uv.y * cu;
                // intersection point
                return i - ro;
            }

            void mainImage(out vec4 fragColor, in vec2 fragCoord) {
                // pixel screen coordinates
                vec2 uv = (2. * fragCoord.xy - iResolution.xy) /
                    max(iResolution.x, iResolution.y);
                // ray origin / look at point
                float sw = 15. + 15. * sin(iTime * .25);
                float sx = 25. + 25. * cos(iTime * .15);
                vec3 lp = vec3(0., 4., 0.);
                vec3 ro = vec3(0., 45. - (sw * .5), -5. - sx);

                // get any camera movment
                ro = get_mouse(ro);
                vec3 rd = ray(ro, lp, uv);
                vec3 col = render(ro, rd, uv);

                fragColor = vec4(col, 1.0);
            }




            //*******************
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
