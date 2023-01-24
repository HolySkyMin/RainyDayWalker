Shader "UIShaderPack/Gradient"
{
    Properties
    {
        _Color2("Secondary Color", Color) = (0, 0, 1, 1)
        _Rotate("Rotation", Range(0, 360)) = 0
        _Scale("Scale", Float) = 1
        _Speed("Scroll Speed", Float) = 0
        [Toggle] _Step("Step", Float) = 0
        
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
                OUT.uv = v.uv;
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                float rad = _Rotate * PI / 180.0;
                // Get UV from center
                float2 pos = IN.uv - 0.5;
                // Get new UV coordinates after rotation
                float2 newPos = float2(pos.x * cos(rad) - pos.y * sin(rad) + 0.5, pos.x * sin(rad) + pos.y * cos(rad) + 0.5);

                // Get interpolant value for gradient
                float t = -cos(newPos.y * PI * _Scale + _Time.y * _Speed) * 0.5 + 0.5;
                if (_Step > 0)
                    t = step(0.5, t);
                
                half4 color = lerp(IN.color, _Color2, t) * (tex2D(_MainTex, IN.uv) + _TextureSampleAdd);
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
            
        }
    }
}
