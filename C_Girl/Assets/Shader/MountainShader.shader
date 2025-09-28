// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ChinesePainting/MountainShader_ColorAdjust" 
{
	Properties 
	{
		[Header(OutLine)]
		_StrokeColor ("Stroke Color", Color) = (0,0,0,1) // 轮廓色（保留原有）
		_OutlineNoise ("Outline Noise Map", 2D) = "white" {}
		_Outline ("Outline", Range(0, 1)) = 0.1
		_OutsideNoiseWidth ("Outside Noise Width", Range(1, 2)) = 1.3
		_MaxOutlineZOffset ("Max Outline Z Offset", Range(0,1)) = 0.5

		[Header(Interior)]
		_Ramp ("Ramp Texture", 2D) = "white" {} // 基础渐变贴图（原有）
		_StrokeTex ("Stroke Noise Tex", 2D) = "white" {} // 笔触贴图（原有）
		_InteriorNoise ("Interior Noise Map", 2D) = "white" {}
		_InteriorNoiseLevel ("Interior Noise Level", Range(0, 1)) = 0.15

		[Header(Color Adjustment)] // 新增：颜色调节分组
		_BaseColor ("Base Color (Tint)", Color) = (1,1,1,1) // 基础色调色（叠加到贴图上）
		_MixColor ("Mix Color", Color) = (0,0,0,0) // 混合色（可选叠加，默认透明无效果）
		_MixMode ("Mix Mode", Range(0,2)) = 0 // 混合模式：0=叠加，1=相乘，2=正常
		_Hue ("Hue", Range(-180, 180)) = 0 // 色调（-180~180度调整）
		_Saturation ("Saturation", Range(0, 3)) = 1 // 饱和度（0=灰度，1=原图，>1=增强）
		_Brightness ("Brightness", Range(0, 3)) = 1 // 亮度（0=黑色，1=原图，>1=增亮）

		[Header(Guassian Blur)] // 原有模糊参数
		radius ("Guassian Blur Radius", Range(0,60)) = 30
        resolution ("Resolution", float) = 800
        hstep("HorizontalStep", Range(0,1)) = 0.5
        vstep("VerticalStep", Range(0,1)) = 0.5  

	}
    SubShader 
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}

		// 第一个轮廓Pass（无修改，保留原有逻辑）
		Pass 
		{
			NAME "OUTLINE"
			Cull Front
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			half _MaxOutlineZOffset;

			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			};
			
			v2f vert (a2v v) 
			{
				float4 burn = tex2Dlod(_OutlineNoise, v.vertex);
				v2f o = (v2f)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;
                
				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _Outline ;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{
				fixed4 c = _StrokeColor;
				return c;
			}
			ENDCG
		}
		
		// 第二个轮廓Pass（无修改，保留原有逻辑）
		Pass 
		{
			NAME "OUTLINE 2"
			Cull Front
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float _OutsideNoiseWidth;
			half _MaxOutlineZOffset;

			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0; 
			}; 
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (a2v v) 
			{
				float4 burn = tex2Dlod(_OutlineNoise, v.vertex);
				v2f o = (v2f)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;

				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.1 * _OutsideNoiseWidth ;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target 
			{
				fixed4 c = _StrokeColor;
				fixed3 burn = tex2D(_OutlineNoise, i.uv).rgb;
				if (burn.x > 0.5)
					discard;
				return c;
			}
			ENDCG
		}
		
		// 第三个Pass：内部填充（核心修改：增加颜色调节逻辑）
		Pass 
		{
			NAME "INTERIOR"
			Tags { "LightMode"="ForwardBase" }
		
			Cull Back
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			
			// 原有参数（声明）
			sampler2D _Ramp;
			float4 _Ramp_ST;
			sampler2D _StrokeTex;
			float4 _StrokeTex_ST;
			sampler2D _InteriorNoise;
			float _InteriorNoiseLevel;
			float radius;
            float resolution;
            float hstep;
            float vstep;

			// 新增：颜色调节参数（声明，与Properties对应）
			float4 _BaseColor;    // 基础色调色
			float4 _MixColor;     // 混合色
			float _MixMode;       // 混合模式
			float _Hue;           // 色调
			float _Saturation;    // 饱和度
			float _Brightness;    // 亮度

			// 新增：HSV颜色转换函数（用于色调/饱和度/亮度调节）
			// RGB转HSV（标准化到0~1范围）
			fixed3 RGBToHSV(fixed3 rgb)
			{
				fixed maxVal = max(rgb.r, max(rgb.g, rgb.b));
				fixed minVal = min(rgb.r, min(rgb.g, rgb.b));
				fixed delta = maxVal - minVal;
				fixed3 hsv = fixed3(0, 0, maxVal);

				if (delta > 0)
				{
					if (maxVal == rgb.r)
						hsv.r = fmod(((rgb.g - rgb.b) / delta), 6.0);
					else if (maxVal == rgb.g)
						hsv.r = ((rgb.b - rgb.r) / delta) + 2.0;
					else
						hsv.r = ((rgb.r - rgb.g) / delta) + 4.0;
					hsv.r *= 60.0; // 色调转为0~360度
					if (hsv.r < 0)
						hsv.r += 360.0;
					hsv.g = delta / maxVal; // 饱和度
				}
				return hsv;
			}

			// HSV转RGB（应用色调偏移、饱和度和亮度）
			fixed3 HSVToRGB(fixed3 hsv)
			{
				fixed c = hsv.b * hsv.g;
				fixed x = c * (1.0 - abs(fmod(hsv.r / 60.0, 2.0) - 1.0));
				fixed m = hsv.b - c;
				fixed3 rgb = fixed3(0, 0, 0);

				if (hsv.r >= 0 && hsv.r < 60)
					rgb = fixed3(c, x, 0);
				else if (hsv.r < 120)
					rgb = fixed3(x, c, 0);
				else if (hsv.r < 180)
					rgb = fixed3(0, c, x);
				else if (hsv.r < 240)
					rgb = fixed3(0, x, c);
				else if (hsv.r < 300)
					rgb = fixed3(x, 0, c);
				else
					rgb = fixed3(c, 0, x);

				return rgb + m;
			}

			// 新增：颜色调节主函数（应用HSV和混合色）
			fixed3 AdjustColor(fixed3 originalColor)
			{
				// 1. 应用基础色调色（先与原图颜色相乘，改变整体基调）
				fixed3 tintedColor = originalColor * _BaseColor.rgb;

				// 2. 转换为HSV，调整色调、饱和度、亮度
				fixed3 hsv = RGBToHSV(tintedColor);
				hsv.r += _Hue; // 色调偏移（-180~180度）
				if (hsv.r < 0) hsv.r += 360;
				if (hsv.r > 360) hsv.r -= 360;
				hsv.g *= _Saturation; // 饱和度缩放（0~3）
				hsv.b *= _Brightness; // 亮度缩放（0~3）
				hsv = clamp(hsv, fixed3(0, 0, 0), fixed3(360, 1, 3)); // 限制范围，避免异常
				fixed3 adjustedColor = HSVToRGB(hsv);

				// 3. 应用混合色（根据混合模式叠加）
				switch (int(_MixMode))
				{
					case 0: // 叠加（Additive）：原图 + 混合色（适合增亮或加色）
						adjustedColor = adjustedColor + _MixColor.rgb * _MixColor.a;
						break;
					case 1: // 相乘（Multiply）：原图 * 混合色（适合变暗或调色）
						adjustedColor = adjustedColor * (_MixColor.rgb * _MixColor.a + (1 - _MixColor.a));
						break;
					case 2: // 正常（Normal）：混合色覆盖（alpha控制透明度）
						adjustedColor = lerp(adjustedColor, _MixColor.rgb, _MixColor.a);
						break;
				}

				// 4. 最终限制颜色范围（避免过曝或异常颜色）
				return clamp(adjustedColor, fixed3(0, 0, 0), fixed3(1, 1, 1));
			}

			// 原有结构体（无修改）
			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			}; 
		
			struct v2f 
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float2 uv2 : TEXCOORD3;
				SHADOW_COORDS(4)
			};
			
			// 顶点着色器（无修改，保留原有逻辑）
			v2f vert (a2v v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _Ramp);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _StrokeTex);
				o.worldNormal  = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o);
				return o;
			}
			
			// 片段着色器（核心修改：调用颜色调节函数）
			float4 frag(v2f i) : SV_Target 
			{ 
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				// 原有逻辑：采样噪声和笔触，计算Ramp贴图采样坐标
				float4 burn = tex2D(_InteriorNoise, i.uv);
				fixed diff =  dot(worldNormal, worldLightDir);
				diff = (diff * 0.5 + 0.5);
				float2 k = tex2D(_StrokeTex, i.uv).xy;
				float2 cuv = float2(diff, diff) + k * burn.xy * _InteriorNoiseLevel;
				if (cuv.x > 0.95) { cuv.x = 0.95; cuv.y = 1; }
				if (cuv.y >  0.95) { cuv.x = 0.95; cuv.y = 1; }
				cuv = clamp(cuv, 0, 1);

				// 原有逻辑：高斯模糊（柔化贴图颜色）
				float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                float2 tc = cuv;
                float blur = radius/resolution/4;     
                sum += tex2D(_Ramp, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
                sum += tex2D(_Ramp, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x, tc.y)) * 0.2270270270;
                sum += tex2D(_Ramp, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;

				// 新增：调用颜色调节函数，修改贴图颜色
				fixed3 finalColor = AdjustColor(sum.rgb);

				// 返回最终颜色（alpha固定为1，不透明）
				return float4(finalColor, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
