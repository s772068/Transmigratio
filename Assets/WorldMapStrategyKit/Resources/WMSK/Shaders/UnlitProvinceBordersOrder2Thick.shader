Shader "WMSK/Unlit Province Borders Order 2 Thick" {
 
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1)
	_DashAmount("Dash Amount", Float) = 0.5
}
SubShader {

	CGINCLUDE

		fixed4 _Color;
		float _DashAmount;

		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float3 coords: TEXCOORD0;
		};

		#define COMPUTE_DASH(v, o) 	o.coords = float3(v.uv.xy, v.vertex.z); v.vertex.z = 0;

		void dash(float3 coords) {
			#if PROVINCES_DASH
				float c = lerp(coords.x, coords.y, coords.z);
				float t = frac(c * _DashAmount) - 0.5;
				clip(t);
			#endif
		}
		#define DASH(i) dash(i.coords);

	ENDCG

	LOD 300
        Tags {
        "Queue"="Geometry+260"
        "RenderType"="Opaque"
    }
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off // avoids z-fighting issues with country frontiers
	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			
		
		v2f vert(appdata v) {
			v2f o;							
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return _Color;
		}
			
		ENDCG
    }
}

SubShader {
	LOD 200
        Tags {
        "Queue"="Geometry+260"
        "RenderType"="Opaque"
    }
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off // avoids z-fighting issues with country frontiers
	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag	
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			

		
		v2f vert(appdata v) {
			v2f o;
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return _Color;
		}
			
		ENDCG
    }
    Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag	
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			
	
		v2f vert(appdata v) {
			v2f o;
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			o.pos.y += 2.0 * (o.pos.w/_ScreenParams.x);
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return fixed4(_Color.rgb, _Color.a * 0.5);
		}
			
		ENDCG
    }
	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag	
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			
		
		v2f vert(appdata v) {
			v2f o;
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			o.pos.y -= 2.0 * (o.pos.w/_ScreenParams.x);								
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return fixed4(_Color.rgb, _Color.a * 0.5);
		}
			
		ENDCG
    }
    Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag	
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			

		v2f vert(appdata v) {
			v2f o;
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			o.pos.x += 2.0 * (o.pos.w/_ScreenParams.x);
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return fixed4(_Color.rgb, _Color.a * 0.5);
		}
			
		ENDCG
    }
	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag	
		#pragma multi_compile _ PROVINCES_DASH
		#include "UnityCG.cginc"			

		v2f vert(appdata v) {
			v2f o;
			COMPUTE_DASH(v,o)
			o.pos = UnityObjectToClipPos(v.vertex);
			o.pos.x -= 2.0 * (o.pos.w/_ScreenParams.x);
			return o;									
		}
		
		fixed4 frag(v2f i) : SV_Target {
			DASH(i)
			return fixed4(_Color.rgb, _Color.a * 0.5);
		}
			
		ENDCG
    }
}
 
}
