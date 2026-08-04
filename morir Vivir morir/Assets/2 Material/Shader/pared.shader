Shader "Custom/pared"
{
      Properties
    {
        [Header(Colores)]
        _ColorBase ("Color base", Color) = (0.08, 0.25, 0.45, 1)
        _ColorCortes ("Color de cortes", Color) = (0.015, 0.04, 0.08, 1)
        _ColorTornillos ("Color de tornillos", Color) = (0.25, 0.3, 0.35, 1)

        [Header(Paneles)]
        _TamanoPanel ("Tamaño de paneles", Float) = 2
        _GrosorCorte ("Grosor de cortes", Range(0.001, 0.15)) = 0.025
        _VariacionPanel ("Variación de color", Range(0, 0.3)) = 0.06

        [Header(Tornillos)]
        _TamanoTornillo ("Tamaño de tornillos", Range(0.005, 0.15)) = 0.035
        _DistanciaTornillo ("Distancia desde el borde", Range(0.02, 0.3)) = 0.1

        [Header(Iluminacion)]
        _BrilloMinimo ("Brillo mínimo", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorBase;
                float4 _ColorCortes;
                float4 _ColorTornillos;

                float _TamanoPanel;
                float _GrosorCorte;
                float _VariacionPanel;

                float _TamanoTornillo;
                float _DistanciaTornillo;

                float _BrilloMinimo;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionWS =
                    TransformObjectToWorld(
                        input.positionOS.xyz
                    );

                output.positionCS =
                    TransformWorldToHClip(
                        output.positionWS
                    );

                output.normalWS =
                    TransformObjectToWorldNormal(
                        input.normalOS
                    );

                return output;
            }

            float RuidoCelda(float2 celda)
            {
                return frac(
                    sin(
                        dot(
                            celda,
                            float2(12.9898, 78.233)
                        )
                    ) * 43758.5453
                );
            }

            float2 ObtenerUVMundo(
                float3 posicion,
                float3 normal)
            {
                float3 normalAbsoluta =
                    abs(normal);

                if (normalAbsoluta.x >
                    normalAbsoluta.y &&
                    normalAbsoluta.x >
                    normalAbsoluta.z)
                {
                    return posicion.zy;
                }

                if (normalAbsoluta.y >
                    normalAbsoluta.x &&
                    normalAbsoluta.y >
                    normalAbsoluta.z)
                {
                    return posicion.xz;
                }

                return posicion.xy;
            }

            float CalcularTornillo(
                float2 posicionCelda,
                float2 posicionTornillo)
            {
                float distancia =
                    distance(
                        posicionCelda,
                        posicionTornillo
                    );

                return 1.0 -
                    smoothstep(
                        _TamanoTornillo,
                        _TamanoTornillo * 1.4,
                        distancia
                    );
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 normal =
                    normalize(input.normalWS);

                float tamano =
                    max(_TamanoPanel, 0.001);

                float2 uvMundo =
                    ObtenerUVMundo(
                        input.positionWS,
                        normal
                    );

                float2 uvPanel =
                    uvMundo / tamano;

                float2 celda =
                    frac(uvPanel);

                float2 numeroCelda =
                    floor(uvPanel);

                float2 distanciaBorde =
                    min(
                        celda,
                        1.0 - celda
                    );

                float distanciaCorte =
                    min(
                        distanciaBorde.x,
                        distanciaBorde.y
                    );

                float corte =
                    1.0 -
                    smoothstep(
                        _GrosorCorte,
                        _GrosorCorte * 1.5,
                        distanciaCorte
                    );

                float variacion =
                    RuidoCelda(numeroCelda);

                variacion =
                    (variacion - 0.5) *
                    _VariacionPanel;

                float distancia =
                    _DistanciaTornillo;

                float tornillo = 0;

                tornillo = max(
                    tornillo,
                    CalcularTornillo(
                        celda,
                        float2(
                            distancia,
                            distancia
                        )
                    )
                );

                tornillo = max(
                    tornillo,
                    CalcularTornillo(
                        celda,
                        float2(
                            1.0 - distancia,
                            distancia
                        )
                    )
                );

                tornillo = max(
                    tornillo,
                    CalcularTornillo(
                        celda,
                        float2(
                            distancia,
                            1.0 - distancia
                        )
                    )
                );

                tornillo = max(
                    tornillo,
                    CalcularTornillo(
                        celda,
                        float2(
                            1.0 - distancia,
                            1.0 - distancia
                        )
                    )
                );

                float3 color =
                    _ColorBase.rgb *
                    (1.0 + variacion);

                color =
                    lerp(
                        color,
                        _ColorCortes.rgb,
                        corte
                    );

                color =
                    lerp(
                        color,
                        _ColorTornillos.rgb,
                        tornillo
                    );

                Light luzPrincipal =
                    GetMainLight();

                float iluminacion =
                    saturate(
                        dot(
                            normal,
                            luzPrincipal.direction
                        )
                    );

                iluminacion =
                    max(
                        iluminacion,
                        _BrilloMinimo
                    );

                float3 luzAmbiente =
                    SampleSH(normal);

                float3 colorFinal =
                    color *
                    (
                        luzAmbiente +
                        luzPrincipal.color *
                        iluminacion
                    );

                return half4(
                    colorFinal,
                    1
                );
            }

            ENDHLSL
        }
    }
}
