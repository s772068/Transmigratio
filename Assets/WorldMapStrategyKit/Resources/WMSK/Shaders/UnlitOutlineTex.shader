Shader "WMSK/Unlit Outline Textured" {
 
Properties {
	_MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
    _AnimationSpeed("Animation Speed", Float) = 0.0
    _AnimationStartTime("Animation Start Time", Float) = 0.0
    _AnimationAcumOffset("Animation Acum Offset", Float) = 0.0
}
 
SubShader {
    Tags {
       "Queue"="Geometry+301"
       "RenderType"="Opaque"
  	}
  	ZWrite Off
/*	Stencil { // Uncomment to avoid outline overlap causing artifacts if color is transparent but this will prevent neighbour outlines to display properly
		Ref 4
		Comp NotEqual
		Pass Replace
		Fail Keep
    }
*/	
  	Blend SrcAlpha OneMinusSrcAlpha
    Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag				
		#include "UnityCG.cginc"

		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _AnimationSpeed, _AnimationStartTime, _AnimationAcumOffset;

		struct AppData {
			float4 vertex : POSITION;
			float2 uv: TEXCOORD0;
		};
		
		void vert(inout AppData v) {
			v.vertex = UnityObjectToClipPos(v.vertex);
			#if UNITY_REVERSED_Z
				v.vertex.z += 0.001;
			#else
				v.vertex.z -= 0.001;
			#endif	
			v.uv = TRANSFORM_TEX(v.uv, _MainTex);
			v.uv.x += _AnimationAcumOffset + _AnimationSpeed * (_Time.y - _AnimationStartTime);
		}
		
		fixed4 frag(AppData i) : SV_Target {
			fixed4 pix = tex2D(_MainTex, i.uv);
			return _Color * pix;					
		}
			
		ENDCG
    }

}
}
