// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Dolphin/YumeBlend"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Color1("Color", Color) = (1,1,1,1)
		_MainTex("Diffuse", 2D) = "white" {}
		_MainTex1("Diffuse", 2D) = "white" {}
		[Normal]_Bump("NormalMap", 2D) = "bump" {}
		[Normal]_Bump1("NormalMap", 2D) = "bump" {}
		_Mask("Mask", 2D) = "white" {}
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_SpecularGloss("Specular Gloss", Range( 0.05 , 1)) = 0.05
		_SpecularPower("Specular Power", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", Range( 0 , 1)) = 1
		_Blend("Blend", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
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
		uniform sampler2D _Bump1;
		uniform float4 _Bump1_ST;
		uniform float _Blend;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _Occlusion;
		uniform float4 _Color;
		uniform sampler2D _MainTex1;
		uniform float4 _MainTex1_ST;
		uniform float4 _Color1;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Bump = i.uv_texcoord * _Bump_ST.xy + _Bump_ST.zw;
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, uv_Bump ) );
			float2 uv_Bump1 = i.uv_texcoord * _Bump1_ST.xy + _Bump1_ST.zw;
			float3 finalNormals245 = UnpackNormal( tex2D( _Bump1, uv_Bump1 ) );
			float3 lerpResult48 = lerp( finalNormals12 , finalNormals245 , _Blend);
			o.Normal = lerpResult48;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode17 = tex2D( _Mask, uv_Mask );
			float lerpResult33 = lerp( 1.0 , tex2DNode17.g , _Occlusion);
			float MaskOcclusion25 = lerpResult33;
			float4 lerpResult28 = lerp( tex2DNode1 , float4( 1,1,1,0 ) , MaskOcclusion25);
			float4 finalColor19 = ( ( ( tex2DNode1 * lerpResult28 ) * _Color ) * i.vertexColor );
			float2 uv_MainTex1 = i.uv_texcoord * _MainTex1_ST.xy + _MainTex1_ST.zw;
			float4 tex2DNode40 = tex2D( _MainTex1, uv_MainTex1 );
			float4 lerpResult42 = lerp( tex2DNode40 , float4( 1,1,1,0 ) , MaskOcclusion25);
			float4 finalColor241 = ( ( ( tex2DNode40 * lerpResult42 ) * _Color1 ) * i.vertexColor );
			float4 lerpResult50 = lerp( finalColor19 , finalColor241 , _Blend);
			o.Albedo = lerpResult50.rgb;
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
Node;AmplifyShaderEditor.SamplerNode;17;-1790.574,-332.2358;Inherit;True;Property;_Mask;Mask;6;0;Create;True;0;0;0;False;0;False;-1;None;25be82caebe5a3a4da70e8dccb2ddd72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-588.715,-179.2383;Inherit;False;Property;_SpecularPower;Specular Power;9;0;Create;True;0;0;0;False;0;False;0;0.218;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-295.5256,-127.4235;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-538.5256,-94.42346;Inherit;False;26;MaskSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1251.816,-334.0419;Inherit;False;MaskOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1672.067,-442.3097;Inherit;False;Property;_Occlusion;Occlusion;10;0;Create;True;0;0;0;False;0;False;1;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-1404.067,-345.3097;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1184.712,-243.7626;Inherit;False;MaskSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-623.715,-317.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;8;0;Create;True;0;0;0;False;0;False;0.05;0.295;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1085.968,-1411.487;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1313.247,-1513.903;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;8;-1251.352,-1325.419;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1462.744,-1645.074;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-1497.934,-1298.221;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2027.932,-1660.821;Inherit;True;Property;_MainTex;Diffuse;2;0;Create;False;0;0;0;False;0;False;-1;None;04fd8409b0d820c4bbf250e52e9a002e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;28;-1700.069,-1555.668;Inherit;True;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-1980.149,-1397.849;Inherit;False;25;MaskOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;34;-2032.827,-2694.518;Inherit;False;1095.825;650.78;Color;9;43;42;41;40;39;38;37;36;35;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1082.281,-2386.37;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1309.56,-2488.786;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;37;-1247.665,-2300.302;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1459.057,-2619.957;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;-1494.247,-2273.104;Inherit;False;Property;_Color1;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;-2024.245,-2635.704;Inherit;True;Property;_MainTex1;Diffuse;3;0;Create;False;0;0;0;False;0;False;-1;None;04fd8409b0d820c4bbf250e52e9a002e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;42;-1696.382,-2530.551;Inherit;True;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-1976.462,-2372.732;Inherit;False;25;MaskOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-123.2215,-366.3776;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;AlonWoof/Dolphin/YumeBlend;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;7;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-1114.401,-2634.591;Inherit;False;finalColor2;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1173.075,-1600.769;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1156.59,-732.068;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;10;-1515,-773.4719;Inherit;True;Property;_Bump;NormalMap;4;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;4409d80605393944ab1a732ebc3e67e5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;44;-1633.151,71.01957;Inherit;False;668.2742;342.2659;Normals;2;46;45;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;46;-1571.603,140.6086;Inherit;True;Property;_Bump1;NormalMap;5;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;4409d80605393944ab1a732ebc3e67e5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1213.193,182.0125;Inherit;False;finalNormals2;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-892.6837,-515.9284;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1014.363,-320.1401;Inherit;False;Property;_Blend;Blend;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-607.1978,-481.4646;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;50;-615.6343,-599.7411;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-894.6343,-587.7411;Inherit;False;41;finalColor2;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;6;-897.1985,-665.4017;Inherit;False;19;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-888.0921,-438.3035;Inherit;False;45;finalNormals2;1;0;OBJECT;;False;1;FLOAT3;0
WireConnection;30;0;16;0
WireConnection;30;1;31;0
WireConnection;25;0;33;0
WireConnection;33;1;17;2
WireConnection;33;2;32;0
WireConnection;26;0;17;3
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;4;0;27;0
WireConnection;4;1;3;0
WireConnection;27;0;1;0
WireConnection;27;1;28;0
WireConnection;28;0;1;0
WireConnection;28;2;29;0
WireConnection;35;0;36;0
WireConnection;35;1;37;0
WireConnection;36;0;38;0
WireConnection;36;1;39;0
WireConnection;38;0;40;0
WireConnection;38;1;42;0
WireConnection;42;0;40;0
WireConnection;42;2;43;0
WireConnection;0;0;50;0
WireConnection;0;1;48;0
WireConnection;0;3;15;0
WireConnection;0;4;30;0
WireConnection;41;0;35;0
WireConnection;19;0;7;0
WireConnection;12;0;10;0
WireConnection;45;0;46;0
WireConnection;48;0;13;0
WireConnection;48;1;47;0
WireConnection;48;2;49;0
WireConnection;50;0;6;0
WireConnection;50;1;51;0
WireConnection;50;2;49;0
ASEEND*/
//CHKSM=806A71B565E0C6918BCFF03D9243E146E8AAA7DF