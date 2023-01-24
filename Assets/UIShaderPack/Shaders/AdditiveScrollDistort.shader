Shader "UIShaderPack/AdditiveScrollDistort"
{
    Properties
    {
        [Header(Additive Scroll)] _Color("Additive Blend Color", Color) = (1, 1, 1, 1)
        [Header(Scroll)] _ScrollX("Horizontal Scroll", Float) = 0
        _ScrollY("Vertical Scroll", Float) = 0

        [Header(Distort)] _Noise ("Noise Texture", 2D) = "white" { }
        _DistortX("Distort X Strength", Float) = 0
        _DistortY("Distort Y Strength", Float) = 0

        _DistortScrollX("Distort X Scroll", Float) = 0
        _DistortScrollY("Distort Y Scroll", Float) = 0
        
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata_t
            {
                float4 vertex: POSITION;
                float4 color: COLOR;
                float2 uv: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed4 color: COLOR;
                float2 uv: TEXCOORD0;
                float4 worldPosition: TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _ScrollX;
            float _ScrollY;
            sampler2D _Noise;
            float4 _Noise_ST;
            float _DistortX;
            float _DistortY;
            float _DistortScrollX;
            float _DistortScrollY;

            fixed4 _Color;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = TRANSFORM_TEX(v.uv, _Noise);
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                // Get noise value
                float4 noise = tex2D(_Noise, IN.uv - _Time.y * float2(_DistortScrollX, _DistortScrollY));

                // Get color based on scroll and distortion
                half4 color = (tex2D(_MainTex, IN.uv - _Time.y * float2(_ScrollX, _ScrollY) + float2(noise.x * _DistortX, noise.y * _DistortY)) + _TextureSampleAdd) * IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                // Additive blend
                color.rgb += _Color.rgb;
                
                return color;
            }
            ENDCG
            
        }
    }
}
