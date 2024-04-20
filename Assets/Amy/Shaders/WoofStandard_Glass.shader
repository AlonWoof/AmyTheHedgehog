// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/WoofStandard_Glass"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("BumpMap", 2D) = "bump" {}
		_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_Mask("Mask", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 1
		_OcclusionStrength("OcclusionStrength", Range( 0 , 1)) = 1
		_Glossiness("Glossiness", Range( 0 , 1)) = 1
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Emissive("Emissive", 2D) = "white" {}
		_EmissivePower("EmissivePower", Range( 0 , 5)) = 0
		_Refraction("Refraction", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
			float3 worldPos;
		};

		uniform sampler2D _BumpMap;
		uniform float2 _Tiling;
		uniform float2 _Offset;
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform float _EmissivePower;
		uniform float _Metallic;
		uniform sampler2D _Mask;
		uniform float _Glossiness;
		uniform float _OcclusionStrength;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refraction;

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) );
			float2 cameraRefraction = float2( refractionOffset.x, refractionOffset.y );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_TexCoord14 = i.uv_texcoord * _Tiling + _Offset;
			float2 finalUVcoords18 = uv_TexCoord14;
			float3 finalNormals41 = UnpackNormal( tex2D( _BumpMap, finalUVcoords18 ) );
			o.Normal = finalNormals41;
			float4 finalAlbedo36 = ( ( tex2D( _MainTex, finalUVcoords18 ) * _Color ) * i.vertexColor );
			o.Albedo = finalAlbedo36.rgb;
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			float4 finalEmissive50 = ( tex2D( _Emissive, uv_Emissive ) * _EmissivePower );
			o.Emission = finalEmissive50.rgb;
			float2 uv_Mask5 = i.uv_texcoord;
			float4 tex2DNode5 = tex2D( _Mask, uv_Mask5 );
			float baseMetalness20 = tex2DNode5.r;
			float finalMetalness24 = ( _Metallic * baseMetalness20 );
			o.Metallic = finalMetalness24;
			float baseSmoothness22 = tex2DNode5.b;
			float finalSmoothness28 = ( baseSmoothness22 * _Glossiness );
			o.Smoothness = finalSmoothness28;
			float baseOcclusion21 = tex2DNode5.g;
			float lerpResult10 = lerp( 1.0 , baseOcclusion21 , _OcclusionStrength);
			float finalOcclusion29 = lerpResult10;
			o.Occlusion = finalOcclusion29;
			o.Alpha = finalAlbedo36.a;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
				half4 color : COLOR0;
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
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				o.color = v.color;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
Node;AmplifyShaderEditor.CommentaryNode;47;-1410.565,944.7007;Inherit;False;640;477;Emissive;4;46;48;49;50;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;17;-3554.395,-31.68936;Inherit;False;531.2866;486.7911;Tex Coords;4;18;14;15;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;16;-3521.225,185.0964;Inherit;False;Property;_Offset;Offset;10;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;15;-3519.125,45.19122;Inherit;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-3384.002,77.32666;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;32;-2735.564,-644.5569;Inherit;False;782.5732;660.2361;Albedo;7;36;34;35;3;2;33;1;;1,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;25;-3548.215,503.8414;Inherit;False;634.7842;379.4366;MaskMap;4;22;5;21;20;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-3244.987,345.8954;Inherit;False;finalUVcoords;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-2692.129,-543.5665;Inherit;False;18;finalUVcoords;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-3141.797,575.2956;Inherit;False;baseMetalness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;27;-2654.418,1375.35;Inherit;False;549;312;Smoothness;4;26;13;12;28;;0,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;19;-2658.446,601.0587;Inherit;False;568.2183;349.2903;Metalness;4;7;23;6;24;;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-3146.354,739.9094;Inherit;False;baseSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2465.236,-603.9739;Inherit;True;Property;_MainTex;MainTex;2;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;40;-2748.062,135.577;Inherit;False;764.1711;427.0397;Normal;3;41;42;4;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-2636.254,737.9861;Inherit;False;20;baseMetalness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-2711.744,259.6566;Inherit;False;18;finalUVcoords;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-2362.389,-371.1901;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2628.54,1511.718;Inherit;False;Property;_Glossiness;Glossiness;8;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2649.93,645.6834;Inherit;False;Property;_Metallic;Metallic;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-2612.386,1427.3;Inherit;False;22;baseSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2340.156,-211.9869;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2418.694,795.3645;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-2487.665,270.965;Inherit;True;Property;_BumpMap;BumpMap;3;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-2328.004,1471.087;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-2304.904,1583.566;Inherit;False;finalSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2300.013,680.9124;Inherit;False;finalMetalness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;31;-2660.121,976.1487;Inherit;False;600;356.9999;Occlusion;5;29;9;8;30;10;;0,1,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-2163.239,-82.8907;Inherit;False;finalAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-2238.583,478.1541;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-1744.059,338.1866;Inherit;False;41;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2491.878,1029.444;Inherit;False;Constant;_White;White;4;0;Create;True;0;0;0;False;0;False;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;30;-2554.111,1099.303;Inherit;False;21;baseOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-2301.631,1066.806;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2620.12,1183.953;Inherit;False;Property;_OcclusionStrength;OcclusionStrength;7;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-3519.694,531.2441;Inherit;True;Property;_Mask;Mask;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;6f64405606dc8f84899d41abd23f345b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-3141.656,661.9672;Inherit;False;baseOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-2275.516,1220.723;Inherit;False;finalOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;-1385.658,1032.103;Inherit;True;Property;_Emissive;Emissive;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-1379.015,1263.577;Inherit;False;Property;_EmissivePower;EmissivePower;12;0;Create;True;0;0;0;False;0;False;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1055.015,1140.577;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-972.0146,1264.577;Inherit;False;finalEmissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-1728.99,720.1885;Inherit;False;29;finalOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1752.306,645.4554;Inherit;False;28;finalSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-1754.852,579.1566;Inherit;False;24;finalMetalness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-1758.015,445.5771;Inherit;False;50;finalEmissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;2;-2683.456,-394.1666;Inherit;False;Property;_Color;Color;4;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.6672303,0.8496123,0.9245283,0.1019608;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;35;-2641.751,-204.3357;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;37;-1766.123,118.1161;Inherit;False;36;finalAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1634.746,848.1707;Inherit;False;36;finalAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;52;-1447.731,744.349;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-888.7581,274.3474;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AlonWoof/WoofStandard_Glass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;0;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1092.724,795.3492;Inherit;False;Property;_Refraction;Refraction;13;0;Create;True;0;0;0;False;0;False;1;1;0;2;0;1;FLOAT;0
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;18;0;14;0
WireConnection;20;0;5;1
WireConnection;22;0;5;3
WireConnection;1;1;33;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;34;0;3;0
WireConnection;34;1;35;0
WireConnection;7;0;6;0
WireConnection;7;1;23;0
WireConnection;4;1;42;0
WireConnection;13;0;26;0
WireConnection;13;1;12;0
WireConnection;28;0;13;0
WireConnection;24;0;7;0
WireConnection;36;0;34;0
WireConnection;41;0;4;0
WireConnection;10;0;9;0
WireConnection;10;1;30;0
WireConnection;10;2;8;0
WireConnection;21;0;5;2
WireConnection;29;0;10;0
WireConnection;49;0;46;0
WireConnection;49;1;48;0
WireConnection;50;0;49;0
WireConnection;52;0;53;0
WireConnection;0;0;37;0
WireConnection;0;1;43;0
WireConnection;0;2;51;0
WireConnection;0;3;39;0
WireConnection;0;4;38;0
WireConnection;0;5;45;0
WireConnection;0;8;54;0
WireConnection;0;9;52;3
ASEEND*/
//CHKSM=51AC729069F114F9A53876F42B702C8D6B2295A0