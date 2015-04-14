// Shader created by N1warhead
// www.warhead-designz.com


Shader "UniToonUltra/Parrallax/Rims/SpecularOutline" {
    Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ShadowColor ("Shadow", Color) = (0.0,0.0,0.0,1)
    _HighColor ("Highlighting Color", Color) = (0.5,0.5,0.5,1) 
	_OutlineColor ("Outline Color", Color) = (0,0,0,1)
	_Outline ("Outline width", Range (.002, 0.03)) = .005
	_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
	_RimPower ("Rim Power", Float) = 1.4
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Ramp ("Shading Ramp", 2D) = "gray" {}
	_SpecMap ("SpecMap", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Parallax ("Height", Range (0.005, 0.2)) = 0.02
	_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 600

CGPROGRAM
#pragma surface surf Ramp
#pragma target 3.0


sampler2D _MainTex;
sampler2D _SpecMap;
sampler2D _BumpMap;
sampler2D _ParallaxMap;
fixed4 _Color;
float _Parallax;
sampler2D _Ramp;
float4 _HighColor;
float4 _ShadowColor;
float _RimPower;
float4 _RimColor;

struct MySurfaceOutput {
	float2 uv_MainTex;
	float2 uv_SpecMap;
	float2 uv_BumpMap;
	float3 viewDir;
	half3 Albedo;
    half3 Normal;
    half3 Emission;
    half Specular;
    half3 GlossColor;
    half Alpha;
	
};

half4 LightingRamp (MySurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 h = normalize (lightDir + viewDir);
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			
			half3 ramp = tex2D (_Ramp, float2(diff, diff)).rgb;
			ramp = lerp(_ShadowColor,_HighColor,ramp);
			
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 32.0);
            half3 specCol = spec * s.GlossColor;
            
			half4 c;
			
		    c.rgb = (s.Albedo * _LightColor0.rgb * diff * ramp + _LightColor0.rgb * specCol) * (atten * 2);
		    c.a = s.Alpha;
		    c.a = s.Alpha + spec * _SpecColor.a;
			return c;
		}

inline half4 LightingRamp_PrePass (MySurfaceOutput s, half4 light)
        {
            half3 spec = light.a * s.GlossColor;
           
            half4 c;
            c.rgb = (s.Albedo * light.rgb + light.rgb * spec * _SpecColor.a);
           // c.a = s.Alpha + spec * _SpecColor.a;
            
            return c;
        }

struct Input {
          float2 uv_MainTex;
          float2 uv_SpecMap;
          float2 uv_BumpMap;
          float3 viewDir;
        };
       

void surf (Input IN, inout MySurfaceOutput o) {
    half h = tex2D (_ParallaxMap, IN.uv_BumpMap).w;
	float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
	half4 spec = tex2D (_SpecMap, IN.uv_MainTex);
	IN.uv_MainTex += offset;
	
	
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * 2;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.GlossColor = spec.rgb;
    o.Specular = 32.0/128.0 * 5;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	half rim = 1.0f - saturate( dot(normalize(IN.viewDir), o.Normal) );
	o.Emission = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
}

ENDCG 
CGINCLUDE
		#include "UnityCG.cginc"
 
		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};
 
		struct v2f {
			float4 pos : POSITION;
			float4 color : COLOR;
		};
 
		uniform float _Outline;
		uniform float4 _OutlineColor;
 
		v2f vert(appdata v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
			float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
			float2 offset = TransformViewToProjection(norm.xy);
 
			o.pos.xy += offset * o.pos.z * _Outline;
			o.color = _OutlineColor;
			return o;
		}
		ENDCG
 
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite On
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			half4 frag(v2f i) :COLOR { return i.color; }
			ENDCG
		}
 
    }
 
	Fallback "Bumped Diffuse"
}