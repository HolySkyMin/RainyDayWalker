Shader "UIShaderPack/Rainbow"
{
    Properties
    {
        _Scale ("Scale", Float) = 1
        _Speed ("Speed", Float) = 1
        _Rotate("Rotation", Range(0, 360)) = 0

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
            float _Scale;
            float _Speed;
            float _Rotate;

            static const float TO_RAD = 0.0174532925;
            
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
                half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                float rad = _Rotate * TO_RAD;
                // Get UV from center
                float2 pos = IN.uv - 0.5;
                // Get new UV coordinates after rotation
                float2 newPos = float2(pos.x * cos(rad) - pos.y * sin(rad) + 0.5, pos.x * sin(rad) + pos.y * cos(rad) + 0.5);

				// Get rainbow color
                fixed4 glow = float4(
					0.4 * sin(_Speed * _Time.y + newPos.y * _Scale) + 0.6,
					0.4 * sin(_Speed * _Time.y - 2.5 + newPos.y * _Scale) + 0.6,
					0.4 * sin(_Speed * _Time.y - 4 + newPos.y * _Scale) + 0.6,
				1);

				return color * glow;
            }
            ENDCG
            
        }
    }
}
