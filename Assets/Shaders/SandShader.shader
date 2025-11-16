Shader "Unlit/SandShader"
{
    Properties
    {
	_WetSandColor("Wet Sand Color", Color) = (1,1,1,1)
	_DrySandColor("Dry Sand Color", Color) = (1,1,1,1)
	_WetSandHeight("Wet Sand Height", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

	Pass
	{
		Name "Sand Color"
		HLSLPROGRAM
		#pragma fragment frag	
		#pragma vertex vert
		
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		struct VSIn 
		{
			float4 positionOS : POSITION;
			float3 normalOS   : NORMAL;
		};	

		struct FSIn 
		{
			float4 positionHCS  : SV_POSITION;	
			float3 worldPos : TEXCOORD0;
			float3 normal   : TEXCOORD1;
		};

		CBUFFER_START(UnityPerMaterial)
			half4 _DrySandColor;
			half4 _WetSandColor;
			float _WetSandHeight;
		CBUFFER_END

		FSIn vert(VSIn IN) {
			FSIn OUT;
			OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
			OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
			return OUT;
		}

		half4 frag(FSIn IN) : SV_Target 
		{
			// Calculate base color
			float h = IN.worldPos.y;
			h = smoothstep(0,_WetSandHeight, h);
			half4 dry = _DrySandColor;
			half4 wet = _WetSandColor;
			half4 base_col = lerp(dry,wet,h);
			return base_col;
		}

		ENDHLSL
	}

    }
}
