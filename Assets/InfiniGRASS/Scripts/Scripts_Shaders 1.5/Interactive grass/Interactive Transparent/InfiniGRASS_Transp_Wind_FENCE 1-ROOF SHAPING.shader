// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

Shader "InfiniGRASS/InfiniGrass Directional Wind ROOF SHAPING" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        
        _BulgeScale ("Bulge Scale", Float ) = 0.2
        _BulgeShape ("Bulge Shape", Float ) = 5
        _BulgeScale_copy ("Bulge Scale_copy", Float ) = 1.2 // control turbulence
                               
        _WaveControl1("Waves", Vector) = (1, 0.01, 0.001, 0.41) // _WaveControl1.w controls interaction power
        _TimeControl1("Time", Vector) = (1, 1, 1, 100)
        _OceanCenter("Ocean Center", Vector) = (0, 0, 0, 0)
        
         _RandYScale ("Vary Height Ammount", Float ) = 1
         _RippleScale ("Vary Height", Float ) = 0     
        
           //INFINIGRASS - hero position for fading and lowering dynamics to reduce jitter while interacting  
           
           _InteractPos("Interact Position", Vector) = (0, 0, 0) //for lowering motion when interaction item is near
            _InteractSpeed("Interact Speed", Vector) = (0, 0, 0) //v1.5
           _FadeThreshold ("Fade out Threshold", Float ) = 100
           _StopMotionThreshold ("Stop motion Threshold", Float ) = 10
           
           _Color ("Grass tint", Color) = (0.5,0.8,0.5,0) //0.5,0.8,0.5
           _ColorGlobal ("Global tint", Color) = (0.5,0.5,0.5,0) //0.5,0.8,0.5
           _TintPower("tint power", Float) = 0
            _TintFrequency("tint frequency", Float) = 0.1
           _SpecularPower("Specular", Float) = 1
           
           _SmoothMotionFactor("Smooth wave motion", Float) = 105
           _WaveXFactor("Wave Control x axis", Float) = 1
           _WaveYFactor("Wave Control y axis", Float) = 1
           
           //SNOW
           _SnowTexture ("Snow texture", 2D) = "white" {}

           //ROOF version of FENCE shader - control local vs global wind
           //_TimeControl1.y for global, _TimeControl1.z for local
           _BaseLight("Grass Base light control", Float) = 0
           _BaseColorYShift("Shift Base color Y axis", Float) = 1

           //v2.0.6
           _InteractMaxYoffset("Interaction max offset in Y axis", Float) = 1.5

		   //v1.9.9.2
		   //float shadowBrightnessCutoff;// = 0.1;
		   //float shadowBrightness;// = 1;
		   shadowBrightnessCutoff("shadow Brightness Cutoff (0 to 0.25)", Float) = 0 //disable the shadow brighting by default, use 0.1 for best result
		   shadowBrightness("shadow Brightness", Float) = 1

			   //v2.0.8
		   _InteractTexture("_Interact Texture", 2D) = "white" {}
		   _InteractTexturePos("Interact Texture Pos", Vector) = (0 ,5000, 0, 0)
			   //v2.1.1
			_shapeOnlyHeight("Shape Only Height",Float) = 0
			   //v2.1.12
		   _XTiles("X tiles",Float) = 1
		   _YTiles("Y tiles",Float) = 1

			_erasedShadowFactor("Shape Erased Grass Shadow Factor",Float) = 1

			   //v2.1.13
			   scaleYFactor("scale Y Factor",Vector) = (1 ,0, 1, 1)

			   //v1.7 - Local interact radial - amplitude - frequency v2.1.14
		   _InteractAmpFreqRad("_InteractAmpFreqRadial", Vector) = (1, 1, 1)
				_InteractPos2("Interact Position", Vector) = (0, 0, 0) //for lowering motion when interaction item is near

    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_instancing //v1.7.8
            #pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile_fwdbase nolightmap //v4.1

           // #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            


			//v2.0.8
			sampler2D _InteractTexture;
			float3 _InteractTexturePos;
			//v2.1.1
			float _shapeOnlyHeight;
			//v2.1.12
			float _XTiles;
			float _YTiles;
			
			//v2.1.13
			float4 scaleYFactor;
			//v2.1.14
			float3 _InteractAmpFreqRad;
			float3 _InteractPos2;

            uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
            uniform fixed _Cutoff;
             uniform float _RandYScale;
            uniform float _RippleScale;
            //float3 _CameraPos;
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            
            float3 _Color;
            float3 _ColorGlobal;
           	float _TintPower; 
           	float _SpecularPower;
           	float _SmoothMotionFactor;
           	float _WaveXFactor;
           	float _WaveYFactor;
           	
           	sampler2D _SnowTexture;
           	float _SnowCoverage; 	
           	float _TintFrequency;

           	 float3 _InteractSpeed;
           	 float _InteractMaxYoffset;

			 //v1.9.9.2
			 float shadowBrightnessCutoff;// = 0.1;
			 float shadowBrightness;// = 1;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;
           //     v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                

				

					//v2.0.8
				half2 tileableUv = mul(unity_ObjectToWorld, (v.vertex)).xz;
				float WorldScale = _InteractTexturePos.y;
				float3 CamPos = float3(0, 0, 0);//_DepthCameraPos;//_WorldSpaceCameraPos;
				float3 Origin = float3(_InteractTexturePos.x,  _InteractTexturePos.z, 0);//float2(CamPos.x - WorldScale/2.0 , CamPos.z - WorldScale/2.0);
				float2 UnscaledTexPoint = float2(tileableUv.x - Origin.x, tileableUv.y - Origin.y);
				float2 ScaledTexPoint = float2(UnscaledTexPoint.x / WorldScale, UnscaledTexPoint.y / WorldScale);
				float4 texInteract = tex2Dlod(_InteractTexture, float4(ScaledTexPoint, 0.0, 0.0));



                
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                            
                       
                       //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);
                                
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
              // o.posWorld =  v.vertex;




				//v.vertex = o.posWorld;
				//v2.0.8
				if (_shapeOnlyHeight == 0) { //v2.1.1
					o.posWorld.y = o.posWorld.y*clamp((1 - texInteract.r*texInteract.r + 0.001)*1.5, 0, 1);
					o.posWorld.y = o.posWorld.y - texInteract.r * 2; //v2.1.14
					if (o.uv0.y > 0.01) {
						//o.posWorld.y = o.posWorld.y - texInteract.b * 2;

						if (texInteract.g >= 0.75) {
							o.posWorld.x = o.posWorld.x - (1 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							o.posWorld.z = o.posWorld.z - (texInteract.g - 0.75)*o.uv0.y * 42 * texInteract.b;
						}
						else if (texInteract.g >= 0.5) {	//negative angle region					
							o.posWorld.x = o.posWorld.x - (texInteract.g - 0.5)*o.uv0.y * 42 * texInteract.b;
							o.posWorld.z = o.posWorld.z + (0.75 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						}
						else if (texInteract.g >= 0.25) {
							o.posWorld.x = o.posWorld.x + (0.5 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						}
						else if (texInteract.g > 0) {
							o.posWorld.x = o.posWorld.x + texInteract.g*o.uv0.y * 42 * texInteract.b;
							o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						}
					}

					//v2.1.16 - _grassNormal
					//v.vertex.y = v.vertex.y*_respectHeight+ (pow(255-tex.r*255,_ShoreFadeFactor) - 25*pow(1+1-tex.r,1.5) + 8  + o.uv0.y*_grassHeight ); 
					//if (_textureFeed == 1) { //v2.1.20b
					//	v.vertex.y = v.vertex.y*_respectHeight + (pow(942.64 - tex.r*942.64, _ShoreFadeFactor) - 0 + o.uv0.y*_grassHeight);
					///}
					//					        if(v.vertex.y > 25*_grassHeight){
					//					       		v.vertex.y =v.vertex.y*_respectHeight + o.uv0.y*_grassHeight + 8;
					//					        }

				}
				else {
					//v.vertex.y = v.vertex.y-texInteract.b*_shapeOnlyHeight*2;
					//v.vertex.y = abs(v.vertex.y)-pow(0+texInteract.r,_shapeOnlyHeight);
					//v.vertex.y = v.vertex.y*((texInteract.r+0.011)*12.5);
					o.posWorld.y = abs(o.posWorld.y) - sign(o.posWorld.y)*(1 - texInteract.r)*_shapeOnlyHeight;

					//if (_textureFeed == 1) { //v2.1.20b
					//	v.vertex.y = v.vertex.y*_respectHeight + (pow(255 - tex.r * 255, _ShoreFadeFactor) - 25 * pow(1 + 1 - tex.r, 1.5) + 8);
					//	v.vertex.y = abs(v.vertex.y)*_grassHeight;
					//}
				}
				//o.posWorld = v.vertex;


                                                                                                  
//                if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }
//
  float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed *_WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
                	}
                	if( o.uv0.y > 0.5){
						//o.posWorld.y = o.posWorld.y - (1-distA)*3*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ; //v2.0.6
					}             
               }

				  //2.1.14
				  /////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
				  SpeedFac = float3(0, 0, 0);
				  distA = distance(_InteractPos2, o.posWorld) / (_InteractAmpFreqRad.z * 1);
				  if (distance(_InteractPos2, o.posWorld) < _InteractAmpFreqRad.z * 1) {
					  SpeedFac = 3 * (o.posWorld - _InteractPos2) *_WaveControl1.w 
						  + _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), o.posWorld - _InteractPos2) - 2.71*cross(float3(0, 1, 0), _InteractPos2 - o.posWorld)) 
						  + _InteractAmpFreqRad.x*(o.posWorld - _InteractPos2) *_WaveControl1.w *sin(o.posWorld.z + _InteractAmpFreqRad.y*_Time.y);
					  _BulgeScale = _BulgeScale * (1 / (distA + 0.01));

					  if (o.uv0.y < 0.3) {

					  }
					  if (o.uv0.y > 0.19) {
						  _WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
						  _WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;
					  }
					  if (o.uv0.y > 0.5) {
						  o.posWorld.y = o.posWorld.y - (1 - distA) * 3 * (o.uv0.y - 0.5)*sin(o.posWorld.z + _Time.y);
					  }
				  }
				  /////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////


	                if( o.uv0.y > 0.1){
	       //         	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;// unity_Scale.w;
					}
	                if( o.uv0.y >= 0.01){
	       //         	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
	                }

	                //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	            //  _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;

	                     dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}	


				//v2.1.13
				if (o.uv0.y > 0.16) {
					o.posWorld.y = o.posWorld.y * scaleYFactor.x + scaleYFactor.y;
				}

				//ADD GLOBAL ROTATION - WIND						
				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
	           // v.vertex =  o.posWorld;
                                                                                




				 //v2.1.12 - 2.0.8
				float tilesX = _XTiles;
				float tilesY = _YTiles;
				float brushCount = tilesX * tilesY; //scale factor
				o.uv0.x = o.uv0.x / tilesX;
				o.uv0.y = o.uv0.y / tilesY;
				int brushID = 255 * texInteract.a;//clamp(255-texInteract.r*255,0,255)/brushCount;//e.g. 5
				int division = (brushID + 1) / tilesX;//e.g. 2
				float ypoloipo = (brushID + 1) - (division*tilesX);//e.g. 5 - 2*2 = 5-4 = 1
				int sub = -1;
				if (ypoloipo > 0) {
					sub = 0;
				}
				o.uv0.x = o.uv0.x + (ypoloipo - 1)*(1 / tilesX);
				o.uv0.y = o.uv0.y + (division + sub)*(1 / tilesY);


                
                
                o.pos = UnityObjectToClipPos(v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            
            
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_582 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_582.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_582.rg, _Diffuse));
                
                
                
                clip(node_1.a - _Cutoff);
                
                //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
                    #else
                        float3 lightmap = DecodeLightmap(lmtex);
                    #endif
                #endif
                #ifndef LIGHTMAP_OFF
                    #ifdef DIRLIGHTMAP_OFF
                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    #else
                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
                    #endif
                #else
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                #endif
                
                
                lightDirection =  normalize(reflect(_WorldSpaceLightPos0.xyz,normalDirection));
                
                
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                //float attenuation = LIGHT_ATTENUATION(i); 
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
                
                   //INFINIGRASS
                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
                 float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                   float dist = distance(_OceanCenter, float3(_WaveControl1.x*i.posWorld.y,_WaveControl1.y*i.posWorld.x,_WaveControl1.z*i.posWorld.z) );
               
               float3 attenColor = attenuation * (_LightColor0.xyz);
               
               
               
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v1.4
                #ifndef LIGHTMAP_OFF
                    float3 diffuse = lightmap.rgb;
                #else
                    float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
                #endif
