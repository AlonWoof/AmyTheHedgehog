// Upgrade NOTE: upgraded instancing buffer 'AmyPlayerEyes' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amy/Player/Eyes"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Pupil", 2D) = "white" {}
		_Bump("Normal", 2D) = "white" {}
		_Shine("Shine", 2D) = "white" {}
		_LookY("Look_Y", Range( -1 , 1)) = 0
		_LookX("Look_X", Range( -1 , 1)) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPosition;
		};

		uniform sampler2D _Bump;
		uniform sampler2D _MainTex;
		uniform sampler2D _Shine;
		uniform float _Cutoff = 0.5;

		UNITY_INSTANCING_BUFFER_START(AmyPlayerEyes)
			UNITY_DEFINE_INSTANCED_PROP(float, _LookX)
#define _LookX_arr AmyPlayerEyes
			UNITY_DEFINE_INSTANCED_PROP(float, _LookY)
#define _LookY_arr AmyPlayerEyes
			UNITY_DEFINE_INSTANCED_PROP(float, _Opacity)
#define _Opacity_arr AmyPlayerEyes
		UNITY_INSTANCING_BUFFER_END(AmyPlayerEyes)


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float _LookX_Instance = UNITY_ACCESS_INSTANCED_PROP(_LookX_arr, _LookX);
			float _LookY_Instance = UNITY_ACCESS_INSTANCED_PROP(_LookY_arr, _LookY);
			float4 appendResult7 = (float4(_LookX_Instance , _LookY_Instance , 0.0 , 0.0));
			float2 uv_TexCoord6 = i.uv_texcoord + appendResult7.xy;
			float2 eyeOffsetUV28 = uv_TexCoord6;
			o.Normal = tex2D( _Bump, eyeOffsetUV28 ).rgb;
			float4 color12 = IsGammaSpace() ? float4(0.9339623,0.9031239,0.9031239,1) : float4(0.8562991,0.7936083,0.7936083,1);
			float4 color19 = IsGammaSpace() ? float4(0.3396226,0.08490565,0.08490565,1) : float4(0.09441278,0.00783788,0.00783788,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV18 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode18 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV18, 5.0 ) );
			float4 lerpResult20 = lerp( color12 , color19 , fresnelNode18);
			float4 baseEyeSclera13 = lerpResult20;
			float4 tex2DNode1 = tex2D( _MainTex, eyeOffsetUV28 );
			float4 lerpResult14 = lerp( baseEyeSclera13 , tex2DNode1 , tex2DNode1.a);
			float4 finalAlbedo34 = lerpResult14;
			o.Albedo = finalAlbedo34.rgb;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult44 = dot( ase_worldlightDir , ase_worldNormal );
			float NDotL45 = dotResult44;
			float clampResult50 = clamp( pow( ( ( NDotL45 * 0.5 ) + 0.5 ) , 2.0 ) , 0.0 , 1.0 );
			float halfLambert49 = max( clampResult50 , 0.1 );
			float2 uv_TexCoord8 = i.uv_texcoord + ( appendResult7 * float4( 0.85,0.85,0,0 ) ).xy;
			float2 shineOffsetUV29 = uv_TexCoord8;
			float4 tex2DNode3 = tex2D( _Shine, shineOffsetUV29 );
			float4 finalEmissive26 = ( ( ( finalAlbedo34 * halfLambert49 ) + ( finalAlbedo34 * 0.1 ) ) + tex2DNode3 );
			o.Emission = finalEmissive26.rgb;
			o.Smoothness = 0.75;
			o.Alpha = 1;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39 = Dither4x4Bayer( fmod(clipScreen39.x, 4), fmod(clipScreen39.y, 4) );
			float _Opacity_Instance = UNITY_ACCESS_INSTANCED_PROP(_Opacity_arr, _Opacity);
			dither39 = step( dither39, _Opacity_Instance );
			clip( dither39 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.screenPosition;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.screenPosition = IN.customPack2.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Node;AmplifyShaderEditor.CommentaryNode;36;-2650.023,-464.2825;Inherit;False;1178.883;503.3871;EyeCoords;8;5;4;7;9;8;6;29;28;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-2566.033,-193.3929;Inherit;False;InstancedProperty;_LookY;Look_Y;4;0;Create;False;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2559.859,-297.732;Inherit;False;InstancedProperty;_LookX;Look_X;5;0;Create;False;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;11;-2457.007,-1588.141;Inherit;False;845.0104;542.916;EyeSclera;5;12;13;18;20;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-2250.611,-236.2136;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;19;-2369.719,-1273.047;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;0.3396226,0.08490565,0.08490565,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;12;-2362.839,-1509.738;Inherit;False;Constant;_EyeScleraColor;EyeScleraColor;5;0;Create;True;0;0;0;False;0;False;0.9339623,0.9031239,0.9031239,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;18;-2113.716,-1254.508;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1942.382,-336.0906;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;20;-1987.768,-1503.329;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1694.269,-337.9135;Inherit;False;eyeOffsetUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;27;-2511.246,-1018.239;Inherit;False;1054.085;529.5724;Albedo;5;32;1;34;14;15;;1,0.03301889,0.03301889,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-2455.197,-822.092;Inherit;False;28;eyeOffsetUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1854.875,-1418.152;Inherit;False;baseEyeSclera;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-2112.956,-849.3243;Inherit;False;13;baseEyeSclera;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2240.103,-751.8213;Inherit;True;Property;_MainTex;Pupil;1;0;Create;False;0;0;0;False;0;False;-1;None;e7b1bff7c3cf6834c8f8e40e8d181a8c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2081.561,-165.3084;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.85,0.85,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1938.792,-212.8193;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;14;-1911.427,-732.9193;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;22;-2315.3,214.0891;Inherit;False;1063.338;734.648;Emissive;11;37;26;31;25;3;23;30;24;52;53;54;;0.01415092,1,0.1617021,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-1683.67,-716.1;Inherit;False;finalAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-1705.273,-242.3107;Inherit;False;shineOffsetUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-2278.568,377.9973;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-2291.007,270.2082;Inherit;False;34;finalAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-539.0634,553.0623;Inherit;False;InstancedProperty;_Opacity;Opacity;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-905.1942,-52.83315;Inherit;False;28;eyeOffsetUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-531.1067,123.9979;Inherit;False;26;finalEmissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-408.6937,-202.8629;Inherit;False;34;finalAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DitheringNode;39;-236.0635,365.0625;Inherit;False;0;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-179.8757,558.3073;Inherit;False;13;baseEyeSclera;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;69.83367,-80.30872;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Amy/Player/Eyes;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;2;-636.3065,-117.5908;Inherit;True;Property;_Bump;Normal;2;0;Create;False;0;0;0;False;0;False;-1;None;e68186d1795edfb4198e32e756d34fdc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;41;-3878.268,-400.8951;Inherit;False;741.9609;784.921;Comment;10;51;50;49;48;47;46;45;44;43;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;42;-3816.656,-334.3997;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;43;-3833.438,-179.2165;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;44;-3603.398,-263.2263;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-3593.935,-131.227;Inherit;False;NDotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-3801.455,105.8933;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-3656.605,-30.87665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;48;-3653.112,107.7667;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-3360.219,229.2155;Inherit;False;halfLambert;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;50;-3722.186,218.7036;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;51;-3556.972,285.3012;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1545.124,386.347;Inherit;False;finalEmissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-1579.8,688.447;Inherit;False;myVarName;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-2081.266,516.7155;Inherit;True;Property;_Shine;Shine;3;0;Create;True;0;0;0;False;0;False;-1;None;b0b2f4025a5f4d7419e0c76e8c7d4a2e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;30;-2265.623,731.0858;Inherit;False;29;shineOffsetUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1718.119,428.6529;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-1812.68,280.3951;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-2276.782,502.6956;Inherit;False;49;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2094.728,278.6686;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2023.28,384.3951;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-532.608,227.2874;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.75;0;0;0;0;1;FLOAT;0
WireConnection;7;0;4;0
WireConnection;7;1;5;0
WireConnection;6;1;7;0
WireConnection;20;0;12;0
WireConnection;20;1;19;0
WireConnection;20;2;18;0
WireConnection;28;0;6;0
WireConnection;13;0;20;0
WireConnection;1;1;32;0
WireConnection;9;0;7;0
WireConnection;8;1;9;0
WireConnection;14;0;15;0
WireConnection;14;1;1;0
WireConnection;14;2;1;4
WireConnection;34;0;14;0
WireConnection;29;0;8;0
WireConnection;39;0;40;0
WireConnection;0;0;35;0
WireConnection;0;1;2;0
WireConnection;0;2;38;0
WireConnection;0;4;10;0
WireConnection;0;10;39;0
WireConnection;2;1;33;0
WireConnection;44;0;42;0
WireConnection;44;1;43;0
WireConnection;45;0;44;0
WireConnection;46;0;47;0
WireConnection;47;0;45;0
WireConnection;48;0;46;0
WireConnection;49;0;51;0
WireConnection;50;0;48;0
WireConnection;51;0;50;0
WireConnection;26;0;31;0
WireConnection;37;0;3;0
WireConnection;3;1;30;0
WireConnection;31;0;53;0
WireConnection;31;1;3;0
WireConnection;53;0;54;0
WireConnection;53;1;25;0
WireConnection;25;0;23;0
WireConnection;25;1;24;0
WireConnection;54;0;23;0
WireConnection;54;1;52;0
ASEEND*/
//CHKSM=E74092EDB5E5C248BD254C8A53D2166BF2C69205