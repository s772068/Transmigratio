Shader "WMSK/Unlit Marker Line" {
 
Properties {
    _Color ("Color", Color) = (1,1,1,0.5)
    _MainTex("Texture (RGBA)", 2D) = "white" {}
    _StencilComp("Stencil Comp", Int) = 8
}
 
SubShader {
         Tags {"Queue"="Geometry+301" "IgnoreProjector"="True" "RenderType"="Transparent"} 
         Offset -1,-1
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha 
         ColorMask RGB
         Stencil {
			Ref 1
			ReadMask 1
			Comp [_StencilComp]
			Pass replace
         }
	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
		#pragma multi_compile_local _ _VIEWPORT_CROP

		#include "UnityCG.cginc"

		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4x4 _ViewportInvProj;

		struct AppData {
			float4 vertex : POSITION;
			float2 uv: TEXCOORD0;
			#if _VIEWPORT_CROP
				float3 wpos: TEXCOORD1;
			#endif	
		};
		
		void vert(inout AppData v) {
			#if _VIEWPORT_CROP
				v.wpos = mul(unity_ObjectToWorld, v.vertex);
			#endif
			v.vertex = UnityObjectToClipPos(v.vertex);
			v.uv = TRANSFORM_TEX(v.uv, _MainTex);
		}
		
		fixed4 frag(AppData i) : SV_Target {
			#if _VIEWPORT_CROP
				float3 localPos = mul(_ViewportInvProj, float4(i.wpos, 1.0)).xyz;
				if (localPos.x < -0.5 || localPos.x > 0.5 || localPos.y < - 0.5 || localPos.y > 0.5 || localPos.z > 0) discard;
			#endif
			fixed4 pix = tex2D(_MainTex, i.uv);
			return _Color * pix;					
		}
			
		ENDCG
    }
 }

}
 