///////// Gloss:
                float gloss = 0.4;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.2;
                float3 specularColor = float3(node_3,node_3,node_3);
                float3 specular = 3 * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
//                #ifndef LIGHTMAP_OFF
//                    #ifndef DIRLIGHTMAP_OFF
//                        specular *= lightmap;
//                    #else
//                        specular *= (floor(attenuation) * _LightColor0.xyz);
//                    #endif
//                #else
//                    specular *= (floor(attenuation) * _LightColor0.xyz);
//                #endif
                specular *= ((attenuation) * _LightColor0.xyz);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_331 = 1.0;
             //   finalColor += diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
                finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal; //v1.4
                
                specular = specular * (i.uv0.y*2-0.5) ;
                finalColor = lerp(finalColor, finalColor*_Color,_TintPower*i.uv0.y*(0.9+0.6*cos(i.posWorld.x*2*_TintFrequency)+0.6*sin(i.posWorld.z*3*_TintFrequency)+0.6*sin(i.posWorld.z*1*_TintFrequency+0.1)));
                
                finalColor += specular * _SpecularPower;
/// Final Color:

				//SNOW
                float3 col = finalColor;
                
                float4 SnowTexColor = tex2D(_SnowTexture,  i.uv0);
				
				//if(i.uv0.y >= 1-(3 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r)
				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r* col.r+0.01) //v1.7.6
				if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01) //v2.0.7
	            {     	  
	            	if(i.uv0.y < 0.99 ){    //v1.7.6
		                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
		                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
		                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
		                //col.rgb = float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z; 
		                //v1.7.6
		               // col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*1.6;  
		               col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*clamp(col.r,0.85*(col.r+0.2),1);   //v2.0.7
	                }                      
	            }
	            else
	            {
					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
				//	col.rgb *= input.color.rgb;
				//	clip(col.a);
				//	col=col* _UnityTerrainTreeTintColorSM;
				}
                
                //END SNOW

				//v1.9.9.2
				//float shadowBrightnessCutoff = 0.1;
				//float shadowBrightness = 1;
				float SBCut = shadowBrightnessCutoff;
				if (col.r < SBCut && col.g < SBCut && col.b < SBCut) {
				//if (col.r < 0.1 && col.g < 0.1 && col.b < 0.1) {
					//col = col * (0.1 - col.r)* (0.1 - col.g)* (0.1 - col.b)*5111;
					float dist = sqrt((SBCut - col.r)* (SBCut - col.r) + (SBCut - col.g)*(SBCut - col.g) + (SBCut - col.b)*(SBCut - col.b));
					col = col + dist * float3(1, 1, 1) * 0.05 * shadowBrightness;
				}

                return fixed4(col,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fwdbase nolightmap //v4.1
           // #pragma exclude_renderers gles xbox360 ps3 flash 
           #pragma multi_compile_instancing //v1.7.8
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            

			//v2.0.8
			sampler2D _InteractTexture;
			float3 _InteractTexturePos;
			//v2.1.1
			float _shapeOnlyHeight;
			//v2.1.12
			float _XTiles;
			float _YTiles;

			//v2.1.13
			float4 scaleYFactor;
			//v2.1.14
			float3 _InteractAmpFreqRad;
			float3 _InteractPos2;

               uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
            uniform fixed _Cutoff;
             uniform float _RandYScale;
            uniform float _RippleScale;
            
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            float _SmoothMotionFactor;
            float _WaveXFactor;
           	float _WaveYFactor;


           	//v2.0.9
           	 float3 _Color;
            float3 _ColorGlobal;
           	float _TintPower; 
           	float _SpecularPower;
           
           	
           	sampler2D _SnowTexture;
           	float _SnowCoverage; 	
           	float _TintFrequency;

           	 float3 _InteractSpeed;
           	 float _InteractMaxYoffset;


           	
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
               // UNITY_SHADOW_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;



				//v2.0.8
				half2 tileableUv = mul(unity_ObjectToWorld, (v.vertex)).xz;
				float WorldScale = _InteractTexturePos.y;
				float3 CamPos = float3(0, 0, 0);//_DepthCameraPos;//_WorldSpaceCameraPos;
				float3 Origin = float3(_InteractTexturePos.x, _InteractTexturePos.z, 0);//float2(CamPos.x - WorldScale/2.0 , CamPos.z - WorldScale/2.0);
				float2 UnscaledTexPoint = float2(tileableUv.x - Origin.x, tileableUv.y - Origin.y);
				float2 ScaledTexPoint = float2(UnscaledTexPoint.x / WorldScale, UnscaledTexPoint.y / WorldScale);
				float4 texInteract = tex2Dlod(_InteractTexture, float4(ScaledTexPoint, 0.0, 0.0));




            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                               
                                 //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);     
                               
                  o.posWorld = mul(unity_ObjectToWorld, v.vertex);
             //  o.posWorld =  v.vertex;
                                      
//                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }


				  //v.vertex = o.posWorld;
				//v2.0.8
				  if (_shapeOnlyHeight == 0) { //v2.1.1
					  o.posWorld.y = o.posWorld.y*clamp((1 - texInteract.r*texInteract.r + 0.001)*1.5, 0, 1);
					  o.posWorld.y = o.posWorld.y - texInteract.r * 2; //v2.1.14
					  if (o.uv0.y > 0.01) {
						  //o.posWorld.y = o.posWorld.y - texInteract.b * 2;

						  if (texInteract.g >= 0.75) {
							  o.posWorld.x = o.posWorld.x - (1 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							  o.posWorld.z = o.posWorld.z - (texInteract.g - 0.75)*o.uv0.y * 42 * texInteract.b;
						  }
						  else if (texInteract.g >= 0.5) {	//negative angle region					
							  o.posWorld.x = o.posWorld.x - (texInteract.g - 0.5)*o.uv0.y * 42 * texInteract.b;
							  o.posWorld.z = o.posWorld.z + (0.75 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						  }
						  else if (texInteract.g >= 0.25) {
							  o.posWorld.x = o.posWorld.x + (0.5 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							  o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						  }
						  else if (texInteract.g > 0) {
							  o.posWorld.x = o.posWorld.x + texInteract.g*o.uv0.y * 42 * texInteract.b;
							  o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
						  }
					  }

					  //v2.1.16 - _grassNormal
					  //v.vertex.y = v.vertex.y*_respectHeight+ (pow(255-tex.r*255,_ShoreFadeFactor) - 25*pow(1+1-tex.r,1.5) + 8  + o.uv0.y*_grassHeight ); 
					  //if (_textureFeed == 1) { //v2.1.20b
					  //	v.vertex.y = v.vertex.y*_respectHeight + (pow(942.64 - tex.r*942.64, _ShoreFadeFactor) - 0 + o.uv0.y*_grassHeight);
					  ///}
					  //					        if(v.vertex.y > 25*_grassHeight){
					  //					       		v.vertex.y =v.vertex.y*_respectHeight + o.uv0.y*_grassHeight + 8;
					  //					        }

				  }
				  else {
					  //v.vertex.y = v.vertex.y-texInteract.b*_shapeOnlyHeight*2;
					  //v.vertex.y = abs(v.vertex.y)-pow(0+texInteract.r,_shapeOnlyHeight);
					  //v.vertex.y = v.vertex.y*((texInteract.r+0.011)*12.5);
					  o.posWorld.y = abs(o.posWorld.y) - sign(o.posWorld.y)*(1 - texInteract.r)*_shapeOnlyHeight;

					  //if (_textureFeed == 1) { //v2.1.20b
					  //	v.vertex.y = v.vertex.y*_respectHeight + (pow(255 - tex.r * 255, _ShoreFadeFactor) - 25 * pow(1 + 1 - tex.r, 1.5) + 8);
					  //	v.vertex.y = abs(v.vertex.y)*_grassHeight;
					  //}
				  }
				  //o.posWorld = v.vertex;



float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
                	}
                	if( o.uv0.y > 0.5){
						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
					}             
               }

				  //2.1.14
				  /////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
				  SpeedFac = float3(0, 0, 0);
				  distA = distance(_InteractPos2, o.posWorld) / (_InteractAmpFreqRad.z * 1);
				  if (distance(_InteractPos2, o.posWorld) < _InteractAmpFreqRad.z * 1) {
					  SpeedFac = 3 * (o.posWorld - _InteractPos2) *_WaveControl1.w
						  + _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), o.posWorld - _InteractPos2) - 2.71*cross(float3(0, 1, 0), _InteractPos2 - o.posWorld))
						  + _InteractAmpFreqRad.x*(o.posWorld - _InteractPos2) *_WaveControl1.w *sin(o.posWorld.z + _InteractAmpFreqRad.y*_Time.y);
					  _BulgeScale = _BulgeScale * (1 / (distA + 0.01));

					  if (o.uv0.y < 0.3) {

					  }
					  if (o.uv0.y > 0.19) {
						  _WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
						  _WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;
					  }
					  if (o.uv0.y > 0.5) {
						  o.posWorld.y = o.posWorld.y - (1 - distA) * 3 * (o.uv0.y - 0.5)*sin(o.posWorld.z + _Time.y);
					  }
				  }
				  /////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////
                               
                if( o.uv0.y > 0.1){
            //    	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
				}
                if( o.uv0.y >= 0.01){
           //     	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
                }
                
                  //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	           //   _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;

                  dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}

				//v2.1.13
				if (o.uv0.y > 0.16) {
					o.posWorld.y = o.posWorld.y * scaleYFactor.x + scaleYFactor.y;
				}

				//ADD GLOBAL ROTATION - WIND						
				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
				//  v.vertex =  o.posWorld;
                

			 //v2.1.12 - 2.0.8
			float tilesX = _XTiles;
			float tilesY = _YTiles;
			float brushCount = tilesX * tilesY; //scale factor
			o.uv0.x = o.uv0.x / tilesX;
			o.uv0.y = o.uv0.y / tilesY;
			int brushID = 255 * texInteract.a;//clamp(255-texInteract.r*255,0,255)/brushCount;//e.g. 5
			int division = (brushID + 1) / tilesX;//e.g. 2
			float ypoloipo = (brushID + 1) - (division*tilesX);//e.g. 5 - 2*2 = 5-4 = 1
			int sub = -1;
			if (ypoloipo > 0) {
				sub = 0;
			}
			o.uv0.x = o.uv0.x + (ypoloipo - 1)*(1 / tilesX);
			o.uv0.y = o.uv0.y + (division + sub)*(1 / tilesY);

                
                //o.posWorld = mul(_Object2World, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                //UNITY_TRANSFER_SHADOW(o,v.uv0);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR {
            
