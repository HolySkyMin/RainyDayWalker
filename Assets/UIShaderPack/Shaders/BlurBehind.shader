Shader "UIShaderPack/BlurBehind"
{
    Properties
    {
        _BlurAmount ("Blur Amount", Range(0, 20)) = 1
        
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
            float _BlurAmount;
            
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
            
            float gaussian(float x, float y)
            {
                // Guassian distribution formula with a standard deviation of 2
                return 0.0397887 * exp((x * x + y * y) / - 8);
            }
            
            fixed4 frag(v2f IN): SV_Target
            {
                half4 color = 0;
                float gauss = 0;
                
                for (int x = -3; x <= 3; x ++)
                {
                    for (int y = -3; y <= 3; y ++)
                    {
                        // Get gaussian value at this point
                        float g = gaussian(x, y);
                        gauss += g;
                        // Add color at this pixel with respect to blur strength and gaussian amount
                        float4 coords = float4(IN.grabUV.x + x * _CameraOpaqueTexture_TexelSize.x * _BlurAmount, IN.grabUV.y + y * _CameraOpaqueTexture_TexelSize.y * _BlurAmount, IN.grabUV.z, IN.grabUV.w);
                        color += tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(coords)) * g;
                    }
                }
                
                // Average out color
                color /= gauss;
                color *= IN.color;
                color.a = tex2D(_MainTex, IN.uv).a * IN.color.a * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
            
        }
    }
}
