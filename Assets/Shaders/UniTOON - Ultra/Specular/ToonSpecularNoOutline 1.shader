Shader "UniToonUltra/SpecularMap/NoOutline" {
	        Properties {
	      _Color ("Main Color", Color) = (1,1,1,1)
          _MainTex ("Texture", 2D) = "white" {}
          _SpecMap ("SpecMap", 2D) = "white" {}
          _BumpMap ("Normalmap", 2D) = "bump" {}
          _Ramp ("Shading Ramp", 2D) = "gray" {}
          
        }
        SubShader {
            Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Ramp
        
         sampler2D _Ramp;
         fixed4 _Color;
         
        struct MySurfaceOutput {
            
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half Specular;
            half3 GlossColor;
            half Alpha;
        };
         
         
        inline half4 LightingRamp (MySurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
          half3 h = normalize (lightDir + viewDir);
          
          half diff = max (0, dot (s.Normal, lightDir));
         
          float nh = max (0, dot (s.Normal, h));
          float spec = pow (nh, 32.0);
          half3 specCol = spec * s.GlossColor;
         
          half3 ramp = tex2D (_Ramp, float2(diff, diff)).rgb;
         
          half4 c;
          c.rgb = (s.Albedo * _LightColor0.rgb * diff * ramp + _LightColor0.rgb * specCol) * (atten * 2);
          c.a = s.Alpha;
          return c;
        }
         
        inline half4 LightingRamp_PrePass (MySurfaceOutput s, half4 light)
        {
            half3 spec = light.a * s.GlossColor;
           
            half4 c;
            c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
            c.a = s.Alpha + spec * _SpecColor.a;
            
            return c;
        }
         
        
         
        struct Input {
          float2 uv_MainTex;
          float2 uv_SpecMap;
          float2 uv_BumpMap;
        };
        sampler2D _MainTex;
        sampler2D _SpecMap;
        sampler2D _BumpMap;
         
        void surf (Input IN, inout MySurfaceOutput o)
        {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.3;
          fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * 2;
          half4 spec = tex2D (_SpecMap, IN.uv_SpecMap);
          o.Albedo = c.rgb;
          o.GlossColor = spec.rgb;
          o.Specular = 32.0/128.0;
          o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
        }
        Fallback "Diffuse"
        }
         

     