//                i.normalDir = normalize(i.normalDir);
//                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
//                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
///////// Normals:
//                float2 node_583 = i.uv0;
//                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_583.rg, _Normal))).rgb;
//                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
//                
//                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
//                i.normalDir *= nSign;
//                normalDirection *= nSign;
//                
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_583.rg, _Diffuse));
//                clip(node_1.a - _Cutoff);
//                
//                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//					 clip(-1);
//				}
//                
//                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
//                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// Lighting:
//                float attenuation = LIGHT_ATTENUATION(i);
//                
//                //INFINIGRASS
//                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
//                  float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                
//      //          float3 attenColor = attenuation * (_LightColor0.xyz*(node_133+1)  + _LightColor0.xyz*dot(viewDirection,lightDirection)/1);
//                
//                float3 attenColor = attenuation * _LightColor0.xyz ;
///////// Diffuse:
//                float NdotL = dot( normalDirection, lightDirection );
//                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
//                float3 NdotLWrap = NdotL * ( 1.0 - w );
//                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
//                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v2.0.9
//                float3 diffuse = (forwardLight+backLight) * attenColor;
/////////// Gloss:
//                float gloss = 0.4;
//                float specPow = exp2( gloss * 10.0+1.0);
//////// Specular:
//                NdotL = max(0.0, NdotL);
//                float node_3 = 0.2;
//                float3 specularColor = float3(node_3,node_3,node_3)*node_1;//v2.0.9
//                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
//                float3 finalColor = 0;
//                float3 diffuseLight = diffuse;
//                float node_331 = 1.0;
//                finalColor += float3(0,0,0);//diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
//                
//                
//                //INFINIGRASS
//                //if( i.uv0.y > 0.6){
////                if(dot(viewDirection,lightDirection) > 0.9){
////                	finalColor += (i.uv0.y)/6 ;
////                }
//                //}
//                
//                
//                finalColor += specular ;
///// Final Color:
//
//				
//
//                return fixed4(finalColor * 1,0);


