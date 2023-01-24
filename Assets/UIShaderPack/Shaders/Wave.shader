Shader "UIShaderPack/Wave"
{
    Properties
    {
        [Toggle]_UseColor2("Use Secondary Color", Float) = 1
        _Color2("Secondary Color", Color) = (0, 0, 1, 1)
        _Width ("Horizontal Scale", Float) = 1
        _Height ("Vertical Scale", Range(0, 0.5)) = 0.25
        _Offset ("Vertical Offset", Range(0, 1)) = 0.25
        _Speed ("Speed", Float) = 1
        _Edge ("Edge Smoothness", Float) = 1.5

        [Header(Foam)] _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamSmoothness ("Foam Smoothness", Range(0, 1)) = 0.05
        _FoamEdge ("Foam Size", Float) = 0.02

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
            float _UseColor2;
            fixed4 _Color2;
            float _Width;
            float _Height;
            float _Offset;
            float _Speed;
            float _Edge;
            float _FoamSmoothness;
            fixed4 _FoamColor;
            float _FoamEdge;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.uv;
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                // Get top position of the wave
                float yHeight = (sin(IN.uv.x * _Width + _Time.y * _Speed) + 1) * _Height + _Offset;
                // Interpolate between main color and secondary color based on y position
                half4 color = lerp(IN.color, _Color2, _UseColor2 * IN.uv.y / yHeight);

                // Foam
                float t = smoothstep(yHeight - _FoamSmoothness - _FoamEdge, yHeight - _FoamEdge, IN.uv.y);
                color = lerp(color, _FoamColor, t);

                // Fade out edge of wave
                color.a *= smoothstep(0, _Edge, (yHeight - IN.uv.y) / fwidth(IN.uv.y));
                
                // Texture and mask clipping
                color *= (tex2D(_MainTex, IN.uv) + _TextureSampleAdd);
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                return color;
            }
            ENDCG
            
        }
    }
}
