// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
	Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct VertInput {
				float4 pos : POSITION;
			};

			struct VertOutput {
				float4 pos : SV_POSITION;
				half3 color : COLOR;
			};

			VertOutput vert(VertInput i) {
			
				VertOutput o;

				o.pos = UnityObjectToClipPos(i.pos);
				o.color = i.pos.xyz *0.5;
				
				return o;
			}

			half4 frag(VertOutput i) : COLOR {
			
			return half4(i.color * 10, 0.00000001f);
			}


			ENDCG
		}
	}	
	FallBack "Diffuse"
}