i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_582 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_582.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_582.rg, _Diffuse));
                
                
                
                clip(node_1.a - _Cutoff);
                
                //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
//                #ifndef LIGHTMAP_OFF
//                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
//                    #ifndef DIRLIGHTMAP_OFF
//                        float3 lightmap = DecodeLightmap(lmtex);
//                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
//                        UNITY_DIRBASIS
//                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
//                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
//                    #else
//                        float3 lightmap = DecodeLightmap(lmtex);
//                    #endif
//                #endif
//                #ifndef LIGHTMAP_OFF
//                    #ifdef DIRLIGHTMAP_OFF
//                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//                    #else
//                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
//                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
//                    #endif
//                #else
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
               // #endif
                
                
                lightDirection =  normalize(reflect(_WorldSpaceLightPos0.xyz,normalDirection));
                
                
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                //float attenuation = 0;// LIGHT_ATTENUATION(i);
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
                
                
                   //INFINIGRASS
                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
                 float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                   float dist = distance(_OceanCenter, float3(_WaveControl1.x*i.posWorld.y,_WaveControl1.y*i.posWorld.x,_WaveControl1.z*i.posWorld.z) );
               
               float3 attenColor = attenuation * (_LightColor0.xyz);
               
               
               
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v1.4


                //v2.0.9
