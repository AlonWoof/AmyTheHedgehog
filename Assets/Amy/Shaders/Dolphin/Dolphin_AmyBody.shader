// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Dolphin/AmyBody"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Diffuse", 2D) = "white" {}
		[Normal]_Bump("NormalMap", 2D) = "bump" {}
		_Mask("Mask", 2D) = "white" {}
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_SpecularGloss("Specular Gloss", Range( 0.05 , 1)) = 0.05
		_SpecularPower("Specular Power", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", Range( 0 , 1)) = 1
		_FlashColor("FlashColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Bump;
		uniform float4 _Bump_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _Occlusion;
		uniform float4 _Color;
		uniform float4 _FlashColor;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Bump = i.uv_texcoord * _Bump_ST.xy + _Bump_ST.zw;
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, uv_Bump ) );
			o.Normal = finalNormals12;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode17 = tex2D( _Mask, uv_Mask );
			float lerpResult33 = lerp( 1.0 , tex2DNode17.g , _Occlusion);
			float MaskOcclusion25 = lerpResult33;
			float4 lerpResult28 = lerp( tex2DNode1 , float4( 1,1,1,0 ) , MaskOcclusion25);
			float4 finalColor19 = ( ( ( tex2DNode1 * lerpResult28 ) * _Color ) * i.vertexColor );
			float4 lerpResult35 = lerp( finalColor19 , _FlashColor , _FlashColor.a);
			o.Albedo = lerpResult35.rgb;
			float4 temp_output_34_0 = _FlashColor;
			o.Emission = temp_output_34_0.rgb;
			o.Specular = _SpecularGloss;
			float MaskSmoothness26 = tex2DNode17.b;
			o.Gloss = ( _SpecularPower * MaskSmoothness26 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;2;-2036.514,-1719.635;Inherit;False;1095.825;650.78;Color;9;28;1;29;27;3;4;8;7;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1576.548,-843.0609;Inherit;False;668.2742;342.2659;Normals;2;12;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1156.59,-732.068;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1085.968,-1411.487;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1313.247,-1513.903;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;8;-1251.352,-1325.419;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1462.744,-1645.074;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-1497.934,-1298.221;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2027.932,-1660.821;Inherit;True;Property;_MainTex;Diffuse;1;0;Create;False;0;0;0;False;0;False;-1;None;04fd8409b0d820c4bbf250e52e9a002e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1118.088,-1658.708;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;17;-1790.574,-332.2358;Inherit;True;Property;_Mask;Mask;3;0;Create;True;0;0;0;False;0;False;-1;None;25be82caebe5a3a4da70e8dccb2ddd72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;28;-1700.069,-1555.668;Inherit;True;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-1980.149,-1397.849;Inherit;False;25;MaskOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1413.816,-163.0419;Inherit;False;MaskSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1251.816,-334.0419;Inherit;False;MaskOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1672.067,-442.3097;Inherit;False;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;0;False;0;False;1;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-1404.067,-345.3097;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-1515,-773.4719;Inherit;True;Property;_Bump;NormalMap;2;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;4409d80605393944ab1a732ebc3e67e5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;214.879,-414.3776;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;AlonWoof/Dolphin/AmyBody;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;4;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-572.5256,-120.4235;Inherit;False;26;MaskSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-281.5256,-198.4235;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-613.715,-207.2383;Inherit;False;Property;_SpecularPower;Specular Power;6;0;Create;True;0;0;0;False;0;False;0;0.218;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-610.715,-286.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;5;0;Create;True;0;0;0;False;0;False;0.05;0.295;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-503.2879,-606.9162;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;6;-489.4059,-743.2475;Inherit;False;19;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;34;-628.3934,-506.5031;Inherit;False;Property;_FlashColor;FlashColor;8;0;Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;-114.3929,-699.5031;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
WireConnection;12;0;10;0
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;4;0;27;0
WireConnection;4;1;3;0
WireConnection;27;0;1;0
WireConnection;27;1;28;0
WireConnection;19;0;7;0
WireConnection;28;0;1;0
WireConnection;28;2;29;0
WireConnection;26;0;17;3
WireConnection;25;0;33;0
WireConnection;33;1;17;2
WireConnection;33;2;32;0
WireConnection;0;0;35;0
WireConnection;0;1;13;0
WireConnection;0;2;34;0
WireConnection;0;3;15;0
WireConnection;0;4;30;0
WireConnection;30;0;16;0
WireConnection;30;1;31;0
WireConnection;35;0;6;0
WireConnection;35;1;34;0
WireConnection;35;2;34;4
ASEEND*/
//CHKSM=5696789AB55BB85631B2DA30B80E51AB8ACE5339