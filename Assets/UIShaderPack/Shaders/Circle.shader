Shader "UIShaderPack/Circle"
{
    Properties
    {
        [Toggle]_UseColor2("Use Secondary Color", Float) = 0
        _Color2 ("Secondary Color", Color) = (0, 0, 0, 1)
        _Edge ("Edge Softness", Float) = 1.5
        _Hollow ("Hollow", Range(0, 1)) = 0
        [IntRange] _Segments ("Segments", Range(0, 100)) = 0
        _RotateSpeed ("Rotate Speed", float) = 0
        
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
            
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _UseColor2;
            fixed4 _Color2;
            float _Hollow;
            float _RotateSpeed;
            float _Edge;
            uint _Segments;
            
            static const float PI = 3.14159265359;
            static const float SEGMENT_SIZE = PI / _Segments;
            
            fixed4 frag(v2f IN): SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                
                IN.uv -= 0.5;
                float len = length(IN.uv);
                // Get angle for calculating fwidth
                float angle1 = abs(atan((IN.uv.y) / (IN.uv.x)));
                // Get actual angle to use for positioning
                float angle = atan2(IN.uv.y, IN.uv.x) + 2 * PI + _Time.y * max(0, _RotateSpeed);

                // Interpolate between main color and secondary color based on distance from center
                color = lerp(color, _Color2,  _UseColor2 * len / 0.5);
                
                float segmentAlpha = 1;
                if (_Segments > 0)
                {
                    // Clip every second segment
                    uint segmentNumber = floor(angle / SEGMENT_SIZE);
                    if (segmentNumber % 2 == 0)
                        clip(-1);
                    else
                    {
                        // Fade edges of filled segments
                        float midAngle = (0.5 + segmentNumber) * SEGMENT_SIZE;
                        float dist = abs(angle - midAngle);
                        segmentAlpha = smoothstep(0, _Edge, (SEGMENT_SIZE * 0.5 - dist) / fwidth(angle1)) + max(0.2 - len, 0);
                    }
                }
                
                float outerAlpha = smoothstep(0, _Edge, (0.5 - len) / fwidth(len));
                float innerAlpha = _Hollow == 0 ? 1: smoothstep(0, _Edge, (len - (_Hollow * 0.5)) / fwidth(len));
                color.a *= innerAlpha * outerAlpha * segmentAlpha * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
            
        }
    }
}
