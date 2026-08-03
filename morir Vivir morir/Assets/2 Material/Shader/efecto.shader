Shader "Custom/efecto"
{
     Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Color general", Color) = (1,1,1,1)

        _ScanlineColor ("Color de líneas", Color) = (0,0,0,1)
        _ScanlineAmount ("Cantidad de líneas", Range(50, 1000)) = 350
        _ScanlineIntensity ("Intensidad de líneas", Range(0, 1)) = 0.25
        _ScanlineSpeed ("Velocidad de líneas", Range(-10, 10)) = 0.5

        _NoiseIntensity ("Intensidad de ruido", Range(0, 1)) = 0.08
        _NoiseSpeed ("Velocidad de ruido", Range(0, 20)) = 5

        _FlickerIntensity ("Intensidad de parpadeo", Range(0, 0.5)) = 0.03
        _FlickerSpeed ("Velocidad de parpadeo", Range(0, 30)) = 8

        _VignetteIntensity ("Intensidad de bordes", Range(0, 3)) = 1
        _VignetteSmoothness ("Suavidad de bordes", Range(0.01, 1)) = 0.45

        _Alpha ("Transparencia", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "CRT_UI"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;

            fixed4 _Color;
            fixed4 _ScanlineColor;

            float _ScanlineAmount;
            float _ScanlineIntensity;
            float _ScanlineSpeed;

            float _NoiseIntensity;
            float _NoiseSpeed;

            float _FlickerIntensity;
            float _FlickerSpeed;

            float _VignetteIntensity;
            float _VignetteSmoothness;

            float _Alpha;

            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;

                OUT.worldPosition = v.vertex;

                OUT.vertex =
                    UnityObjectToClipPos(v.vertex);

                OUT.texcoord = v.texcoord;

                OUT.color =
                    v.color * _Color;

                return OUT;
            }

            float Ruido(float2 uv)
            {
                float2 ruidoUV =
                    uv +
                    float2(
                        _Time.y * _NoiseSpeed,
                        _Time.y * _NoiseSpeed * 0.37
                    );

                return frac(
                    sin(
                        dot(
                            ruidoUV,
                            float2(12.9898, 78.233)
                        )
                    ) * 43758.5453
                );
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 sprite =
                    tex2D(
                        _MainTex,
                        IN.texcoord
                    ) * IN.color;

                float linea =
                    sin(
                        (
                            IN.texcoord.y *
                            _ScanlineAmount
                        ) +
                        (
                            _Time.y *
                            _ScanlineSpeed
                        )
                    );

                linea =
                    linea * 0.5 + 0.5;

                float intensidadLinea =
                    linea *
                    _ScanlineIntensity;

                float ruido =
                    Ruido(IN.texcoord);

                ruido =
                    (
                        ruido - 0.5
                    ) *
                    _NoiseIntensity;

                float2 centro =
                    IN.texcoord - 0.5;

                float distanciaCentro =
                    length(centro);

                float vignette =
                    smoothstep(
                        _VignetteSmoothness,
                        0.75,
                        distanciaCentro
                    );

                vignette *=
                    _VignetteIntensity;

                float parpadeo =
                    sin(
                        _Time.y *
                        _FlickerSpeed
                    );

                parpadeo =
                    parpadeo *
                    _FlickerIntensity;

                fixed3 colorFinal =
                    sprite.rgb;

                colorFinal =
                    lerp(
                        colorFinal,
                        _ScanlineColor.rgb,
                        intensidadLinea
                    );

                colorFinal += ruido;

                colorFinal -= vignette;

                colorFinal += parpadeo;

                fixed4 resultado;

                resultado.rgb =
                    colorFinal;

                resultado.a =
                    sprite.a *
                    _Alpha;

                resultado.a *=
                    UnityGet2DClipping(
                        IN.worldPosition.xy,
                        _ClipRect
                    );

                return resultado;
            }

            ENDCG
        }
    }
}
