﻿Shader "WMSK/Lit Viewport Wrapped" {
	Properties {
		_MainTex ("Primary Tex (RGB)", 2D) = "white" {}
        _WrappedTex ("Wrapped Tex (RGB)", 2D) = "black" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _WrapEnabled ("Wrap Enabled", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0
		#pragma multi_compile_local _ WMSK_VIEWPORT_UNLIT

		sampler2D _MainTex, _WrappedTex;
		float2 _MainTex_TexelSize;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half4 _Color;
        half _WrapEnabled;

		void vert (inout appdata_full v, out Input data) {
          UNITY_INITIALIZE_OUTPUT(Input,data);
      }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            c = max(c, _WrapEnabled * tex2D (_WrappedTex, IN.uv_MainTex));
			#if WMSK_VIEWPORT_UNLIT
				o.Emission = c.rgb;
				o.Albedo = 0;
			#else
				o.Albedo = c.rgb;
			#endif

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness * c.a;
			o.Alpha = 1.0; //c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
