// Upgrade NOTE: upgraded instancing buffer 'AlonWoofAmyAmyBody' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Amy/AmyBody"
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
		_Wetness("Wetness", Range( 0 , 1)) = 0
		_Dirtiness("Dirtiness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Bump;
		uniform sampler2D _MainTex;
		uniform sampler2D _Mask;
		uniform float _Occlusion;
		uniform float4 _Color;
		uniform float4 _FlashColor;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		UNITY_INSTANCING_BUFFER_START(AlonWoofAmyAmyBody)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Bump_ST)
#define _Bump_ST_arr AlonWoofAmyAmyBody
			UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define _MainTex_ST_arr AlonWoofAmyAmyBody
			UNITY_DEFINE_INSTANCED_PROP(float4, _Mask_ST)
#define _Mask_ST_arr AlonWoofAmyAmyBody
			UNITY_DEFINE_INSTANCED_PROP(float, _Dirtiness)
#define _Dirtiness_arr AlonWoofAmyAmyBody
			UNITY_DEFINE_INSTANCED_PROP(float, _Wetness)
#define _Wetness_arr AlonWoofAmyAmyBody
		UNITY_INSTANCING_BUFFER_END(AlonWoofAmyAmyBody)

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _Bump_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Bump_ST_arr, _Bump_ST);
			float2 uv_Bump = i.uv_texcoord * _Bump_ST_Instance.xy + _Bump_ST_Instance.zw;
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, uv_Bump ) );
			o.Normal = finalNormals12;
			float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 _Mask_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Mask_ST_arr, _Mask_ST);
			float2 uv_Mask = i.uv_texcoord * _Mask_ST_Instance.xy + _Mask_ST_Instance.zw;
			float4 tex2DNode17 = tex2D( _Mask, uv_Mask );
			float lerpResult33 = lerp( 1.0 , tex2DNode17.g , _Occlusion);
			float MaskOcclusion25 = lerpResult33;
			float4 lerpResult28 = lerp( tex2DNode1 , float4( 1,1,1,0 ) , MaskOcclusion25);
			float4 baseColor19 = ( ( ( tex2DNode1 * lerpResult28 ) * _Color ) * i.vertexColor );
			float baseDirt42 = tex2DNode17.a;
			float lerpResult49 = lerp( 0.0 , 1.0 , baseDirt42);
			float _Dirtiness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dirtiness_arr, _Dirtiness);
			float4 lerpResult51 = lerp( baseColor19 , ( baseColor19 * lerpResult49 ) , _Dirtiness_Instance);
			float4 finalColor46 = lerpResult51;
			float4 lerpResult35 = lerp( finalColor46 , _FlashColor , _FlashColor.a);
			o.Albedo = lerpResult35.rgb;
			float4 temp_output_34_0 = _FlashColor;
			o.Emission = temp_output_34_0.rgb;
			float _Wetness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Wetness_arr, _Wetness);
			float lerpResult38 = lerp( _SpecularGloss , 0.15 , _Wetness_Instance);
			o.Specular = lerpResult38;
			float MaskSmoothness26 = tex2DNode17.b;
			float lerpResult37 = lerp( ( _SpecularPower * MaskSmoothness26 ) , 0.5 , _Wetness_Instance);
			o.Gloss = lerpResult37;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;43;-558.7756,-1687.139;Inherit;False;924.6267;576.0123;DirtMask;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2;-2036.514,-1719.635;Inherit;False;1095.825;650.78;Color;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1576.548,-843.0609;Inherit;False;668.2742;342.2659;Normals;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1156.59,-732.068;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1313.247,-1513.903;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;8;-1251.352,-1325.419;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1462.744,-1645.074;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-1497.934,-1298.221;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2027.932,-1660.821;Inherit;True;Property;_MainTex;Diffuse;1;0;Create;False;0;0;0;False;0;False;-1;None;04fd8409b0d820c4bbf250e52e9a002e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-1790.574,-332.2358;Inherit;True;Property;_Mask;Mask;3;0;Create;True;0;0;0;False;0;False;-1;None;25be82caebe5a3a4da70e8dccb2ddd72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;28;-1700.069,-1555.668;Inherit;True;3;0;COLOR;1,1,1,1;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-1980.149,-1397.849;Inherit;False;25;MaskOcclusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1413.816,-163.0419;Inherit;False;MaskSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1672.067,-442.3097;Inherit;False;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;0;False;0;False;1;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-1404.067,-345.3097;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-1515,-773.4719;Inherit;True;Property;_Bump;NormalMap;2;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;4409d80605393944ab1a732ebc3e67e5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-628.3934,-506.5031;Inherit;False;Property;_FlashColor;FlashColor;8;0;Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-742.715,-199.2383;Inherit;False;Property;_SpecularPower;Specular Power;6;0;Create;True;0;0;0;False;0;False;0;0.218;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-705.5256,-102.4235;Inherit;False;26;MaskSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1224.816,-356.0419;Inherit;False;MaskOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-755.715,-312.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;5;0;Create;True;0;0;0;False;0;False;0.05;0.295;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-435.5256,-156.4235;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-141.262,-307.335;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.15;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;-146.262,-161.335;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-758.262,74.66498;Inherit;False;InstancedProperty;_Wetness;Wetness;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;410.879,-365.3776;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;AlonWoof/Amy/AmyBody;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;4;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LerpOp;35;-262.3929,-698.5031;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-508.8418,-1587.192;Inherit;False;InstancedProperty;_Dirtiness;Dirtiness;10;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1085.968,-1411.487;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;6;-489.4059,-743.2475;Inherit;False;46;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-72.2879,-453.9162;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1120.608,-1658.708;Inherit;False;baseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-1480.502,-62.05324;Inherit;False;baseDirt;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;163.9957,-1636.385;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-498.0772,-1308.818;Inherit;False;42;baseDirt;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-492.3873,-1505.709;Inherit;False;19;baseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-143.9441,-1442.055;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;49;-313.2198,-1357.875;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;51;22.15167,-1455.608;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-223.8483,-1575.608;Inherit;False;19;baseColor;1;0;OBJECT;;False;1;COLOR;0
WireConnection;12;0;10;0
WireConnection;4;0;27;0
WireConnection;4;1;3;0
WireConnection;27;0;1;0
WireConnection;27;1;28;0
WireConnection;28;0;1;0
WireConnection;28;2;29;0
WireConnection;26;0;17;3
WireConnection;33;1;17;2
WireConnection;33;2;32;0
WireConnection;25;0;33;0
WireConnection;30;0;16;0
WireConnection;30;1;31;0
WireConnection;38;0;15;0
WireConnection;38;2;36;0
WireConnection;37;0;30;0
WireConnection;37;2;36;0
WireConnection;0;0;35;0
WireConnection;0;1;13;0
WireConnection;0;2;34;0
WireConnection;0;3;38;0
WireConnection;0;4;37;0
WireConnection;35;0;6;0
WireConnection;35;1;34;0
WireConnection;35;2;34;4
WireConnection;7;0;4;0
WireConnection;7;1;8;0
WireConnection;19;0;7;0
WireConnection;42;0;17;4
WireConnection;46;0;51;0
WireConnection;47;0;45;0
WireConnection;47;1;49;0
WireConnection;49;2;44;0
WireConnection;51;0;50;0
WireConnection;51;1;47;0
WireConnection;51;2;39;0
ASEEND*/
//CHKSM=2C508FD9BF06E20594A849062066BE3EFEAD09E4