Shader "Hidden/ScrollingTexture"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollValue ("Value", Range(0, 1)) = 1.0
    }

    SubShader
    {
        // No culling or depth
        // Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Tags { "Queue"="Transparent"}

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed _ScrollXSpeed;
            fixed4 _Color;
            fixed _ScrollValue;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 scrolledUV = i.uv + fixed2(0, 1 - (_ScrollValue * 2));

                fixed4 col = tex2D(_MainTex, scrolledUV) * _Color;
                col.a = tex2D(_MainTex, scrolledUV).x;
                return col;
            }
            ENDCG
        }
    }
}
