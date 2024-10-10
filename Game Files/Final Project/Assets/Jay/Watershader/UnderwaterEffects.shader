Shader "HDRP/UnderwaterEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0, 0, 1, 1)
        _FogDensity ("Fog Density", Float) = 1
        _Alpha ("Alpha", Range(0, 1)) = 0.5
        _Refraction ("Refraction", Float) = 0.1
        _NormalUV ("Normal UV", Vector) = (1, 1, 0.2, 0.1)
    }

    HLSLINCLUDE

    #pragma vertex Vert
    #pragma fragment Frag

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    TEXTURE2D(_MainTex);
    TEXTURE2D(_NormalMap);
    SAMPLER(sampler_MainTex);
    SAMPLER(sampler_NormalMap);

    float4 _Color;
    float _FogDensity;
    float _Alpha;
    float _Refraction;
    float4 _NormalUV;

    Varyings Vert(Attributes input)
    {
        Varyings output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        return output;
    }

    float4 Frag(Varyings input) : SV_Target
    {
        float3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv * _NormalUV.xy + _NormalUV.zw * _Time.y));
        
        float2 distortedUV = input.uv + normalMap.xy * _Refraction * 0.01;
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);

        float depth = LoadCameraDepth(input.positionCS.xy);
        float linearDepth = LinearEyeDepth(depth, _ZBufferParams);
        
        float fogAmount = 1.0 - exp(-_FogDensity * linearDepth);
        
        return lerp(col, _Color, saturate(fogAmount + _Alpha));
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="HDRenderPipeline" }

        Pass
        {
            Name "Underwater Effect"
            
            ZTest Always
            ZWrite Off
            Blend Off
            Cull Off

            HLSLPROGRAM
            ENDHLSL
        }
    }
}