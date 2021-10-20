// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X
Shader "Hovl/Particles/Sand"
{
	Properties
	{
		_Numberofwaves("Number of waves", Float) = 1
		_WavesspeedsizeXYTwistspeedsizeZW("Waves speed-size XY Twist speed-size ZW", Vector) = (-1,0.2,4,0.6)
		_VertexScale("VertexScale", Float) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Noisescale("Noise scale", Float) = 1000
		_Noisepower("Noise power", Float) = 1
		_Noiselerp("Noise lerp", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_Emissionpower("Emission power", Float) = 1
		_Emission("Emission", Float) = 2
		_OpacityTex("OpacityTex", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Maskpower("Mask power", Float) = 1
		_Maskmultiplayer("Mask multiplayer", Float) = 3
		[Toggle]_Softedges("Soft edges", Float) = 0
		[Toggle]_Usedepth("Use depth", Float) = 0
		_Depthpower("Depth power", Float) = 1
		_OpacityTexspeedXY("OpacityTex speed XY", Vector) = (0,-0.5,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
			float3 worldNormal;
			float3 viewDir;
		};

		uniform float _Numberofwaves;
		uniform float4 _WavesspeedsizeXYTwistspeedsizeZW;
		uniform float _VertexScale;
		uniform float4 _SpeedMainTexUVNoiseZW;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Noisescale;
		uniform float _Noisepower;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;
		uniform float _Noiselerp;
		uniform float _Emissionpower;
		uniform float _Emission;
		uniform float4 _Color;
		uniform float _Softedges;
		uniform float _Usedepth;
		uniform sampler2D _OpacityTex;
		uniform float4 _OpacityTexspeedXY;
		uniform float4 _OpacityTex_ST;
		uniform float _Maskpower;
		uniform float _Maskmultiplayer;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depthpower;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float w28 = v.texcoord.z;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_112_0 = ( ase_vertex3Pos.y * _VertexScale );
			float mulTime3 = _Time.y * _WavesspeedsizeXYTwistspeedsizeZW.x;
			float mulTime6 = _Time.y * _WavesspeedsizeXYTwistspeedsizeZW.z;
			float3 appendResult23 = (float3(( sin( ( temp_output_112_0 + mulTime6 ) ) * w28 ) , 0.0 , ( w28 * sin( ( mulTime6 + temp_output_112_0 + ( UNITY_PI / 2.0 ) ) ) )));
			v.vertex.xyz += ( ( ( ase_vertexNormal * ( w28 * sin( ( _Numberofwaves * ( temp_output_112_0 + mulTime3 ) * UNITY_PI ) ) ) * _WavesspeedsizeXYTwistspeedsizeZW.y ) + ( _WavesspeedsizeXYTwistspeedsizeZW.w * appendResult23 ) ) * float3( 1,0,1 ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult34 = (float2(_SpeedMainTexUVNoiseZW.x , _SpeedMainTexUVNoiseZW.y));
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner31 = ( 1.0 * _Time.y * appendResult34 + uv0_MainTex);
			float simplePerlin2D40 = snoise( panner31*_Noisescale );
			simplePerlin2D40 = simplePerlin2D40*0.5 + 0.5;
			float4 temp_cast_0 = (( simplePerlin2D40 * _Noisepower )).xxxx;
			float2 appendResult36 = (float2(_SpeedMainTexUVNoiseZW.z , _SpeedMainTexUVNoiseZW.w));
			float2 uv0_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			float2 panner38 = ( 1.0 * _Time.y * appendResult36 + uv0_Noise);
			float4 lerpResult44 = lerp( temp_cast_0 , ( tex2D( _MainTex, panner31 ) * tex2D( _Noise, panner38 ) ) , _Noiselerp);
			float4 temp_cast_1 = (_Emissionpower).xxxx;
			o.Emission = ( pow( lerpResult44 , temp_cast_1 ) * _Emission * _Color * i.vertexColor ).rgb;
			float2 appendResult110 = (float2(_OpacityTexspeedXY.x , _OpacityTexspeedXY.y));
			float2 uv0_OpacityTex = i.uv_texcoord * _OpacityTex_ST.xy + _OpacityTex_ST.zw;
			float2 panner94 = ( 1.0 * _Time.y * appendResult110 + uv0_OpacityTex);
			float clampResult97 = clamp( ( pow( tex2D( _OpacityTex, panner94 ).r , _Maskpower ) * _Maskmultiplayer ) , 0.0 , 1.0 );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float temp_output_62_0 = ( _Color.a * i.vertexColor.a * clampResult97 * tex2D( _Mask, uv_Mask ).a );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth71 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth71 = abs( ( screenDepth71 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depthpower ) );
			float clampResult73 = clamp( distanceDepth71 , 0.0 , 1.0 );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult80 = dot( ase_worldNormal , i.viewDir );
			float temp_output_89_0 = ( pow( dotResult80 , 3.0 ) * 1.5 );
			float clampResult99 = clamp( ( pow( uv0_MainTex.y , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float lerpResult84 = lerp( temp_output_89_0 , ( clampResult99 * (0.0 + (temp_output_89_0 - 0.0) * (1.0 - 0.0) / (-1.0 - 0.0)) ) , (1.0 + (sign( dotResult80 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0)));
			float clampResult85 = clamp( lerpResult84 , 0.0 , 1.0 );
			float clampResult91 = clamp( clampResult85 , 0.0 , 1.0 );
			o.Alpha = lerp(lerp(temp_output_62_0,( temp_output_62_0 * clampResult73 ),_Usedepth),( lerp(temp_output_62_0,( temp_output_62_0 * clampResult73 ),_Usedepth) * clampResult91 ),_Softedges);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
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
				float3 worldNormal : TEXCOORD4;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				surfIN.viewDir = worldViewDir;
				surfIN.worldNormal = IN.worldNormal;
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
}
/*ASEBEGIN
Version=17000
7;128;1759;905;1558.013;-2186.429;1.491038;True;False
Node;AmplifyShaderEditor.CommentaryNode;105;-1782.93,1133.382;Float;False;2035.048;868.9979;Soft edges;15;82;83;84;85;91;100;86;101;99;98;87;89;81;79;80;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;109;-2309.97,549.2347;Float;False;Property;_OpacityTexspeedXY;OpacityTex speed XY;19;0;Create;True;0;0;False;0;0,-0.5,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;81;-1732.93,1497.234;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;79;-1725.894,1644.996;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;110;-1977.97,579.2347;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;95;-2067.155,459.6099;Float;False;0;63;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;80;-1495.457,1608.056;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;94;-1610.606,503.7028;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;106;-1093.628,2219.066;Float;False;1851.543;1224.21;Vetex offset;28;1;2;3;4;5;6;8;9;11;10;7;15;13;14;12;19;18;17;16;21;22;23;20;24;25;26;112;113;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-1993.299,-79.58044;Float;False;0;29;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PiNode;1;-730.259,3315.824;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;2;-1043.628,2739.447;Float;False;Property;_WavesspeedsizeXYTwistspeedsizeZW;Waves speed-size XY Twist speed-size ZW;1;0;Create;True;0;0;False;0;-1,0.2,4,0.6;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;98;-1334.583,1183.382;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-1421.938,444.807;Float;True;Property;_OpacityTex;OpacityTex;12;0;Create;True;0;0;False;0;05ea33e1e37a8c245aca3e86dec28fdf;4b0225a5290cbe540bc56e26a8682db2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;54;-1313.171,631.929;Float;False;Property;_Maskpower;Mask power;14;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;87;-1292.546,1508.552;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;4;-1029.912,2971.659;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;113;-1010.997,3111.513;Float;False;Property;_VertexScale;VertexScale;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;32;-2475.029,113.5536;Float;False;Property;_SpeedMainTexUVNoiseZW;Speed MainTex U/V + Noise Z/W;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-1879.248,2377.91;Float;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1114.472,1508.615;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-1111.835,633.8851;Float;False;Property;_Maskmultiplayer;Mask multiplayer;15;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;53;-1104.212,537.9693;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-689.3636,2589.115;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-1170.83,1184.735;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-807.4176,3017.529;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;5;-539.4658,3310.276;Float;False;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;6;-575.4227,3042.801;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-362.5024,2906.916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-891.8019,538.4253;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;99;-929.196,1184.342;Float;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;82;-1021.445,1802.546;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1642.536,2442.131;Float;False;w;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-1900.836,89.92324;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-737.9787,989.5668;Float;False;Property;_Depthpower;Depth power;18;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-487.0428,2565.321;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1893.917,302.0966;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-1989.587,182.8138;Float;False;0;35;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-364.8605,3226.743;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-512.3538,2486.775;Float;False;Property;_Numberofwaves;Number of waves;0;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;86;-928.6188,1592.925;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;-1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;8;-478.1588,2667.723;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-287.4584,2541.716;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;71;-523.9275,970.9049;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;31;-1706.663,22.98934;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;12;-219.9214,3232.914;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1628.532,-312.4841;Float;False;Property;_Noisescale;Noise scale;6;0;Create;True;0;0;False;0;1000;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;38;-1699.744,235.1628;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;14;-216.7355,2904.141;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;50;-553.3295,263.2375;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;83;-863.4612,1800.38;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-341.2184,3053.271;Float;False;28;w;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;49;-593.6298,76.03733;Float;False;Property;_Color;Color;9;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;52;-878.2848,656.6249;Float;True;Property;_Mask;Mask;13;0;Create;True;0;0;False;0;05ea33e1e37a8c245aca3e86dec28fdf;05ea33e1e37a8c245aca3e86dec28fdf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-719.9819,1569.965;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;97;-734.5894,538.6395;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1322.135,-109.8612;Float;False;Property;_Noisepower;Noise power;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-81.23772,2905.754;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-148.5247,2428.848;Float;False;28;w;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;84;-517.6207,1513.128;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;16;-134.7847,2540.916;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;40;-1387.777,-334.0132;Float;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-81.7157,3174.965;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;73;-252.2921,972.2811;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;35;-1419.74,254.6808;Float;True;Property;_Noise;Noise;4;0;Create;True;0;0;False;0;74ed93858b3298e4f93e6146b3ef490c;74ed93858b3298e4f93e6146b3ef490c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-279.5515,638.8843;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-1422.349,38.68793;Float;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;74ed93858b3298e4f93e6146b3ef490c;74ed93858b3298e4f93e6146b3ef490c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;23;91.48537,2991.696;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;21;92.91148,2269.066;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;85;-331.6248,1512.939;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1049.707,-61.41811;Float;False;Property;_Noiselerp;Noise lerp;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1038.135,-198.861;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;96.25678,2434.511;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-84.76637,819.1151;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1042.133,164.2799;Float;False;2;2;0;COLOR;1,1,1,1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;22;17.69128,2745.085;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;91;77.118,1250.014;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-826.4573,-108.3406;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;373.5433,2412.458;Float;False;3;3;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;74;105.5726,639.8444;Float;False;Property;_Usedepth;Use depth;17;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;313.9922,2827.883;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-820.3423,36.43726;Float;False;Property;_Emissionpower;Emission power;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;373.2736,773.8013;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;47;-586.3438,-109.1628;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-540.7476,-7.942116;Float;False;Property;_Emission;Emission;11;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;603.9154,2525.402;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-299.0436,-106.5626;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;76;533.3589,639.5092;Float;False;Property;_Softedges;Soft edges;16;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;984.66,1038.067;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;69;1260.156,160.5588;Float;False;True;2;Float;;0;0;Standard;Hovl/Particles/Sand;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;110;0;109;1
WireConnection;110;1;109;2
WireConnection;80;0;81;0
WireConnection;80;1;79;0
WireConnection;94;0;95;0
WireConnection;94;2;110;0
WireConnection;98;0;30;2
WireConnection;63;1;94;0
WireConnection;87;0;80;0
WireConnection;89;0;87;0
WireConnection;53;0;63;1
WireConnection;53;1;54;0
WireConnection;3;0;2;1
WireConnection;101;0;98;0
WireConnection;112;0;4;2
WireConnection;112;1;113;0
WireConnection;5;0;1;0
WireConnection;6;0;2;3
WireConnection;7;0;112;0
WireConnection;7;1;6;0
WireConnection;96;0;53;0
WireConnection;96;1;103;0
WireConnection;99;0;101;0
WireConnection;82;0;80;0
WireConnection;28;0;102;3
WireConnection;34;0;32;1
WireConnection;34;1;32;2
WireConnection;11;0;112;0
WireConnection;11;1;3;0
WireConnection;36;0;32;3
WireConnection;36;1;32;4
WireConnection;10;0;6;0
WireConnection;10;1;112;0
WireConnection;10;2;5;0
WireConnection;86;0;89;0
WireConnection;15;0;9;0
WireConnection;15;1;11;0
WireConnection;15;2;8;0
WireConnection;71;0;72;0
WireConnection;31;0;30;0
WireConnection;31;2;34;0
WireConnection;12;0;10;0
WireConnection;38;0;37;0
WireConnection;38;2;36;0
WireConnection;14;0;7;0
WireConnection;83;0;82;0
WireConnection;100;0;99;0
WireConnection;100;1;86;0
WireConnection;97;0;96;0
WireConnection;17;0;14;0
WireConnection;17;1;13;0
WireConnection;84;0;89;0
WireConnection;84;1;100;0
WireConnection;84;2;83;0
WireConnection;16;0;15;0
WireConnection;40;0;31;0
WireConnection;40;1;107;0
WireConnection;18;0;13;0
WireConnection;18;1;12;0
WireConnection;73;0;71;0
WireConnection;35;1;38;0
WireConnection;62;0;49;4
WireConnection;62;1;50;4
WireConnection;62;2;97;0
WireConnection;62;3;52;4
WireConnection;29;1;31;0
WireConnection;23;0;17;0
WireConnection;23;2;18;0
WireConnection;85;0;84;0
WireConnection;42;0;40;0
WireConnection;42;1;43;0
WireConnection;20;0;19;0
WireConnection;20;1;16;0
WireConnection;75;0;62;0
WireConnection;75;1;73;0
WireConnection;39;0;29;0
WireConnection;39;1;35;0
WireConnection;22;0;2;2
WireConnection;91;0;85;0
WireConnection;44;0;42;0
WireConnection;44;1;39;0
WireConnection;44;2;45;0
WireConnection;25;0;21;0
WireConnection;25;1;20;0
WireConnection;25;2;22;0
WireConnection;74;0;62;0
WireConnection;74;1;75;0
WireConnection;24;0;2;4
WireConnection;24;1;23;0
WireConnection;77;0;74;0
WireConnection;77;1;91;0
WireConnection;47;0;44;0
WireConnection;47;1;46;0
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;48;0;47;0
WireConnection;48;1;51;0
WireConnection;48;2;49;0
WireConnection;48;3;50;0
WireConnection;76;0;74;0
WireConnection;76;1;77;0
WireConnection;93;0;26;0
WireConnection;69;2;48;0
WireConnection;69;9;76;0
WireConnection;69;11;93;0
ASEEND*/
//CHKSM=8FDB63155A98D42D955D5D218886655121AF614A