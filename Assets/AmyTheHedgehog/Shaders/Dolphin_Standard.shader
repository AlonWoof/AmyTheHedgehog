// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Dolphin/Standard"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Diffuse("Diffuse", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Bump("NormalMap", 2D) = "bump" {}
		_SpecularGloss("Specular Gloss", Range( 0.05 , 1)) = 0.05
		_SpecularPower("Specular Power", Range( 0 , 1)) = 0
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
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _Color;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Bump = i.uv_texcoord * _Bump_ST.xy + _Bump_ST.zw;
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, uv_Bump ) );
			o.Normal = finalNormals12;
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 finalColor5 = ( ( tex2D( _Diffuse, uv_Diffuse ) * _Color ) * i.vertexColor );
			o.Albedo = finalColor5.rgb;
			o.Specular = _SpecularGloss;
			o.Gloss = _SpecularPower;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
1135;73;784;936;794.3894;680.8417;1.094251;True;False
Node;AmplifyShaderEditor.CommentaryNode;2;-1574.089,-1401.204;Inherit;False;695.9481;476.48;Color;6;5;4;3;1;7;8;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-1520.335,-1335.565;Inherit;True;Property;_Diffuse;Diffuse;1;0;Create;True;0;0;0;False;0;False;-1;None;e6ab3c5060e9e0e41947d6352d520e9d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-1493.063,-1157.09;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;9;-1576.548,-843.0609;Inherit;False;668.2742;342.2659;Normals;2;12;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1220.348,-1231.644;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;8;-1295.331,-1105.396;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1101.129,-1109.015;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;10;-1515,-773.4719;Inherit;True;Property;_Bump;NormalMap;3;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-1084.917,-1347.743;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1156.59,-732.068;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-390.715,-168.2383;Inherit;False;Property;_SpecularPower;Specular Power;5;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-378.715,-254.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;4;0;Create;True;0;0;0;False;0;False;0.05;0.05;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-309.2879,-373.9162;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;6;-303.311,-447.2095;Inherit;False;5;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-52.12098,-375.3776;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;AlonWoof/Dolphin/Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;3;0
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;5;0;7;0
WireConnection;12;0;10;0
WireConnection;0;0;6;0
WireConnection;0;1;13;0
WireConnection;0;3;15;0
WireConnection;0;4;16;0
ASEEND*/
//CHKSM=9401E4AB317D6958C7FAC7A65907B9C898B37096