// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/WoofStandard"
{
	Properties
	{
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("BumpMap", 2D) = "bump" {}
		_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_Mask("Mask", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 1
		_Glossiness("Glossiness", Range( 0 , 1)) = 1
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _BumpMap;
		uniform float2 _Tiling;
		uniform float2 _Offset;
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform float _Metallic;
		uniform sampler2D _Mask;
		uniform float _Glossiness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord14 = i.uv_texcoord * _Tiling + _Offset;
			float2 finalUVcoords18 = uv_TexCoord14;
			float3 finalNormals41 = UnpackNormal( tex2D( _BumpMap, finalUVcoords18 ) );
			o.Normal = finalNormals41;
			float4 finalAlbedo36 = ( ( tex2D( _MainTex, finalUVcoords18 ) * _Color ) * i.vertexColor );
			o.Albedo = finalAlbedo36.rgb;
			float2 uv_Mask5 = i.uv_texcoord;
			float4 tex2DNode5 = tex2D( _Mask, uv_Mask5 );
			float baseMetalness20 = tex2DNode5.r;
			float finalMetalness24 = ( _Metallic * baseMetalness20 );
			o.Metallic = finalMetalness24;
			float baseSmoothness22 = tex2DNode5.b;
			float finalSmoothness28 = ( baseSmoothness22 * _Glossiness );
			o.Smoothness = finalSmoothness28;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
257;73;1283;640;5354.479;726.5743;3.225892;True;True
Node;AmplifyShaderEditor.CommentaryNode;17;-3554.395,-31.68936;Inherit;False;531.2866;486.7911;Tex Coords;4;18;14;15;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;16;-3521.225,185.0964;Inherit;False;Property;_Offset;Offset;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;15;-3519.125,45.19122;Inherit;False;Property;_Tiling;Tiling;7;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-3384.002,77.32666;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;32;-2735.564,-644.5569;Inherit;False;782.5732;660.2361;Albedo;7;36;34;35;3;2;33;1;;1,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;25;-3548.215,503.8414;Inherit;False;634.7842;379.4366;MaskMap;4;22;5;21;20;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-3244.987,345.8954;Inherit;False;finalUVcoords;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-3509.215,587.8278;Inherit;True;Property;_Mask;Mask;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;33;-2692.129,-543.5665;Inherit;False;18;finalUVcoords;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-3141.797,575.2956;Inherit;False;baseMetalness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;27;-2654.418,1375.35;Inherit;False;549;312;Smoothness;4;26;13;12;28;;0,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;19;-2658.446,601.0587;Inherit;False;568.2183;349.2903;Metalness;4;7;23;6;24;;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-3146.354,739.9094;Inherit;False;baseSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-2677.456,-389.1666;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2465.236,-603.9739;Inherit;True;Property;_MainTex;MainTex;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;40;-2748.062,135.577;Inherit;False;764.1711;427.0397;Normal;3;41;42;4;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;35;-2637.751,-164.3357;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;23;-2636.254,737.9861;Inherit;False;20;baseMetalness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-2711.744,259.6566;Inherit;False;18;finalUVcoords;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-2362.389,-371.1901;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2628.54,1511.718;Inherit;False;Property;_Glossiness;Glossiness;6;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2649.93,645.6834;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-2612.386,1427.3;Inherit;False;22;baseSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2340.156,-211.9869;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2418.694,795.3645;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-2487.665,270.965;Inherit;True;Property;_BumpMap;BumpMap;1;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-2328.004,1471.087;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-2304.904,1583.566;Inherit;False;finalSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2300.013,680.9124;Inherit;False;finalMetalness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;31;-2660.121,976.1487;Inherit;False;600;356.9999;Occlusion;5;29;9;8;30;10;;0,1,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-2163.239,-82.8907;Inherit;False;finalAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-2238.583,478.1541;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-1744.059,338.1866;Inherit;False;41;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2491.878,1029.444;Inherit;False;Constant;_White;White;4;0;Create;True;0;0;0;False;0;False;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-2275.516,1220.723;Inherit;False;finalOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;30;-2554.111,1099.303;Inherit;False;21;baseOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-2301.631,1066.806;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-1727.852,425.1566;Inherit;False;24;finalMetalness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-3139.656,661.9672;Inherit;False;baseOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-1719.123,207.8161;Inherit;False;36;finalAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1729.306,491.4554;Inherit;False;28;finalSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2620.12,1183.953;Inherit;False;Property;_OcclusionStrength;OcclusionStrength;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1387.758,293.3474;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AlonWoof/WoofStandard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;29;0;10;0
WireConnection;10;0;9;0
WireConnection;10;1;30;0
WireConnection;10;2;8;0
WireConnection;21;0;5;2
WireConnection;0;0;37;0
WireConnection;0;1;43;0
WireConnection;0;3;39;0
WireConnection;0;4;38;0
ASEEND*/
//CHKSM=053AD71321EBB71080AF00AFE59E5D86563A9521