Shader "Unlit/Pick Colored" {
    Properties {
       _MainTex ("Base (RGB)", 2D) = "white" {}
       _Color ("AlphaColor", Color) = (0,0,0,1)
    }
    SubShader {
		LOD 200
		Lighting On
		Fog { Mode Off }
		Tags { "Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Opaque"}       
 
       CGPROGRAM
       #pragma surface surf Lambert alpha
 
       sampler2D _MainTex;
 
       struct Input {
         float2 uv_MainTex;
       };
       float4 _Color;
 
    void surf (Input IN, inout SurfaceOutput o) {
        half4 c = tex2D (_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        if(c.r == _Color.r && c.g == _Color.g && c.b == _Color.b)
            o.Alpha = 0;
        else
            o.Alpha = 1; 
    }
       ENDCG
    } 
    FallBack "Diffuse"
}
