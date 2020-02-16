// Default sprite shader with the addition of fog and shadow based on the z axis.

Shader "Sprites/DefaultPlus"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		[Space]

		_FogColor("FogColor", Color) = (1,1,1,1)
		_FogDistance("FogDistance", Float) = 100

		[Space]

		_ShadowColor("ShadowColor", Color) = (0,0,0,1)
		_ShadowDistance("ShadowDistance", Float) = 100
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			#ifdef UNITY_INSTANCING_ENABLED

			UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
			// SpriteRenderer.Color while Non-Batched/Instanced.
			UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
			// this could be smaller but that's how bit each entry is regardless of type
			UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
			UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

			#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
			#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

			#endif // instancing

			CBUFFER_START(UnityPerDrawSprite)
			#ifndef UNITY_INSTANCING_ENABLED
			fixed4 _RendererColor;
			fixed2 _Flip;
			#endif
			float _EnableExternalAlpha;
			CBUFFER_END

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			fixed4 _Color;
			
			uniform fixed4 _FogColor;
			uniform half _FogDistance;

			uniform fixed4 _ShadowColor;
			uniform half _ShadowDistance;

			inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
			{
				return float4(pos.xy * flip, pos.z, 1.0);
			}

			float4 Fog(float4 color, float z)
			{
				float a = color.a;

				float depth = smoothstep(0, _FogDistance, z);
				depth = clamp(depth, 0, 1);	
				color = lerp(color, _FogColor, depth);

				color.a = a;
				return color;
			}

			float4 Shadow(float4 color, float z)
			{
				float a = color.a;

				float depth = smoothstep(0, _ShadowDistance, z);
				depth = clamp(depth, 0, 1);
				color = lerp(color, _ShadowColor, depth);

				color.a = a;
				return color;
			}

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				//OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;
				OUT.color = Fog(OUT.color, mul(unity_ObjectToWorld, IN.vertex).z);
				OUT.color = Shadow(OUT.color, mul(unity_ObjectToWorld, IN.vertex).z);

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				fixed4 alpha = tex2D(_AlphaTex, uv);
				color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
				#endif

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}

	}
	Fallback "Sprites/Default"
}