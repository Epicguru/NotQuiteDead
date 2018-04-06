Shader "Chunks/BG_Blend"
{
	Properties
	{
		[Header(Core)]
		_MainTex("Main Texture", 2D) = "white" {}

		 // Textures...
		[Header(Surrounding Textures)]
		_Tex0("Top Left Texture", 2D) = "white" {}
		_Tex1("Top Texture", 2D) = "white" {}
		_Tex2("Top Right Texture", 2D) = "white" {}
		_Tex3("Right Texture", 2D) = "white" {}
		_Tex4("Bottom Right Texture", 2D) = "white" {}
		_Tex5("Bottom Texture", 2D) = "white" {}
		_Tex6("Bottom Left Texture", 2D) = "white" {}
		_Tex7("Left Texture", 2D) = "white" {}

		// Masks
		[Header(Masks)]
		_Msk0("Top Left Mask", 2D) = "white" {}
		_Msk1("Top Mask", 2D) = "white" {}
		_Msk2("Top Right Mask", 2D) = "white" {}
		_Msk3("Right Mask", 2D) = "white" {}
		_Msk4("Bottom Right Mask", 2D) = "white" {}
		_Msk5("Bottom Mask", 2D) = "white" {}
		_Msk6("Bottom Left Mask", 2D) = "white" {}
		_Msk7("Left Mask", 2D) = "white" {}


		_Tiling ("Tiling", Float) = 16.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			sampler2D _Tex0;
			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _Tex4;
			sampler2D _Tex5;
			sampler2D _Tex6;
			sampler2D _Tex7;

			sampler2D _Msk0;
			sampler2D _Msk1;
			sampler2D _Msk2;
			sampler2D _Msk3;
			sampler2D _Msk4;
			sampler2D _Msk5;
			sampler2D _Msk6;
			sampler2D _Msk7;

			float _Tiling;

			fixed4 frag (v2f i) : SV_Target
			{				
				float2 uv = i.uv * _Tiling;

				// White colour.
				const fixed4 white = fixed4(1., 1., 1., 1.);

				// Get the main colour.
				fixed4 mainCol = tex2D(_MainTex, uv);	
				
				// Value of 1 is full effect.
				// Value of 0 has no effect on the texture.

				// Corners...

				// Top left
				float strength = tex2D(_Msk0, i.uv).r;
				fixed4 col = tex2D(_Tex0, uv);
				mainCol = lerp(mainCol, col, strength);

				// Top right
				strength = tex2D(_Msk2, i.uv).r;
				col = tex2D(_Tex2, uv);
				mainCol = lerp(mainCol, col, strength);

				// Bottom right
				strength = tex2D(_Msk4, i.uv).r;
				col = tex2D(_Tex4, uv);
				mainCol = lerp(mainCol, col, strength);

				// Bottom left
				strength = tex2D(_Msk6, i.uv).r;
				col = tex2D(_Tex6, uv);
				mainCol = lerp(mainCol, col, strength);

				// Sides...

				// Top
				strength = tex2D(_Msk1, i.uv).r;
				col = tex2D(_Tex1, uv);
				mainCol = lerp(mainCol, col, strength);

				// Right
				strength = tex2D(_Msk3, i.uv).r;
				col = tex2D(_Tex3, uv);
				mainCol = lerp(mainCol, col, strength);

				// Bottom
				strength = tex2D(_Msk5, i.uv).r;
				col = tex2D(_Tex5, uv);
				mainCol = lerp(mainCol, col, strength);

				// Left
				strength = tex2D(_Msk7, i.uv).r;
				col = tex2D(_Tex7, uv);
				mainCol = lerp(mainCol, col, strength);

				return mainCol;
			}
			ENDCG
		}
	}
}
