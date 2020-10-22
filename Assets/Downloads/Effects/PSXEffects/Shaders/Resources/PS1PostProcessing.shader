Shader "Hidden/PS1ColorDepth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DitherTex("", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float GetDither(float2 pos, float factor) {
				float DITHER_THRESHOLDS[16] =
				{
					1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
					13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
					4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
					16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
				};

				// Dynamic indexing isn't allowed in WebGL for some weird reason so here's this strange workaround
				#ifdef SHADER_API_GLES3
				int i = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
				for (int x = 0; x < 16; x++)
					if (x == i)
						return factor - DITHER_THRESHOLDS[x];
				return 0;
				#else
				return factor - DITHER_THRESHOLDS[(uint(pos.x) % 4) * 4 + uint(pos.y) % 4];
				#endif
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DitherTex;
			float4 _DitherTex_TexelSize;
			float _ColorDepth;
			float _Scanlines;
			float _ScanlineIntensity;
			float _Dithering;
			float _DitherThreshold;
			float _DitherIntensity;
			float _SubtractFade;
			float _FavorRed;
			float _SLDirection;
			float _ResX;
			float _ResY;

			fixed4 frag (v2f i) : SV_Target
			{
				float aspect = _ResY / _ResX;
				fixed4 col = tex2D(_MainTex, i.uv);
				#if !UNITY_COLORSPACE_GAMMA
				col.rgb = LinearToGammaSpace(col.rgb);
				#endif
				int colors = pow(2, _ColorDepth);
				
				//Scanlines
				float sl = floor(i.uv.x*_ResX % 2) * _SLDirection + floor(i.uv.y*_ResY % 2) * (1 - _SLDirection);
				col.rgb += _Scanlines * sl * _ScanlineIntensity;
				
				#ifdef UNITY_COLORSPACE_GAMMA
					half luma = LinearRgbToLuminance(GammaToLinearSpace(saturate(col.rgb)));
				#else
					half luma = LinearRgbToLuminance(col.rgb);
				#endif

				//Manipulate colors/saturate
				col.rgb -= (3 - col.rgb) * _SubtractFade;
				#if UNITY_COLORSPACE_GAMMA
					col.rgb -= _FavorRed * ((1 - col.rgb) * 0.25);
					col.r += _FavorRed * ((0.5 - col.rgb) * 0.1);
				#else
					col.rgb -= GammaToLinearSpace(_FavorRed * ((1 - col.rgb) * 0.25));
					col.r += GammaToLinearSpace(_FavorRed * ((0.5 - col.rgb) * 0.1));
				#endif

				//Calculate dithering values based on _DitherTex
				float4 ditherTex = tex2D(_DitherTex, float2(i.uv.x, i.uv.y * aspect) * _DitherTex_TexelSize.x * _ResX);
				half dither = 0;
				if (ditherTex.r == 1 && ditherTex.g == 1 && ditherTex.b == 1 && ditherTex.a == 1) {
					dither = GetDither(float2(i.uv.x, i.uv.y * aspect) * _ResX, _DitherThreshold);
				} else {
					dither = ditherTex.a;
					dither = (dither - 0.5) * 2 / _DitherThreshold;
				}

				col.rgb *= 1.0f + (luma < dither ? (1 - luma) * (1 - (_ColorDepth / 24)) * _DitherIntensity : 0) * _Dithering;
				col.rgb = floor(col.rgb * colors) / colors;

				col.rgb = saturate(col.rgb);

				#if !UNITY_COLORSPACE_GAMMA
					col.rgb = GammaToLinearSpace(col.rgb);
				#endif

				return col;
			}
			ENDCG
		}
	}
}
