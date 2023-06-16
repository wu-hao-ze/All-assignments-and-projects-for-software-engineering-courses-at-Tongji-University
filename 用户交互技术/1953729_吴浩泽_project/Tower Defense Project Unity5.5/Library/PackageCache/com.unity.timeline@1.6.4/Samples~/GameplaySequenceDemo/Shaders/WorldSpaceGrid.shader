Shader "PlaceHolder/WorldSpaceGrid"
{
    Properties
    {
        [Toggle] _EnableWorldSpace("Enable World Space", int) = 1
        _Color ("Color", Color) = (0.61,0.61,0.61,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Scale("Scale", float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float4 color : COLOR;
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        int _EnableWorldSpace;
        fixed4 _Color;
        half _Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            float2 UV;
            fixed4 c;
            if (_EnableWorldSpace == 1)
            {
                if (abs(IN.worldNormal.x) > 0.5)
                {
                    UV = IN.worldPos.yz; // side
                    c = tex2D(_MainTex, UV* _Scale);
                }
                else if (abs(IN.worldNormal.z) > 0.5)
                {
                    UV = IN.worldPos.xy; // front
                    c = tex2D(_MainTex, UV* _Scale);
                }
                else
                {
                    UV = IN.worldPos.xz; // top
                    c = tex2D(_MainTex, UV* _Scale);
                }
                o.Albedo = c.rgb * _Color;
            }
            else
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex*_Scale) * _Color;
                o.Albedo = c.rgb * _Color;
            }

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
