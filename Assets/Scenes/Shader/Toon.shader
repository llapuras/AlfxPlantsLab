// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lapu/ToonBasic" {
	Properties{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ToonShade("ToonShader Cubemap(RGB)", CUBE) = "" { }
		_Atten("Intensity",Range(0,10)) = 1
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			Blend SrcAlpha OneMinusSrcAlpha
			Pass {
				Name "BASE"
				Cull Off
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				samplerCUBE _ToonShade;
				float4 _MainTex_ST;
				float4 _Color;
				float _Atten;

				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float3 cubenormal : TEXCOORD1;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);// UnityCG.cginc file contains function to transform
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);	
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
					fixed4 c = fixed4(_Atten  * col.rgb, col.a);
					return c;
				}
				ENDCG
			}
	}

		Fallback "VertexLit"
}
