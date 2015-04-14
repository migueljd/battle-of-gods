// Shader created by N1warhead
// www.warhead-designz.com


Shader "UniToonUltra/Basic/Basic" {
	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _ShadowColor ("Shadow", Color) = (0.0,0.0,0.0,1)
		_HighColor ("Highlighting Color", Color) = (0.5,0.5,0.5,1) 
		_MainTex ("Texture", 2D) = "white" {}
		_Ramp ("Shading Ramp", 2D) = "gray" {}
		
    }
 
    SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Ramp
 
		sampler2D _Ramp;
        float4 _HighColor;
		float4 _ShadowColor;
        
		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(diff, diff)).rgb;
			ramp = lerp(_ShadowColor,_HighColor,ramp);
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff * ramp) * (atten * 2);
			c.a = s.Alpha;
			return c;
		}
 
		struct Input {
			float2 uv_MainTex;
			
		};
		sampler2D _MainTex;
		fixed4 _Color;
 
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
		}
 
		ENDCG
  }
 
	Fallback "Diffuse"
}