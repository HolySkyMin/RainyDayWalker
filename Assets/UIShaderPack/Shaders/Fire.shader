Shader "UIShaderPack/Fire"
{
    Properties
    {
        _Top ("Top Color", Color) = (1, 0, 0, 1)
        _Bottom ("Bottom Color", Color) = (1, 1, 0, 1)
        
        _MiddlePos ("Middle Color Offset", Range(0, 1)) = 0.2
        _AlphaClip ("Alpha Clip", Range(0, 1)) = 0.4
        
        _Speed ("Fire Speed", Float) = 1
        _Noise ("Noise Texture", 2D) = "white" { }
        
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
                float2 noiseUV: TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            
            sampler2D _Noise;
            float4 _Noise_ST;
            fixed4 _Top;
            fixed4 _Bottom;
            float _Speed;
            float _MiddlePos;
            float _AlphaClip;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.uv;
                OUT.noiseUV = TRANSFORM_TEX(v.uv, _Noise);
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                // Get value of noise
                float noise = tex2D(_Noise, IN.noiseUV - float2(0, _Time.y * _Speed)).r;
                // Get alpha based on noise and y position
                float gradient = 1 - IN.noiseUV.y;
                float alpha = saturate(gradient - noise);
                clip(alpha - _AlphaClip);
                
                // Interpolate between bottom and top colors offset by the middle position
                half4 color = lerp(_Bottom, _Top, IN.noiseUV.y + _MiddlePos);
                
                color *= (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect) * alpha;
                return color;
            }
            ENDCG
            
        }
    }
}
