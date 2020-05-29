Shader "Unlit/Video Screen Blend"
{
    Properties
    {
        _TextureA ("TextureA", 2D) = "white" {}
        _TextureB("TextureB", 2D) = "white" {}
        _Factor("Factor", Range(0, 1)) = 0
        _BlendOverWithA("BlendOverWithA", Range(0, 1)) = 0
        _BlendOverWithB("BlendOverWithB", Range(0, 1)) = 0
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

            sampler2D _TextureA;
            float4 _TextureA_ST;
            sampler2D _TextureB;
            float4 _TextureB_ST;
            float1 _Factor;
            float1 _BlendOverWithA;
            float1 _BlendOverWithB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _TextureA);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 colA = tex2D(_TextureA, i.uv);
                fixed4 colB = tex2D(_TextureB, i.uv);
                
                fixed4 col;

                if (_BlendOverWithA < 0.01 && _BlendOverWithB < 0.01) {
                    col = colA * (1 - _Factor) + colB * _Factor;
                }
                else if (_BlendOverWithA > 0.99) {
                    if ((colA.r + colA.g + colA.b) < 0.01) {
                        col = colB;
                    }
                    else {
                        col = colA;
                    }
                }
                else {
                    if ((colB.r + colB.g + colB.b) < 0.01) {
                        col = colA;
                    }
                    else {
                        col = colB;
                    }
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