//                #ifndef LIGHTMAP_OFF
//                    float3 diffuse = lightmap.rgb;
//                #else
//                    float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
//                #endif


                //v2.0.9
                float3 diffuse = (forwardLight+backLight) * attenColor  + UNITY_LIGHTMODEL_AMBIENT.rgb;

///////// Gloss:
                float gloss = 0.4;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.2;
                float3 specularColor = float3(node_3,node_3,node_3)*node_1;
                float3 specular = 3 * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
//                #ifndef LIGHTMAP_OFF
//                    #ifndef DIRLIGHTMAP_OFF
//                        specular *= lightmap;
//                    #else
//                        specular *= (floor(attenuation) * _LightColor0.xyz);
//                    #endif
//                #else
//                    specular *= (floor(attenuation) * _LightColor0.xyz);
//                #endif
                specular *= ((attenuation) * _LightColor0.xyz);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_331 = 1.0;
             //   finalColor += diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
				finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal * 5 * attenuation; //v1.9.6 finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal; //v1.4
                
                specular = specular * (i.uv0.y*2-0.5) ;
                finalColor = lerp(finalColor, finalColor*_Color,_TintPower*i.uv0.y*(0.9+0.6*cos(i.posWorld.x*2*_TintFrequency)+0.6*sin(i.posWorld.z*3*_TintFrequency)+0.6*sin(i.posWorld.z*1*_TintFrequency+0.1)));
                
                finalColor += specular * _SpecularPower;
