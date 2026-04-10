// Upgrade NOTE: upgraded instancing buffer 'AlonWoofDolphinStandardFlipbook' to new syntax.

// Made with Amplify Shader Editor v1.9.9.4
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Dolphin/Standard/Flipbook"
{
	Properties
	{
		_Color( "Color", Color ) = ( 1, 1, 1, 1 )
		_MainTex( "Diffuse", 2D ) = "white" {}
		[Normal] _Bump( "NormalMap", 2D ) = "bump" {}
		_Mask( "Mask", 2D ) = "white" {}
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_SpecularGloss( "Specular Gloss", Range( 0.05, 1 ) ) = 0.05
		_SpecularPower( "Specular Power", Range( 0, 1 ) ) = 0
		_Occlusion( "Occlusion", Range( 0, 1 ) ) = 1
		_Speed( "Speed", Float ) = 1
		_ColumsRows( "Colums/Rows", Vector ) = ( 1, 1, 0, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#define ASE_VERSION 19904
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Bump;
		uniform float2 _ColumsRows;
		uniform sampler2D _MainTex;
		uniform sampler2D _Mask;
		uniform float _Occlusion;
		uniform float4 _Color;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		UNITY_INSTANCING_BUFFER_START(AlonWoofDolphinStandardFlipbook)
			UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
#define _Speed_arr AlonWoofDolphinStandardFlipbook
		UNITY_INSTANCING_BUFFER_END(AlonWoofDolphinStandardFlipbook)

		void surf( Input i , inout SurfaceOutput o )
		{
			float _Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Speed_arr, _Speed);
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles35 = _ColumsRows.x * _ColumsRows.y;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset35 = 1.0f / _ColumsRows.x;
			float fbrowsoffset35 = 1.0f / _ColumsRows.y;
			// Speed of animation
			float fbspeed35 = _Time[ 1 ] * _Speed_Instance;
			// UV Tiling (col and row offset)
			float2 fbtiling35 = float2(fbcolsoffset35, fbrowsoffset35);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex35 = floor( fmod( fbspeed35 + 0.0, fbtotaltiles35) );
			fbcurrenttileindex35 += ( fbcurrenttileindex35 < 0) ? fbtotaltiles35 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox35 = round ( fmod ( fbcurrenttileindex35, _ColumsRows.x ) );
			// Multiply Offset X by coloffset
			float fboffsetx35 = fblinearindextox35 * fbcolsoffset35;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy35 = round( fmod( ( fbcurrenttileindex35 - fblinearindextox35 ) / _ColumsRows.x, _ColumsRows.y ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy35 = (int)(_ColumsRows.y-1) - fblinearindextoy35;
			// Multiply Offset Y by rowoffset
			float fboffsety35 = fblinearindextoy35 * fbrowsoffset35;
			// UV Offset
			float2 fboffset35 = float2(fboffsetx35, fboffsety35);
			// Flipbook UV
			float2 fbuv35 = i.uv_texcoord * fbtiling35 + fboffset35;
			// *** END Flipbook UV Animation vars ***
			int flipbookFrame35 = ( ( int )fbcurrenttileindex35);
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, fbuv35 ) );
			o.Normal = finalNormals12;
			float4 tex2DNode1 = tex2D( _MainTex, fbuv35 );
			float4 tex2DNode17 = tex2D( _Mask, fbuv35 );
			float lerpResult33 = lerp( 1.0 , tex2DNode17.g , _Occlusion);
			float MaskOcclusion25 = lerpResult33;
			float4 lerpResult28 = lerp( tex2DNode1 , float4( 1,1,1,0 ) , MaskOcclusion25);
			float4 finalColor19 = ( ( ( tex2DNode1 * lerpResult28 ) * _Color ) * i.vertexColor );
			o.Albedo = finalColor19.rgb;
			o.Specular = _SpecularGloss;
			float MaskSmoothness26 = tex2DNode17.b;
			o.Gloss = ( _SpecularPower * MaskSmoothness26 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19904
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;17;-1790.574,-332.2358;Inherit;True;Property;_Mask;Mask;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;32;-1672.067,-442.3097;Inherit;False;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;0;False;0;False;1;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;33;-1404.067,-345.3097;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-2036.514,-1719.635;Inherit;False;1095.825;650.78;Color;9;28;1;29;27;3;4;8;7;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;25;-1251.816,-334.0419;Inherit;False;MaskOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;37;-3408,-1280;Inherit;False;Property;_ColumsRows;Colums/Rows;9;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;38;-3408,-1136;Inherit;False;InstancedProperty;_Speed;Speed;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;40;-3408,-1408;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;29;-1980.149,-1397.849;Inherit;False;25;MaskOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;35;-2992,-1248;Inherit;False;0;0;7;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;-1;False;4;FLOAT2;0;FLOAT;1;FLOAT;2;INT;3
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1;-2027.932,-1660.821;Inherit;True;Property;_MainTex;Diffuse;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;28;-1700.069,-1555.668;Inherit;True;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;27;-1462.744,-1645.074;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-1497.934,-1298.221;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;9;-1712,-960;Inherit;False;668.2742;342.2659;Normals;2;12;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-1313.247,-1513.903;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-1251.352,-1325.419;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-1085.968,-1411.487;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-1648,-896;Inherit;True;Property;_Bump;NormalMap;2;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;26;-1184.712,-243.7626;Inherit;False;MaskSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-1296,-848;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;19;-1118.088,-1658.708;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;16;-588.715,-179.2383;Inherit;False;Property;_SpecularPower;Specular Power;6;0;Create;True;0;0;0;False;0;False;0;0.218;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;31;-538.5256,-94.42346;Inherit;False;26;MaskSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;30;-295.5256,-127.4235;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-623.715,-317.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;5;0;Create;True;0;0;0;False;0;False;0.05;0.295;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-672,-512;Inherit;False;19;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;13;-672,-448;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;0;144,-416;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;BlinnPhong;AlonWoof/Dolphin/Standard/Flipbook;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;4;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;1;35;0
WireConnection;33;1;17;2
WireConnection;33;2;32;0
WireConnection;25;0;33;0
WireConnection;35;0;40;0
WireConnection;35;1;37;1
WireConnection;35;2;37;2
WireConnection;35;3;38;0
WireConnection;1;1;35;0
WireConnection;28;0;1;0
WireConnection;28;2;29;0
WireConnection;27;0;1;0
WireConnection;27;1;28;0
WireConnection;4;0;27;0
WireConnection;4;1;3;0
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;10;1;35;0
WireConnection;26;0;17;3
WireConnection;12;0;10;0
WireConnection;19;0;7;0
WireConnection;30;0;16;0
WireConnection;30;1;31;0
WireConnection;0;0;6;0
WireConnection;0;1;13;0
WireConnection;0;3;15;0
WireConnection;0;4;30;0
ASEEND*/
//CHKSM=CEE5A81E16A7D8E696B028E8C9BF2738036EB906