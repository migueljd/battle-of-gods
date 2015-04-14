// Shader created by N1warhead
// www.warhead-designz.com


Shader "UniToonUltra/Parrallax/Rims/NormalNoOutine" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ShadowColor ("Shadow", Color) = (0.0,0.0,0.0,1)
    _HighColor ("Highlighting Color", Color) = (0.5,0.5,0.5,1) 
	_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
	_RimPower ("Rim Power", Float) = 1.4
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Parallax ("Height", Range (0.005, 0.08)) = 0.02
	_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
	_Ramp ("Shading Ramp", 2D) = "gray" {}
	
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 500

CGPROGRAM
#pragma surface surf Ramp

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _ParallaxMap;
fixed4 _Color;
float _Parallax;
sampler2D _Ramp;
float4 _HighColor;
float4 _ShadowColor;
float _RimPower;
float4 _RimColor;
		
struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 viewDir;
};

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

void surf (Input IN, inout SurfaceOutput o) {
	half h = tex2D (_ParallaxMap, IN.uv_BumpMap).w;
	float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
	IN.uv_MainTex += offset;
	IN.uv_BumpMap += offset;
	
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * 2;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	half rim = 1.0f - saturate( dot(normalize(IN.viewDir), o.Normal) );
	o.Emission = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
}
ENDCG 
}
 
	Fallback "Bumped Diffuse"
}