/// Final Color:

				//SNOW
                float3 col = finalColor;
                
                float4 SnowTexColor = tex2D(_SnowTexture,  i.uv0);
				
				//if(i.uv0.y >= 1-(3 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r)
				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r* col.r+0.01) //v1.7.6
				if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 ) //v2.0.7 
				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 +  i.uv0.y*2+0.6  ) //v2.0.8
				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)+  i.uv0.y*0.3 + 0.03 ) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 +  i.uv0.y*0.3 + 0.03  ) //v2.0.8
	            {     	  
	            	if(i.uv0.y < 0.99 ){    //v1.7.6
	            		//if(i.uv0.y >=  1-(4 * (_SnowCoverage+_TimeControl1.y-1))*clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 ){
			                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
			                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
			                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
			                //col.rgb = float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z; 
			                //v1.7.6
			               // col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*1.6;  
			               col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*clamp(col.r,0.85*(col.r+0.2),1);   //v2.0.7
			              // col.rgb = float3(1,1,1)*2*(_TimeControl1.z+i.uv0.y*1)*clamp(col.r,0.35*(col.r+0.2),1);   //v2.0.8
		               //}
	                }                      
	            }
	            else
	            {
					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
				//	col.rgb *= input.color.rgb;
				//	clip(col.a);
				//	col=col* _UnityTerrainTreeTintColorSM;
				}
                
                //END SNOW

                return fixed4(col,1);


            }
            ENDCG
        }
