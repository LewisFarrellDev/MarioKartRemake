Shader "Custom/Shader_RGB" {
	Properties {
		_Color_Red ("Red Channel", Color) = (1,1,1,1)
		_Color_Green("Green Channel", Color) = (1,1,1,1)
		_Color_Blue ("Blue Channel", Color) = (1,1,1,1)
		_Color_Alpha ("Alpha Channel", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half4 _Color_Red;
		half4 _Color_Green;
		half4 _Color_Blue;
		half4 _Color_Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			float4 c = tex2D (_MainTex, IN.uv_MainTex);

			float4 red = c.r * _Color_Red;
			float4 green = c.g * _Color_Green;
			float4 blue = c.b * _Color_Blue;
			float4 alpha = c.a * _Color_Alpha;

			float4 final = red + green + blue + alpha;

			o.Albedo = final;

			//o.Albedo.r = red;
			//o.Albedo.g = (c.g * _Color_Green);
			//o.Albedo.b = (c.b * _Color_Blue);
			//o.Alpha = (c.a * _Color_Alpha);
				
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
