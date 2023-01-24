Shader "UIShaderPack/Rounded Rect"
{
    Properties
    {
        _Width ("Rect Width", Float) = 100
        _Height ("Rect Height", Float) = 100
        _Radius ("Radius", Float) = 10
        _Edge ("Edge Softness", Float) = 1.5
        
        [Header(Gradient)]
        [Toggle(USE_SECOND_COLOUR)]_UseColor2 ("Use Secondary Color", Float) = 0
        _Color2 ("Secondary Color", Color) = (0, 0, 1, 1)
        _Rotate ("Rotation", Range(0, 360)) = 0
        _Scale ("Scale", Float) = 1
        _Speed ("Scroll Speed", Float) = 0
        [Toggle] _Step ("Step", Float) = 0
        
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
            float4 _MainTex_ST;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _Edge;
            float _Width;
            float _Height;
            float _Radius;
            fixed4 _Color2;
            float _Rotate;
            float _Speed;
            float _Step;
            float _Scale;
            
            static const float PI = 3.14159265359;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                float2 size = float2(_Width, _Height);
                // Get UV from center with respect to size
                float2 uv2 = (IN.uv - 0.5) * size + 0.5;
                // Get position relative to corner
                float2 cornerPos = abs(uv2 * 2 - 1) - size + _Radius * 2;
                // Get dist as length from corner towards edge
                float dist = length(max(0, cornerPos)) / (_Radius * 2);
                
                // Gradient - see Gradient.shader
                #if USE_SECOND_COLOUR
                    float rad = _Rotate * PI / 180.0;
                    float2 pos = IN.uv - 0.5;
                    float2 newPos = float2(pos.x * cos(rad) - pos.y * sin(rad) + 0.5, pos.x * sin(rad) + pos.y * cos(rad) + 0.5);
                    
                    float t = -cos(newPos.y * PI * _Scale + _Time.y * _Speed) * 0.5 + 0.5;
                    
                    if (_Step > 0)
                        t = step(0.5, t);
                    
                    half4 color = lerp(IN.color, _Color2, t) * (tex2D(_MainTex, IN.uv) + _TextureSampleAdd);
                #else
                    half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                #endif
                
                // Fade out the edge
                color.a *= smoothstep(0, _Edge, (1 - dist) / fwidth(dist)) * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                return color;
            }
            ENDCG
            
        }
    }
}
