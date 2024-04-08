// Upgrade NOTE: upgraded instancing buffer 'AlonWoofDolphinEye' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/Dolphin/Eye"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Diffuse", 2D) = "white" {}
		_ShineTex("ShineTex", 2D) = "black" {}
		[Normal]_Bump("NormalMap", 2D) = "bump" {}
		_SpecularGloss("Specular Gloss", Range( 0.05 , 1)) = 0.05
		_SpecularPower("Specular Power", Range( 0 , 1)) = 0
		_LookY("Look_Y", Range( -1 , 1)) = 0
		_LookX("Look_X", Range( -1 , 1)) = 0
		_FlashColor("FlashColor", Color) = (0,0,0,0)
		_EyeJitter("EyeJitter", Range( 0 , 0.01)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Bump;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _FlashColor;
		uniform sampler2D _ShineTex;
		uniform float _SpecularGloss;
		uniform float _SpecularPower;

		UNITY_INSTANCING_BUFFER_START(AlonWoofDolphinEye)
			UNITY_DEFINE_INSTANCED_PROP(float, _LookX)
#define _LookX_arr AlonWoofDolphinEye
			UNITY_DEFINE_INSTANCED_PROP(float, _LookY)
#define _LookY_arr AlonWoofDolphinEye
			UNITY_DEFINE_INSTANCED_PROP(float, _EyeJitter)
#define _EyeJitter_arr AlonWoofDolphinEye
		UNITY_INSTANCING_BUFFER_END(AlonWoofDolphinEye)

		void surf( Input i , inout SurfaceOutput o )
		{
			float _LookX_Instance = UNITY_ACCESS_INSTANCED_PROP(_LookX_arr, _LookX);
			float _LookY_Instance = UNITY_ACCESS_INSTANCED_PROP(_LookY_arr, _LookY);
			float4 appendResult49 = (float4(_LookX_Instance , _LookY_Instance , 0.0 , 0.0));
			float2 uv_TexCoord50 = i.uv_texcoord + appendResult49.xy;
			float2 temp_cast_2 = (_Time.y).xx;
			float dotResult4_g2 = dot( temp_cast_2 , float2( 12.9898,78.233 ) );
			float lerpResult10_g2 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g2 ) * 43758.55 ) ));
			float mulTime73 = _Time.y * 0.85;
			float2 temp_cast_3 = (mulTime73).xx;
			float dotResult4_g3 = dot( temp_cast_3 , float2( 12.9898,78.233 ) );
			float lerpResult10_g3 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
			float4 appendResult82 = (float4(lerpResult10_g2 , lerpResult10_g3 , 0.0 , 0.0));
			float _EyeJitter_Instance = UNITY_ACCESS_INSTANCED_PROP(_EyeJitter_arr, _EyeJitter);
			float4 eyeOffsetUV51 = ( float4( uv_TexCoord50, 0.0 , 0.0 ) + ( appendResult82 * _EyeJitter_Instance ) );
			float3 finalNormals12 = UnpackNormal( tex2D( _Bump, eyeOffsetUV51.xy ) );
			o.Normal = finalNormals12;
			float4 tex2DNode1 = tex2D( _MainTex, eyeOffsetUV51.xy );
			float4 lerpResult41 = lerp( _Color , tex2DNode1 , tex2DNode1.a);
			float4 finalColor19 = ( lerpResult41 * i.vertexColor );
			float4 lerpResult67 = lerp( finalColor19 , _FlashColor , _FlashColor.a);
			o.Albedo = lerpResult67.rgb;
			float2 uv_TexCoord53 = i.uv_texcoord + ( appendResult49 * float4( 0.85,0.85,0,0 ) ).xy;
			float4 shineOffsetUV54 = ( float4( uv_TexCoord53, 0.0 , 0.0 ) + ( appendResult82 * float4( 0.003,0.003,0,0 ) ) );
			float4 finalEmissive56 = ( ( finalColor19 * float4( 0.05,0.05,0.05,1 ) ) + tex2D( _ShineTex, shineOffsetUV54.xy ) );
			float4 lerpResult66 = lerp( finalEmissive56 , _FlashColor , _FlashColor.a);
			o.Emission = lerpResult66.rgb;
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
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;44;-1924.405,-437.2724;Inherit;False;793.3723;474.981;Comment;5;43;56;59;60;58;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2;-2131.488,-1741.796;Inherit;False;1095.825;650.78;Color;7;1;3;8;7;19;41;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1754.548,-859.0609;Inherit;False;668.2742;342.2659;Normals;3;12;10;63;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1334.59,-748.068;Inherit;False;finalNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;-2002.906,-1629.982;Inherit;True;Property;_MainTex;Diffuse;2;0;Create;False;0;0;0;False;0;False;-1;None;2d151b45b53bc0b4398ad1f536ce878f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;41;-1560.401,-1557.636;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-1911.908,-1419.382;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;8;-1618.327,-1406.58;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1291.063,-1275.869;Inherit;False;finalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1388.943,-1485.648;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;46;-2304.508,-2654.485;Inherit;False;1178.883;503.3871;EyeCoords;10;54;53;52;51;50;49;48;47;72;85;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-1920.207,-385.5415;Inherit;False;54;shineOffsetUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1603.163,-365.8717;Inherit;False;19;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1305.163,-224.8717;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;43;-1738.888,-258.8568;Inherit;True;Property;_ShineTex;ShineTex;3;0;Create;True;0;0;0;False;0;False;-1;None;601df28eb6560724ebcb1d3f4e11ad77;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-1430.163,-370.8717;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.05,0.05,0.05,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;10;-1692,-789.4719;Inherit;True;Property;_Bump;NormalMap;4;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;17d8f9e21ef51c1468e108cd5e6f1064;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;61;-582.0432,52.30521;Inherit;False;Half Lambert Term;-1;;1;86299dc21373a954aa5772333626c9c1;0;1;3;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-555.2217,-438.0888;Inherit;False;12;finalNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-2127.254,-1380.355;Inherit;False;51;eyeOffsetUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-538.715,-85.23831;Inherit;False;Property;_SpecularPower;Specular Power;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-552.715,-205.2383;Inherit;False;Property;_SpecularGloss;Specular Gloss;5;0;Create;True;0;0;0;False;0;False;0.05;0.05;0.05;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-762.2065,-389.5415;Inherit;False;56;finalEmissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;6;-582.4059,-599.2475;Inherit;False;19;finalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;67;-357.306,-554.1653;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;66;-404.3059,-337.1653;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;64;-944.3059,-214.1653;Inherit;False;Property;_FlashColor;FlashColor;9;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1372.207,-58.5415;Inherit;False;finalEmissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-1692.645,-608.1062;Inherit;False;51;eyeOffsetUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2273.344,-2582.934;Inherit;False;InstancedProperty;_LookX;Look_X;8;0;Create;False;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2275.518,-2504.595;Inherit;False;InstancedProperty;_LookY;Look_Y;7;0;Create;False;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;49;-2144.096,-2360.416;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1935.046,-2375.51;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.85,0.85,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-1961.867,-2586.293;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1319.754,-2591.116;Inherit;False;eyeOffsetUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-1299.758,-2338.513;Inherit;False;shineOffsetUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-1678.277,-2346.021;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;69;-2627.303,-2152.191;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;73;-2623.103,-2056.426;Inherit;False;1;0;FLOAT;0.85;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-2138.646,-2138.028;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-1688.803,-2589.191;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;-1440.546,-2294.032;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1980.045,-2158.829;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;75;-2381.803,-2048.324;Inherit;False;Random Range;-1;;3;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;68;-2379.803,-2168.492;Inherit;False;Random Range;-1;;2;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1981.344,-2041.83;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.003,0.003,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-4.121002,-418.3776;Float;False;True;-1;2;ASEMaterialInspector;0;0;BlinnPhong;AlonWoof/Dolphin/Eye;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;0;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-2665.304,-1852.991;Inherit;False;InstancedProperty;_EyeJitter;EyeJitter;10;0;Create;True;0;0;0;False;0;False;0;0;0;0.01;0;1;FLOAT;0
WireConnection;12;0;10;0
WireConnection;1;1;42;0
WireConnection;41;0;3;0
WireConnection;41;1;1;0
WireConnection;41;2;1;4
WireConnection;19;0;7;0
WireConnection;7;0;41;0
WireConnection;7;1;8;0
WireConnection;60;0;59;0
WireConnection;60;1;43;0
WireConnection;43;1;55;0
WireConnection;59;0;58;0
WireConnection;10;1;63;0
WireConnection;67;0;6;0
WireConnection;67;1;64;0
WireConnection;67;2;64;4
WireConnection;66;0;57;0
WireConnection;66;1;64;0
WireConnection;66;2;64;4
WireConnection;56;0;60;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;52;0;49;0
WireConnection;50;1;49;0
WireConnection;51;0;72;0
WireConnection;54;0;85;0
WireConnection;53;1;52;0
WireConnection;82;0;68;0
WireConnection;82;1;75;0
WireConnection;72;0;50;0
WireConnection;72;1;83;0
WireConnection;85;0;53;0
WireConnection;85;1;84;0
WireConnection;83;0;82;0
WireConnection;83;1;70;0
WireConnection;75;1;73;0
WireConnection;68;1;69;0
WireConnection;84;0;82;0
WireConnection;0;0;67;0
WireConnection;0;1;13;0
WireConnection;0;2;66;0
WireConnection;0;3;15;0
WireConnection;0;4;16;0
ASEEND*/
//CHKSM=4F1E714F1F4BAB84849641F3D62500ABF268FD33