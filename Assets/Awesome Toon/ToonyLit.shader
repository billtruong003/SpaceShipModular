Shader "MalyaWka/ShaderPack/ToonyLit"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _HighlightColor("Highlight Color", Color) = (1, 1, 0.965, 1)
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5

        // Metallic properties
        _MetallicMap("Metallic Map", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.5

        // Roughness properties
        _RoughnessMap("Roughness Map", 2D) = "white" {}
        _Roughness("Roughness", Range(0.0, 1.0)) = 0.5

        [HideInInspector] _CavityEnabled("__cavity", Float) = 1.0
        [HideInInspector] _QueueOffset("__queue", Float) = 0.0
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _ZWriteMode("__zwm", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _AlwaysOnTop("__top", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "ToonyLit" "IgnoreProjector" = "True" "ShaderModel"="2.0" }
        LOD 300

        Pass // ForwardLit
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #if defined (_SCREEN_SPACE_CAVITY)
            #include "CavityInput.hlsl"
            #endif

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float _Metallic;
            float _Roughness;
            half4 _BaseColor;
            half4 _HighlightColor;
            half4 _ShadowColor;
            half _Cutoff;
            half _Surface;
            half _CavityEnabled;
            half _AlwaysOnTop;
            CBUFFER_END

            // Khai báo Metallic Map và Roughness Map
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);

            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma multi_compile_instancing

            #pragma multi_compile_fragment _ _SCREEN_SPACE_CAVITY
            #pragma multi_compile _ _CAVITY_DEBUG

            #pragma vertex VertexToonyLit
            #pragma fragment FragmentToonyLit

            struct Attributes
            {
                float4 positionOS    : POSITION;
                float3 normalOS      : NORMAL;
                float4 tangentOS     : TANGENT;
                float2 texcoord      : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 posWS : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings VertexToonyLit(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.posWS.xyz = vertexInput.positionWS;
                output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
                output.positionCS = vertexInput.positionCS;

                if (_AlwaysOnTop)
                {
#if UNITY_REVERSED_Z
                    output.positionCS.z = output.positionCS.w - (output.positionCS.w - output.positionCS.z) * 0.1;
                    output.positionCS.z -= 0.00001;
#else
                    output.positionCS.z = -output.positionCS.w + (output.positionCS.z + output.positionCS.w) * 0.1;
                    output.positionCS.z += 0.00001;
#endif
                }

                return output;
            }

            half4 FragmentToonyLit(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 normalWS = NormalizeNormalPerPixel(input.normal);

                float2 uv = input.uv;

                // Lấy giá trị từ map hoặc slider
                float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, uv).r * _Metallic;
                float roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, uv).r * _Roughness;

                float4 __albedo = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).rgba;
                float4 __mainColor = _BaseColor.rgba;
                float __alpha = __albedo.a * __mainColor.a;
                float3 __highlightColor = _HighlightColor.rgb;
                float3 __shadowColor = _ShadowColor.rgb;

                AlphaDiscard(__alpha, _Cutoff);

                half3 albedo = __albedo.rgb;
                half alpha = __alpha;
                half3 emission = half3(0, 0, 0);

                albedo *= __mainColor.rgb;
                #ifdef _ALPHAPREMULTIPLY_ON
                albedo *= alpha;
                #endif

                Light mainLight = GetMainLight();
                half3 bakedGI = SampleSH(normalWS);
                half3 lightDir = mainLight.direction;
                half3 lightColor = mainLight.color.rgb;

                #if defined (_SCREEN_SPACE_CAVITY)
                if (_CavityEnabled)
                {
                    float2 normalizedUV = GetNormalizedScreenSpaceUV(input.positionCS);
                    half cavity = SampleCavity(normalizedUV);
                    #ifdef _CAVITY_DEBUG
                    albedo.rgb = cavity * 2.0;
                    #else
                    bakedGI *= cavity * 4.0;
                    lightColor *= cavity * 4.0;
                    #endif
                }
                #endif

                half3 indirectDiffuse = bakedGI * albedo * metallic * (1.0 - roughness);

                half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                half ndl = dot(normalWS, lightDir);

                ndl = saturate(ndl) * atten;
                // Toon shading logic
                half3 ramp = lerp(__shadowColor, __highlightColor, ndl);
                half3 rampColor = albedo * lightColor.rgb * ramp;

                // Áp dụng hiệu ứng cavity nếu cần thiết
                half cavity = 1.0; // Giá trị mặc định của cavity là 1.0 (không thay đổi)
                #if defined (_SCREEN_SPACE_CAVITY)
                if (_CavityEnabled)
                {
                    float2 normalizedUV = GetNormalizedScreenSpaceUV(input.positionCS);
                    cavity = SampleCavity(normalizedUV);

                    bakedGI *= cavity;  // Cavity ảnh hưởng đến ánh sáng gián tiếp
                    lightColor *= cavity; // Và cả ánh sáng trực tiếp
                }
                #endif

                // Ánh sáng gián tiếp với cavity nhưng không ảnh hưởng bởi roughness
                rampColor += bakedGI * albedo;

                // Tính toán Specular dựa trên metallic và roughness (Specular không bị ảnh hưởng bởi cavity)
                float3 halfDir = normalize(lightDir + normalize(_WorldSpaceCameraPos.xyz - input.posWS));  
                half specAngle = max(dot(normalWS, halfDir), 0.0);
                float specularStrength = pow(specAngle, (1.0 - roughness) * 64.0); 

                // Specular highlights dựa trên metallic và roughness
                half3 specular = specularStrength * lerp(float3(0.04, 0.04, 0.04), albedo, metallic);
                specular *= lightColor;

                // Tổng hợp ánh sáng diffuse, specular và cavity
                half3 finalColor = rampColor * (1.0 - metallic) + specular;

                // Thêm lớp emission nếu cần thiết
                finalColor += emission;

                // Trả lại màu cuối cùng
                half4 color = half4(finalColor, alpha);
                color.a = OutputAlpha(color.a, _Surface);
                return color;
            }

            ENDHLSL
        }

        // Các pass khác giữ nguyên

        Pass // DepthOnly
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            half4 _HighlightColor;
            half4 _ShadowColor;
            half _Cutoff;
            half _Surface;
            half _CavityEnabled;
            CBUFFER_END

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma multi_compile_instancing

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            struct Attributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = TransformObjectToHClip(input.position.xyz);

                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
                return 0;
            }

            ENDHLSL
        }

        Pass // DepthNormals
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            half4 _HighlightColor;
            half4 _ShadowColor;
            half _Cutoff;
            half _Surface;
            half _CavityEnabled;
            CBUFFER_END

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma multi_compile_instancing

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            struct Attributes
            {
                float4 positionOS    : POSITION;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float3 normal       : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD1;
                float3 normalWS     : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
                output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

                return output;
            }

            float4 DepthNormalsFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
                return float4(PackNormalOctRectEncode(TransformWorldToViewDir(input.normalWS, true)), 0.0, 0.0);
            }

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "MalyaWka.ShaderPack.Editor.ToonyLitShader"
}
