Shader "Hidden/Vertex Colour"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AmbientColour ("Ambient Light Colour", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			// vertex input: position, color
			struct appdata {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};
        
			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex );
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _AmbientColour;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 main = i.color;	
				main.rgb *= _AmbientColour;
				return main;
			}
			ENDCG
		}
	}
}
