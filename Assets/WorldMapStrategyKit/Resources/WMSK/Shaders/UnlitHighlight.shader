Shader "WMSK/Unlit Highlight" {
Properties {
	[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,0.5)
}
SubShader {
    Tags {
        "Queue"="Geometry+5"
        "IgnoreProjector"="True"
        "RenderType"="Transparent"
    }
	Cull Off		
	ZWrite Off		
	Offset -5, -5
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Color;

		struct appdata {
			float4 vertex : POSITION;
		    float2 uv  : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
		    float2 uv  : TEXCOORD0;
		};
		
		v2f vert(appdata v) {
			v2f o;							
			o.pos = UnityObjectToClipPos(v.vertex);
   			o.uv = v.uv;
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 color = tex2D(_MainTex, i.uv) * _Color;
			return color;
		}
		ENDCG
	}
	}	
}
