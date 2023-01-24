Shader "UIShaderPack/Dissolve"
{
    Properties
    {
        _Noise ("Noise Texture", 2D) = "white" { }
        [Header(Edge)]
        _Cutoff ("Clip Threshold", Range(0, 1)) = 0.5
        _Size ("Clip Smoothness", Range(0, 0.5)) = 0.02

        [Header(Color)]
        _ColorSize ("Color Size", Range(0, 0.5)) = 0.01
        _ColorSmoothness ("Color Smoothness", Range(0, 0.5)) = 0.02
        _Color ("Edge Color", Color) = (1, 1, 1, 1)
        [Toggle(USE_SECOND_COLOUR)]_UseColor2 ("Use Secondary Color", Float) = 0
        _Color2 ("Edge Secondary Color", Color) = (0, 0, 0, 1)
        
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
            
            #pragma shader_feature USE_SECOND_COLOUR
            
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
            
            sampler2D _Noise;
            float4 _Noise_ST;
            
            float _Cutoff;
            float _Edge;
            
            float _Size;
            float _ColorSize;
            float _ColorSmoothness;
            fixed4 _Color;
            fixed4 _Color2;
            float _UseColor2;
            
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
                half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                // Get noise value
                float noise = tex2D(_Noise, IN.uv);
                // Cut off and fade out
                color.a *= smoothstep(_Cutoff, _Cutoff + _Size, noise);
                
                #if USE_SECOND_COLOUR
                    // Set edge colors by interpolating between the two
                    float edge = smoothstep(_Cutoff + _Size + _ColorSize, _Cutoff + _Size + _ColorSmoothness + _ColorSize, noise);
                    if (edge < 0.5)
                        color.rgb = lerp(_Color.rgb, _Color2.rgb, edge / 0.5);
                    else
                        color.rgb = lerp(_Color2.rgb, color.rgb, (edge - 0.5) / 0.5);
                #else
                    // Set edge color
                    float edge = smoothstep(_Cutoff + _Size + _ColorSize, _Cutoff + _Size + _ColorSmoothness + _ColorSize, noise);
                    color.rgb = lerp(_Color.rgb, color.rgb, edge);
                #endif
                
                return color;
            }
            ENDCG
            
        }
    }
}
