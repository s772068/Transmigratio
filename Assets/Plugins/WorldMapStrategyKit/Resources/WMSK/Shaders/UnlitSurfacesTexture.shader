﻿Shader "WMSK/Unlit Surface Texture" {
 
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1)
    _WaterMaskTex ("Water Mask Tex", 2D) = "white" {}
    _WaterLevel ("Water Level", Float) = 0.1
    _AlphaOnWater ("Alpha On Wagter", Float) = 0.2
}
 
SubShader {
    Tags {
        "Queue"="Geometry+1"
        "RenderType"="Opaque"
    }
	ZWrite Off
    Offset 1, 1
    Blend SrcAlpha OneMinusSrcAlpha
	Pass {
 	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
        #pragma multi_compile _ USE_MASK
		#include "UnityCG.cginc"

		sampler2D _MainTex, _MaskTex, _WaterMaskTex;
		fixed4 _Color;
		fixed _WaterLevel, _AlphaOnWater;

		struct appdata {
			float4 vertex : POSITION;
		    float2 uv  : TEXCOORD0;
            #if USE_MASK
    		    float2 uv2 : TEXCOORD1;
            #endif
		};

		struct v2f {
			float4 pos : SV_POSITION;
		    float2 uv  : TEXCOORD0;
            #if USE_MASK
    		    float2 uv2 : TEXCOORD1;
            #endif
		};
		
		v2f vert(appdata v) {
			v2f o;							
			o.pos = UnityObjectToClipPos(v.vertex);
   			o.uv = v.uv;
            #if USE_MASK
                o.uv2 = v.uv2;
            #endif
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 color = tex2D(_MainTex, i.uv) * _Color;
            #if USE_MASK
			    fixed4 mask = tex2D(_WaterMaskTex, i.uv2);
			    if (mask.a <= _WaterLevel) {
    				color.a *= _AlphaOnWater;
	    		}
            #endif
			return color;
		}
			
	ENDCG
    }
}
}
