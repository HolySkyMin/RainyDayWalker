Shader "UIShaderPack/DistortBehind"
{
    Properties
    {
        [Header(Distort)] _Noise ("Noise Texture", 2D) = "white" { }
        _DistortStength("Distort Strength", Float) = 20

        _DistortScrollX("Distort X Scroll", Float) = 0.05
        _DistortScrollY("Distort Y Scroll", Float) = 0.05
                
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
        
        GrabPass
        {
            "_CameraOpaqueTexture"
        }
        
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
                float4 worldPosition: TEXCOORD1;
                float2 uv: TEXCOORD0;
                float4 grabUV: TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _ClipRect;
            sampler2D _CameraOpaqueTexture;
            float4 _CameraOpaqueTexture_TexelSize;
            sampler2D _Noise;
            float4 _Noise_ST;
            float _DistortStength;
            float _DistortScrollX;
            float _DistortScrollY;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.uv;
                OUT.grabUV = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex));
                // Uncomment below if using LWRP
                // OUT.grabUV.y = 1 - OUT.grabUV.y;
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                // Get noise with respect to scroll
                float4 noise = (tex2D(_Noise, IN.uv + _Time.y * float2(_DistortScrollX, _DistortScrollY)) - 0.5) * 2;
                // Get background texture and offset by noise and distort strength

                half4 color = tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(float4(  IN.grabUV.x + _CameraOpaqueTexture_TexelSize.x * noise.x * _DistortStength,
                                                                                        IN.grabUV.y + _CameraOpaqueTexture_TexelSize.y * noise.y * _DistortStength, IN.grabUV.z, IN.grabUV.w))) * IN.color;

                color.a = tex2D(_MainTex, IN.grabUV).a * IN.color.a * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                return color;
            }
            ENDCG
        }
    }
}
