// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		[Normal]_NormalMap2("NormalMap2", 2D) = "bump" {}
		[Normal]_NormalMap3("NormalMap3", 2D) = "bump" {}
		_Scale("Scale", Range( 0 , 1)) = 0
		_DistortAmount("DistortAmount", Range( 0 , 1)) = 1
		_SpecularGloss("SpecularLevel", Range( 0 , 3)) = 0.5
		_DepthDistance("DepthDistance", Float) = 16
		_SpecularPower("SpecularPower", Range( 0.05 , 5)) = 2
		_DepthColor("DepthColor", Color) = (0.1376825,0.2308503,0.3207547,1)
		_DepthPower("DepthPower", Range( 0 , 1)) = 1
		_EdgePower("Edge Power", Float) = 0
		_EdgeDistance("Edge Distance", Float) = 0
		[HDR]_EdgeColor("EdgeColor", Color) = (0,0,0,0)
		_EdgeFoamTex("EdgeFoamTex", 2D) = "white" {}
		_FlowDirection2("FlowDirection2", Vector) = (0.03,0.01,0,0)
		_FlowDirection1("FlowDirection1", Vector) = (0.03,0.01,0,0)
		_FlowDirection3("FlowDirection3", Vector) = (0.03,0.01,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform float2 _FlowDirection1;
		uniform float _Scale;
		uniform sampler2D _NormalMap2;
		uniform float2 _FlowDirection2;
		uniform sampler2D _NormalMap3;
		uniform float2 _FlowDirection3;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortAmount;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthDistance;
		uniform float _DepthPower;
		uniform sampler2D _TextureSample0;
		uniform float4 _DepthColor;
		uniform float _EdgeDistance;
		uniform float _EdgePower;
		uniform float4 _EdgeColor;
		uniform sampler2D _EdgeFoamTex;
		uniform float _SpecularPower;
		uniform float _SpecularGloss;

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 panner35 = ( 1.0 * _Time.y * _FlowDirection1 + ( (ase_worldPos).xz * _Scale ));
			float2 panner68 = ( 1.0 * _Time.y * _FlowDirection2 + ( (ase_worldPos).xz * ( _Scale * 0.5 ) ));
			float2 panner97 = ( 1.0 * _Time.y * _FlowDirection3 + ( (ase_worldPos).xz * ( _Scale * 0.125 ) ));
			float3 waterNormalMix71 = BlendNormals( BlendNormals( UnpackNormal( tex2D( _NormalMap, panner35 ) ) , UnpackScaleNormal( tex2D( _NormalMap2, panner68 ), 0.5 ) ) , UnpackScaleNormal( tex2D( _NormalMap3, panner97 ), 0.5 ) );
			o.Normal = waterNormalMix71;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth77 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth77 = saturate( abs( ( screenDepth77 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDistance ) ) );
			float saferPower78 = abs( distanceDepth77 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float fresnelNdotV113 = dot( ase_normWorldNormal, ase_worldViewDir );
			float fresnelNode113 = ( 0.03 + 4.0 * pow( max( 1.0 - fresnelNdotV113 , 0.0001 ), 20.0 ) );
			float clampResult80 = clamp( ( ( pow( saferPower78 , _DepthPower ) * 1.0 ) + fresnelNode113 ) , 0.0 , 0.75 );
			float edgeStrength74 = clampResult80;
			float2 temp_output_34_0 = ( (waterNormalMix71).xy * ( _DistortAmount * edgeStrength74 ) );
			float4 screenColor12 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_screenPosNorm + float4( temp_output_34_0, 0.0 , 0.0 ) ).xy);
			float4 lerpResult49 = lerp( screenColor12 , ( float4( UnpackNormal( tex2D( _TextureSample0, panner35 ) ) , 0.0 ) * _DepthColor ) , ( _DepthColor.a * edgeStrength74 ));
			float screenDepth89 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth89 = saturate( abs( ( screenDepth89 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) ) );
			float clampResult90 = clamp( distanceDepth89 , 0.0 , 1.0 );
			float saferPower101 = abs( ( 1.0 - clampResult90 ) );
			float temp_output_101_0 = pow( saferPower101 , _EdgePower );
			float2 panner108 = ( 1.0 * _Time.y * float2( 0.035,0.02 ) + ( (ase_worldPos).xz * float2( 0.15,0.15 ) ));
			float4 clampResult126 = clamp( ( lerpResult49 + ( temp_output_101_0 * ( _EdgeColor * tex2D( _EdgeFoamTex, ( temp_output_34_0 + panner108 ) ) ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Emission = clampResult126.rgb;
			o.Specular = _SpecularPower;
			o.Gloss = _SpecularGloss;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf BlinnPhong alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 screenPos : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.WorldPosInputsNode;64;-3763.959,-32.82275;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;29;-3519.169,-434.0367;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;31;-3897.607,-249.9658;Inherit;False;Property;_Scale;Scale;4;0;Create;True;0;0;0;False;0;False;0;0.334;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2460.732,-1308.642;Inherit;False;Property;_DepthDistance;DepthDistance;7;0;Create;True;0;0;0;False;0;False;16;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;93;-4179.152,208.3373;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-3398.086,-171.6361;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;65;-3492.136,-29.71634;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;30;-3247.346,-430.9304;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-2284.034,-1157.336;Inherit;False;Property;_DepthPower;DepthPower;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-3025.409,-415.1556;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;121;-3688.42,554.532;Inherit;False;Property;_FlowDirection2;FlowDirection2;15;0;Create;True;0;0;0;False;0;False;0.03,0.01;0.03,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-3270.199,-13.94165;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;77;-2246.094,-1335.826;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-3435.925,173.1285;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.125;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;68;-3118.32,-17.19353;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.05,-0.03;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;78;-1996.77,-1257.56;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;35;-2873.53,-418.4075;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.03;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;122;-3677.42,686.532;Inherit;False;Property;_FlowDirection3;FlowDirection3;17;0;Create;True;0;0;0;False;0;False;0.03,0.01;0.03,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-3594.155,292.6984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;97;-3371.734,326.5807;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,-0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;113;-2177.325,-1002.83;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0.03;False;2;FLOAT;4;False;3;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-2636.435,-257.5245;Inherit;True;Property;_NormalMap;NormalMap;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;db994ea24e3a86a42b48aa5148d55f3b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;63;-3022.832,131.1331;Inherit;True;Property;_NormalMap2;NormalMap2;2;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.5;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1820.738,-1319.657;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;70;-2549.434,242.9302;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-1778.974,-1033.332;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;98;-3197.971,466.3463;Inherit;True;Property;_NormalMap3;NormalMap3;3;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.5;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;92;-2449.027,458.1696;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;109;-1566.939,672.0018;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-2202.647,275.4844;Inherit;False;waterNormalMix;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-1402.532,-79.80056;Inherit;False;71;waterNormalMix;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1396.188,-257.7399;Inherit;False;Property;_DistortAmount;DistortAmount;5;0;Create;True;0;0;0;False;0;False;1;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;110;-791.5349,721.0501;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;-1231.129,64.93012;Inherit;False;74;edgeStrength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-654.282,833.9274;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.15,0.15;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;27;-1139.129,-108.2638;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-1023.129,-274.3698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;108;-492.3211,737.3573;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.035,0.02;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-855.9852,-148.7526;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-395.0371,582.061;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;13;-1423.268,-477.1016;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;106;100.379,845.8342;Inherit;True;Property;_EdgeFoamTex;EdgeFoamTex;14;0;Create;True;0;0;0;False;0;False;-1;None;76dd2447f18d60e4daf8283b38908589;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;123;-1142.636,-1002.38;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;db994ea24e3a86a42b48aa5148d55f3b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;105;129.1584,526.2966;Inherit;False;Property;_EdgeColor;EdgeColor;13;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;101;248.3267,232.7319;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-639.1878,-931.0697;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;429.239,641.4662;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;12;-282.3702,-456.9362;Inherit;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;617.3748,464.1365;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;116;1128.856,-104.8565;Inherit;False;74;edgeStrength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;744.8265,35.84912;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;587.3096,-568.7322;Inherit;False;Property;_SpecularGloss;SpecularLevel;6;0;Create;False;0;0;0;False;0;False;0.5;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;125;682.7448,660.6351;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;587.1202,-638.7905;Inherit;False;Property;_SpecularPower;SpecularPower;8;0;Create;True;0;0;0;False;0;False;2;2;0.05;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;21;1470.51,-497.4448;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;0;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;683.2366,-823.1384;Inherit;False;71;waterNormalMix;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;126;968.3528,-53.31599;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-645.3204,-349.0013;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;49;213.3922,-617.3192;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;120;-3688.627,410.3351;Inherit;False;Property;_FlowDirection1;FlowDirection1;16;0;Create;True;0;0;0;False;0;False;0.03,0.01;0.03,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ComponentMaskNode;95;-3978.955,155.4939;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-544.9875,75.037;Inherit;False;Property;_EdgeDistance;Edge Distance;12;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;90;-82.35722,65.02409;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;100;46.6469,81.7301;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-110.4421,263.5504;Inherit;False;Property;_EdgePower;Edge Power;11;0;Create;True;0;0;0;False;0;False;0;6.49;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;89;-340.8832,50.0815;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;128;101.7445,380.9148;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1422.573,-1124.868;Inherit;False;edgeStrength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-15.53968,-232.8571;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-431.7894,-153.632;Inherit;False;74;edgeStrength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;85;-760.4131,-654.278;Inherit;False;Property;_DepthColor;DepthColor;9;0;Create;True;0;0;0;False;0;False;0.1376825,0.2308503,0.3207547,1;0.2476859,0.4033327,0.4339622,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;80;-1654.036,-1326.722;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.75;False;1;FLOAT;0
WireConnection;69;0;31;0
WireConnection;65;0;64;0
WireConnection;30;0;29;0
WireConnection;32;0;30;0
WireConnection;32;1;31;0
WireConnection;67;0;65;0
WireConnection;67;1;69;0
WireConnection;77;0;82;0
WireConnection;94;0;31;0
WireConnection;68;0;67;0
WireConnection;68;2;121;0
WireConnection;78;0;77;0
WireConnection;78;1;91;0
WireConnection;35;0;32;0
WireConnection;35;2;120;0
WireConnection;96;0;95;0
WireConnection;96;1;94;0
WireConnection;97;0;96;0
WireConnection;97;2;122;0
WireConnection;26;1;35;0
WireConnection;63;1;68;0
WireConnection;79;0;78;0
WireConnection;70;0;26;0
WireConnection;70;1;63;0
WireConnection;117;0;79;0
WireConnection;117;1;113;0
WireConnection;98;1;97;0
WireConnection;92;0;70;0
WireConnection;92;1;98;0
WireConnection;71;0;92;0
WireConnection;110;0;109;0
WireConnection;111;0;110;0
WireConnection;27;0;72;0
WireConnection;119;0;33;0
WireConnection;119;1;118;0
WireConnection;108;0;111;0
WireConnection;34;0;27;0
WireConnection;34;1;119;0
WireConnection;112;0;34;0
WireConnection;112;1;108;0
WireConnection;106;1;112;0
WireConnection;123;1;35;0
WireConnection;101;0;100;0
WireConnection;101;1;102;0
WireConnection;124;0;123;0
WireConnection;124;1;85;0
WireConnection;107;0;105;0
WireConnection;107;1;106;0
WireConnection;12;0;28;0
WireConnection;104;0;101;0
WireConnection;104;1;107;0
WireConnection;99;0;49;0
WireConnection;99;1;104;0
WireConnection;21;1;73;0
WireConnection;21;2;126;0
WireConnection;21;3;62;0
WireConnection;21;4;51;0
WireConnection;126;0;99;0
WireConnection;28;0;13;0
WireConnection;28;1;34;0
WireConnection;49;0;12;0
WireConnection;49;1;124;0
WireConnection;49;2;86;0
WireConnection;95;0;93;0
WireConnection;90;0;89;0
WireConnection;100;0;90;0
WireConnection;89;0;103;0
WireConnection;128;0;101;0
WireConnection;74;0;80;0
WireConnection;86;0;85;4
WireConnection;86;1;83;0
WireConnection;80;0;117;0
ASEEND*/
//CHKSM=BF05A8BA7902E5CD1EA4528363D699ED288290D4