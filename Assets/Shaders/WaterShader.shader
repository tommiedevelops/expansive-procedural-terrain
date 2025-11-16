Shader "TerrainGen/WaterShader"
{
	Properties
	{
		_DeepColor("Deep Color", Color) = (1,1,1,1)
		_ShallowColor("Shallow Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
				"Queue"="Transparent"
    			"RenderType"="Transparent"
    			"RenderPipeline"="UniversalRenderPipeline"
				"IgnoreProjection"="True"
		}	

		Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			TEXTURE2D(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);

			struct VSIn 
			{
				float3 positionOS : POSITION;
			};

			struct FSIn
			{
				float4 positionHCS : SV_POSITION;
				float3 worldPos    : TEXCOORD0;
			};

			FSIn vert(VSIn IN)
			{
				FSIn OUT;

				OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
				OUT.worldPos = TransformObjectToWorld(IN.positionOS);

				return OUT;
			}

			half4 frag(FSIn IN) : SV_Target
			{
				float3 cam_pos = _WorldSpaceCameraPos;
				float3 view_vec = cam_pos - IN.worldPos;

				float2 uv = IN.positionHCS.xy / IN.positionHCS.w; // perspective divide
				float raw_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
				float linear_depth = LinearEyeDepth(raw_depth, _ZBufferParams);

				half4 col = half4(0.5, 0, 0, 0.8);
				return col;
			}
			
			ENDHLSL
		}
	}
}
