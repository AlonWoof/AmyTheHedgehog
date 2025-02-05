// Upgrade NOTE: upgraded instancing buffer 'AlonWoofUIScrollBG' to new syntax.

// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AlonWoof/UI/ScrollBG"
{
	Properties
	{
		_ScollTex("ScollTex", 2D) = "white" {}
		_Tile("Tile", Vector) = (8,8,0,0)
		_ScrollSpeed("ScrollSpeed", Vector) = (-0.2,-0.2,0,0)
		_AspectRatio("AspectRatio", Float) = 1.777778
		_fgColor("fgColor", Color) = (0,0,0,0)
		_bgColor("bgColor", Color) = (1,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
		};

		uniform float4 _fgColor;
		uniform float4 _bgColor;
		uniform sampler2D _ScollTex;
		uniform float2 _ScrollSpeed;
		uniform float2 _Tile;

		UNITY_INSTANCING_BUFFER_START(AlonWoofUIScrollBG)
			UNITY_DEFINE_INSTANCED_PROP(float, _AspectRatio)
#define _AspectRatio_arr AlonWoofUIScrollBG
		UNITY_INSTANCING_BUFFER_END(AlonWoofUIScrollBG)


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float _AspectRatio_Instance = UNITY_ACCESS_INSTANCED_PROP(_AspectRatio_arr, _AspectRatio);
			float4 appendResult10 = (float4(( _Tile.x * _AspectRatio_Instance ) , _Tile.y , 0.0 , 0.0));
			float2 panner3 = ( 1.0 * _Time.y * _ScrollSpeed + ( ase_screenPosNorm * appendResult10 ).xy);
			float4 scrollTexMask14 = tex2D( _ScollTex, panner3 );
			float4 lerpResult17 = lerp( _fgColor , _bgColor , scrollTexMask14);
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 lerpResult24 = lerp( lerpResult17 , _bgColor , ase_grabScreenPosNorm.g);
			o.Emission = lerpResult24.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;15;-2195.08,572.4317;Inherit;False;833.2338;543.8668;Gradient;6;25;24;17;22;19;18;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;-2191.638,-144.6101;Inherit;False;1165.214;645.9643;ScrollPattern;10;2;12;9;10;8;3;7;5;11;14;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1873.478,146.2278;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.7777;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;7;-2061.199,148.1745;Inherit;False;Property;_Tile;Tile;2;0;Create;True;0;0;0;False;0;False;8,8;8,8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScreenPosInputsNode;5;-2074.668,-72.8057;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-2079.873,285.7091;Inherit;False;InstancedProperty;_AspectRatio;AspectRatio;4;0;Create;True;0;0;0;False;0;False;1.777778;1.777778;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1733.004,223.7886;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1687.712,-46.76904;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;3;-1489.805,-80.6154;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;11;-1710.704,29.89063;Inherit;False;Property;_ScrollSpeed;ScrollSpeed;3;0;Create;True;0;0;0;False;0;False;-0.2,-0.2;-0.2,-0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;2;-1443.208,49.02811;Inherit;True;Property;_ScollTex;ScollTex;1;0;Create;True;0;0;0;False;0;False;-1;None;44d3e499dd4452549bbcf196d0371179;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1260.454,390.7533;Inherit;False;scrollTexMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-2164.587,811.6926;Inherit;False;Property;_bgColor;bgColor;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5038269,0.9622641,0.5327808,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-2168.167,624.3373;Inherit;False;Property;_fgColor;fgColor;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.5801886,1,0.6161726,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;17;-1785.034,678.0282;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-2156.624,994.2941;Inherit;False;14;scrollTexMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;24;-1557.45,813.3039;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GrabScreenPosition;25;-1872.562,873.4034;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;4;-1192.496,680.5978;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;AlonWoof/UI/ScrollBG;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;1;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;Transparent;;Overlay;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;7;1
WireConnection;9;1;12;0
WireConnection;10;0;9;0
WireConnection;10;1;7;2
WireConnection;8;0;5;0
WireConnection;8;1;10;0
WireConnection;3;0;8;0
WireConnection;3;2;11;0
WireConnection;2;1;3;0
WireConnection;14;0;2;0
WireConnection;17;0;19;0
WireConnection;17;1;18;0
WireConnection;17;2;22;0
WireConnection;24;0;17;0
WireConnection;24;1;18;0
WireConnection;24;2;25;2
WireConnection;4;2;24;0
ASEEND*/
//CHKSM=29C3FBD44E91C32F728CFBBABB064F7C61EA671D