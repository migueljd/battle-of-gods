// Shader created by N1warhead
// www.warhead-designz.com


Shader "UniToonUltra/Normals/NoOutline" {
	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Ramp ("Shading Ramp", 2D) = "gray" {}
    }
 
    SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Ramp
 
		sampler2D _Ramp;
        
        
		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(diff, diff)).rgb;
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff * ramp) * (atten * 2);
			c.a = s.Alpha;
			return c;
		}
 
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};
		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
 
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color * 2;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
 
		ENDCG
    }
 
	Fallback "Diffuse"
}