//        Pass {
//            Name "ShadowCollector"
//            Tags {
//                "LightMode"="ShadowCollector"
//            }
//            Cull Off
//            
//            Fog {Mode Off}
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #define UNITY_PASS_SHADOWCOLLECTOR
//            #define SHADOW_COLLECTOR_PASS
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcollector
//            #pragma exclude_renderers gles xbox360 ps3 flash 
//            #pragma target 3.0
//            uniform float4 _TimeEditor;
//            #ifndef LIGHTMAP_OFF
//                // float4 unity_LightmapST;
//                // sampler2D unity_Lightmap;
//                #ifndef DIRLIGHTMAP_OFF
//                    // sampler2D unity_LightmapInd;
//                #endif
//            #endif
//            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
//            
//            uniform float _BulgeScale; 
//            uniform float _BulgeShape;
//            uniform float _BulgeScale_copy;
//            float4 _WaveControl1;
//   			float4 _TimeControl1;
//    		float3 _OceanCenter;
//    		uniform fixed _Cutoff;
//    		 uniform float _RandYScale;
//            uniform float _RippleScale;
//            
//            float3 _InteractPos;
//            float _FadeThreshold;
//            
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float2 texcoord0 : TEXCOORD0;
//                float4 vertexColor : COLOR;
//            };
//            struct VertexOutput {
//                V2F_SHADOW_COLLECTOR;
//                float2 uv0 : TEXCOORD5;
//                float3 normalDir : TEXCOORD6;
//                float4 vertexColor : COLOR;
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o;
//                o.uv0 = v.texcoord0;
//                o.vertexColor = v.vertexColor;
//                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
//                float4 node_389 = o.vertexColor;
//                float4 node_392 = _Time + _TimeEditor;
//            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
//                
//                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(_Object2World, v.vertex).y,_WaveControl1.y*mul(_Object2World, v.vertex).x,_WaveControl1.z*mul(_Object2World, v.vertex).z) );
//                float dist2 = distance(_OceanCenter, float3(mul(_Object2World, v.vertex).y,mul(_Object2World, v.vertex).x*0.10,0.1*mul(_Object2World, v.vertex).z) );
//                
//                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
//                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                              
//                               //INIFNIGRASS
//                       float4 modelY = float4(0.0,1.0,0.0,0.0);
//                               float4 ModelYWorld =mul(_Object2World,modelY);
//                               float scaleY = length(ModelYWorld);
//                                 
//                if( o.uv0.y > 0.1){
//                	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;
//				}
//				if( o.uv0.y >= 0.01){
//                	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
//                }
//                
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                TRANSFER_SHADOW_COLLECTOR(o)
//                return o;
//            }
//            fixed4 frag(VertexOutput i) : COLOR {
//                i.normalDir = normalize(i.normalDir);
//                float2 node_584 = i.uv0;
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_584.rg, _Diffuse));
//                clip(node_1.a - _Cutoff);
//                
//                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				//if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//				//	 clip(-1);
//				//}
//                
//                
//                SHADOW_COLLECTOR_FRAGMENT(i)
//            }
//            ENDCG
//        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
        	//    #pragma exclude_renderers gles xbox360 ps3 flash 
			#pragma multi_compile_instancing //v1.7.8
			#pragma multi_compile_fwdbase nolightmap //v4.1
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            

			//v2.0.8
			sampler2D _InteractTexture;
			float3 _InteractTexturePos;
			//v2.1.1
			float _shapeOnlyHeight;
			//v2.1.12
			float _XTiles;
			float _YTiles;
			float _erasedShadowFactor;

			//v2.1.13
			float4 scaleYFactor;
			//v2.1.14
			float3 _InteractAmpFreqRad;
			float3 _InteractPos2;

            uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
    		uniform fixed _Cutoff;
    		 uniform float _RandYScale;
            uniform float _RippleScale;
            
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            float _SmoothMotionFactor;
            float _WaveXFactor;
           	float _WaveYFactor;

           	 float3 _InteractSpeed;
           	 float _InteractMaxYoffset;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;
              //  v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                

				//v2.0.8
				half2 tileableUv = mul(unity_ObjectToWorld, (v.vertex)).xz;
				float WorldScale = _InteractTexturePos.y;
				float3 CamPos = float3(0, 0, 0);//_DepthCameraPos;//_WorldSpaceCameraPos;
				float3 Origin = float3(_InteractTexturePos.x, _InteractTexturePos.z, 0);//float2(CamPos.x - WorldScale/2.0 , CamPos.z - WorldScale/2.0);
				float2 UnscaledTexPoint = float2(tileableUv.x - Origin.x, tileableUv.y - Origin.y);
				float2 ScaledTexPoint = float2(UnscaledTexPoint.x / WorldScale, UnscaledTexPoint.y / WorldScale);
				float4 texInteract = tex2Dlod(_InteractTexture, float4(ScaledTexPoint, 0.0, 0.0));



                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                            
                                //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);
                                  
                    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
              // o.posWorld =  v.vertex;
                                      
//                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }   


					//v.vertex = o.posWorld;
				//v2.0.8
					if (_shapeOnlyHeight == 0) { //v2.1.1
						o.posWorld.y = o.posWorld.y*clamp((1 - texInteract.r*texInteract.r* _erasedShadowFactor + 0.001)*1.5, 0, 1);
						o.posWorld.y = o.posWorld.y - texInteract.r * 2 * _erasedShadowFactor; //v2.1.14
						if (o.uv0.y > 0.01) {
							//o.posWorld.y = o.posWorld.y - texInteract.b * 2;

							if (texInteract.g >= 0.75) {
								o.posWorld.x = o.posWorld.x - (1 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
								o.posWorld.z = o.posWorld.z - (texInteract.g - 0.75)*o.uv0.y * 42 * texInteract.b;
							}
							else if (texInteract.g >= 0.5) {	//negative angle region					
								o.posWorld.x = o.posWorld.x - (texInteract.g - 0.5)*o.uv0.y * 42 * texInteract.b;
								o.posWorld.z = o.posWorld.z + (0.75 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							}
							else if (texInteract.g >= 0.25) {
								o.posWorld.x = o.posWorld.x + (0.5 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
								o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							}
							else if (texInteract.g > 0) {
								o.posWorld.x = o.posWorld.x + texInteract.g*o.uv0.y * 42 * texInteract.b;
								o.posWorld.z = o.posWorld.z + (0.25 - texInteract.g)*o.uv0.y * 42 * texInteract.b;
							}
						}

						//v2.1.16 - _grassNormal
						//v.vertex.y = v.vertex.y*_respectHeight+ (pow(255-tex.r*255,_ShoreFadeFactor) - 25*pow(1+1-tex.r,1.5) + 8  + o.uv0.y*_grassHeight ); 
						//if (_textureFeed == 1) { //v2.1.20b
						//	v.vertex.y = v.vertex.y*_respectHeight + (pow(942.64 - tex.r*942.64, _ShoreFadeFactor) - 0 + o.uv0.y*_grassHeight);
						///}
						//					        if(v.vertex.y > 25*_grassHeight){
						//					       		v.vertex.y =v.vertex.y*_respectHeight + o.uv0.y*_grassHeight + 8;
						//					        }

					}
					else {
						//v.vertex.y = v.vertex.y-texInteract.b*_shapeOnlyHeight*2;
						//v.vertex.y = abs(v.vertex.y)-pow(0+texInteract.r,_shapeOnlyHeight);
						//v.vertex.y = v.vertex.y*((texInteract.r+0.011)*12.5);
						o.posWorld.y = abs(o.posWorld.y) - sign(o.posWorld.y)*(1 - texInteract.r)*_shapeOnlyHeight;

						//if (_textureFeed == 1) { //v2.1.20b
						//	v.vertex.y = v.vertex.y*_respectHeight + (pow(255 - tex.r * 255, _ShoreFadeFactor) - 25 * pow(1 + 1 - tex.r, 1.5) + 8);
						//	v.vertex.y = abs(v.vertex.y)*_grassHeight;
						//}
					}
					//o.posWorld = v.vertex;



float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
                	}
                	if( o.uv0.y > 0.5){
						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
					}             
               }

				  //2.1.14
				  /////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
				  SpeedFac = float3(0, 0, 0);
				  distA = distance(_InteractPos2, o.posWorld) / (_InteractAmpFreqRad.z * 1);
				  if (distance(_InteractPos2, o.posWorld) < _InteractAmpFreqRad.z * 1) {
					  SpeedFac = 3 * (o.posWorld - _InteractPos2) *_WaveControl1.w
						  + _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), o.posWorld - _InteractPos2) - 2.71*cross(float3(0, 1, 0), _InteractPos2 - o.posWorld))
						  + _InteractAmpFreqRad.x*(o.posWorld - _InteractPos2) *_WaveControl1.w *sin(o.posWorld.z + _InteractAmpFreqRad.y*_Time.y);
					  _BulgeScale = _BulgeScale * (1 / (distA + 0.01));

					  if (o.uv0.y < 0.3) {

					  }
					  if (o.uv0.y > 0.19) {
						  _WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
						  _WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;
					  }
					  if (o.uv0.y > 0.5) {
						  o.posWorld.y = o.posWorld.y - (1 - distA) * 3 * (o.uv0.y - 0.5)*sin(o.posWorld.z + _Time.y);
					  }
				  }
				  /////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////
                                                                                         
                if( o.uv0.y > 0.1){
          //      	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
				}
				if( o.uv0.y >= 0.01){
          //      	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
                }

                  //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	         //  _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;
                
               dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}

				//v2.1.13
				if (o.uv0.y > 0.16) {
					o.posWorld.y = o.posWorld.y * scaleYFactor.x + scaleYFactor.y;
				}

				//ADD GLOBAL ROTATION - WIND						
				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
	            //v.vertex =  o.posWorld;
                
                
				 //v2.1.12 - 2.0.8
				float tilesX = _XTiles;
				float tilesY = _YTiles;
				float brushCount = tilesX * tilesY; //scale factor
				o.uv0.x = o.uv0.x / tilesX;
				o.uv0.y = o.uv0.y / tilesY;
				int brushID = 255 * texInteract.a;//clamp(255-texInteract.r*255,0,255)/brushCount;//e.g. 5
				int division = (brushID + 1) / tilesX;//e.g. 2
				float ypoloipo = (brushID + 1) - (division*tilesX);//e.g. 5 - 2*2 = 5-4 = 1
				int sub = -1;
				if (ypoloipo > 0) {
					sub = 0;
				}
				o.uv0.x = o.uv0.x + (ypoloipo - 1)*(1 / tilesX);
				o.uv0.y = o.uv0.y + (division + sub)*(1 / tilesY);
                
                
                o.pos = UnityObjectToClipPos(v.vertex);
               // o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float2 node_585 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_585.rg, _Diffuse));
                clip(node_1.a - _Cutoff);
                
                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse"
   
}