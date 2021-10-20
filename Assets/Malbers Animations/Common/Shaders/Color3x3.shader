// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Color3x3"
{
	Properties
	{
		[Header(Albedo (A Gradient))]_Color1("Color 1", Color) = (1,0.1544118,0.1544118,0)
		_Color2("Color 2", Color) = (1,0.1544118,0.8017241,1)
		_Color3("Color 3", Color) = (0.2535501,0.1544118,1,1)
		[Space(10)]_Color4("Color 4", Color) = (0.9533468,1,0.1544118,1)
		_Color5("Color 5", Color) = (0.2669384,0.3207547,0.0226949,1)
		_Color6("Color 6", Color) = (1,0.4519259,0.1529412,1)
		[Space(10)]_Color7("Color 7", Color) = (0.9099331,0.9264706,0.6267301,1)
		_Color8("Color 8", Color) = (0.1544118,0.1602434,1,1)
		_Color9("Color 9", Color) = (0.1529412,0.9929401,1,1)
		[Header(Metallic(R) Rough(G) Emmission(B))]_MRE1("MRE 1", Color) = (0,1,0,0)
		_MRE2("MRE 2", Color) = (0,1,0,0)
		_MRE3("MRE 3", Color) = (0,1,0,0)
		[Space(10)]_MRE4("MRE 4", Color) = (0,1,0,0)
		_MRE5("MRE 5", Color) = (0,1,0,0)
		_MRE6("MRE 6", Color) = (0,1,0,0)
		[Space()]_MRE7("MRE 7", Color) = (0,1,0,0)
		_MRE8("MRE 8", Color) = (0,1,0,0)
		_MRE9("MRE 9", Color) = (0,1,0,0)
		[Header(Emmision)]_EmissionPower("Emission Power", Float) = 1
		[SingleLineTexture][Header(Gradient)]_Gradient("Gradient", 2D) = "white" {}
		_GradientIntensity("Gradient Intensity", Range( 0 , 1)) = 0.75
		_GradientColor("Gradient Color", Color) = (0,0,0,0)
		_GradientScale("Gradient Scale", Float) = 1
		_GradientOffset("Gradient Offset", Float) = 0
		_GradientPower("Gradient Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform float4 _Color5;
		uniform float4 _Color6;
		uniform float4 _Color7;
		uniform float4 _Color8;
		uniform float4 _Color9;
		uniform sampler2D _Gradient;
		uniform float4 _GradientColor;
		uniform float _GradientIntensity;
		uniform float _GradientScale;
		uniform float _GradientOffset;
		uniform float _GradientPower;
		uniform float _EmissionPower;
		uniform float4 _MRE1;
		uniform float4 _MRE2;
		uniform float4 _MRE3;
		uniform float4 _MRE4;
		uniform float4 _MRE5;
		uniform float4 _MRE6;
		uniform float4 _MRE7;
		uniform float4 _MRE8;
		uniform float4 _MRE9;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_3_0_g361 = 1.0;
			float temp_output_7_0_g361 = 3.0;
			float temp_output_9_0_g361 = 3.0;
			float temp_output_8_0_g361 = 3.0;
			float temp_output_3_0_g351 = 2.0;
			float temp_output_7_0_g351 = 3.0;
			float temp_output_9_0_g351 = 3.0;
			float temp_output_8_0_g351 = 3.0;
			float temp_output_3_0_g365 = 3.0;
			float temp_output_7_0_g365 = 3.0;
			float temp_output_9_0_g365 = 3.0;
			float temp_output_8_0_g365 = 3.0;
			float temp_output_3_0_g360 = 1.0;
			float temp_output_7_0_g360 = 3.0;
			float temp_output_9_0_g360 = 2.0;
			float temp_output_8_0_g360 = 3.0;
			float temp_output_3_0_g363 = 2.0;
			float temp_output_7_0_g363 = 3.0;
			float temp_output_9_0_g363 = 2.0;
			float temp_output_8_0_g363 = 3.0;
			float temp_output_3_0_g359 = 3.0;
			float temp_output_7_0_g359 = 3.0;
			float temp_output_9_0_g359 = 2.0;
			float temp_output_8_0_g359 = 3.0;
			float temp_output_3_0_g358 = 1.0;
			float temp_output_7_0_g358 = 3.0;
			float temp_output_9_0_g358 = 1.0;
			float temp_output_8_0_g358 = 3.0;
			float temp_output_3_0_g362 = 2.0;
			float temp_output_7_0_g362 = 3.0;
			float temp_output_9_0_g362 = 1.0;
			float temp_output_8_0_g362 = 3.0;
			float temp_output_3_0_g364 = 3.0;
			float temp_output_7_0_g364 = 3.0;
			float temp_output_9_0_g364 = 1.0;
			float temp_output_8_0_g364 = 3.0;
			float4 temp_output_155_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g361 - 1.0 ) / temp_output_7_0_g361 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g361 / temp_output_7_0_g361 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g361 - 1.0 ) / temp_output_8_0_g361 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g361 / temp_output_8_0_g361 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g351 - 1.0 ) / temp_output_7_0_g351 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g351 / temp_output_7_0_g351 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g351 - 1.0 ) / temp_output_8_0_g351 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g351 / temp_output_8_0_g351 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g365 - 1.0 ) / temp_output_7_0_g365 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g365 / temp_output_7_0_g365 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g365 - 1.0 ) / temp_output_8_0_g365 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g365 / temp_output_8_0_g365 ) ) * 1.0 ) ) ) ) ) + ( ( _Color4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g360 - 1.0 ) / temp_output_7_0_g360 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g360 / temp_output_7_0_g360 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g360 - 1.0 ) / temp_output_8_0_g360 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g360 / temp_output_8_0_g360 ) ) * 1.0 ) ) ) ) + ( _Color5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g363 - 1.0 ) / temp_output_7_0_g363 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g363 / temp_output_7_0_g363 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g363 - 1.0 ) / temp_output_8_0_g363 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g363 / temp_output_8_0_g363 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g359 - 1.0 ) / temp_output_7_0_g359 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g359 / temp_output_7_0_g359 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g359 - 1.0 ) / temp_output_8_0_g359 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g359 / temp_output_8_0_g359 ) ) * 1.0 ) ) ) ) ) + ( ( _Color7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g358 - 1.0 ) / temp_output_7_0_g358 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g358 / temp_output_7_0_g358 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g358 - 1.0 ) / temp_output_8_0_g358 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g358 / temp_output_8_0_g358 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g362 - 1.0 ) / temp_output_7_0_g362 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g362 / temp_output_7_0_g362 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g362 - 1.0 ) / temp_output_8_0_g362 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g362 / temp_output_8_0_g362 ) ) * 1.0 ) ) ) ) + ( _Color9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g364 - 1.0 ) / temp_output_7_0_g364 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g364 / temp_output_7_0_g364 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g364 - 1.0 ) / temp_output_8_0_g364 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g364 / temp_output_8_0_g364 ) ) * 1.0 ) ) ) ) ) );
			float2 uv_TexCoord258 = i.uv_texcoord * float2( 3,3 );
			float4 clampResult206 = clamp( ( ( tex2D( _Gradient, uv_TexCoord258 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 saferPower254 = max( (clampResult206*_GradientScale + _GradientOffset) , 0.0001 );
			float4 temp_cast_0 = (_GradientPower).xxxx;
			float4 clampResult255 = clamp( ( pow( saferPower254 , temp_cast_0 ) + ( 1.0 - (temp_output_155_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Albedo = ( temp_output_155_0 * clampResult255 ).rgb;
			float temp_output_3_0_g372 = 1.0;
			float temp_output_7_0_g372 = 3.0;
			float temp_output_9_0_g372 = 3.0;
			float temp_output_8_0_g372 = 3.0;
			float temp_output_3_0_g373 = 2.0;
			float temp_output_7_0_g373 = 3.0;
			float temp_output_9_0_g373 = 3.0;
			float temp_output_8_0_g373 = 3.0;
			float temp_output_3_0_g374 = 3.0;
			float temp_output_7_0_g374 = 3.0;
			float temp_output_9_0_g374 = 3.0;
			float temp_output_8_0_g374 = 3.0;
			float temp_output_3_0_g368 = 1.0;
			float temp_output_7_0_g368 = 3.0;
			float temp_output_9_0_g368 = 2.0;
			float temp_output_8_0_g368 = 3.0;
			float temp_output_3_0_g367 = 2.0;
			float temp_output_7_0_g367 = 3.0;
			float temp_output_9_0_g367 = 2.0;
			float temp_output_8_0_g367 = 3.0;
			float temp_output_3_0_g370 = 3.0;
			float temp_output_7_0_g370 = 3.0;
			float temp_output_9_0_g370 = 2.0;
			float temp_output_8_0_g370 = 3.0;
			float temp_output_3_0_g366 = 1.0;
			float temp_output_7_0_g366 = 3.0;
			float temp_output_9_0_g366 = 1.0;
			float temp_output_8_0_g366 = 3.0;
			float temp_output_3_0_g369 = 2.0;
			float temp_output_7_0_g369 = 3.0;
			float temp_output_9_0_g369 = 1.0;
			float temp_output_8_0_g369 = 3.0;
			float temp_output_3_0_g371 = 3.0;
			float temp_output_7_0_g371 = 3.0;
			float temp_output_9_0_g371 = 1.0;
			float temp_output_8_0_g371 = 3.0;
			float4 temp_output_263_0 = ( ( ( _MRE1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g372 - 1.0 ) / temp_output_7_0_g372 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g372 / temp_output_7_0_g372 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g372 - 1.0 ) / temp_output_8_0_g372 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g372 / temp_output_8_0_g372 ) ) * 1.0 ) ) ) ) + ( _MRE2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g373 - 1.0 ) / temp_output_7_0_g373 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g373 / temp_output_7_0_g373 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g373 - 1.0 ) / temp_output_8_0_g373 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g373 / temp_output_8_0_g373 ) ) * 1.0 ) ) ) ) + ( _MRE3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g374 - 1.0 ) / temp_output_7_0_g374 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g374 / temp_output_7_0_g374 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g374 - 1.0 ) / temp_output_8_0_g374 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g374 / temp_output_8_0_g374 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g368 - 1.0 ) / temp_output_7_0_g368 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g368 / temp_output_7_0_g368 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g368 - 1.0 ) / temp_output_8_0_g368 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g368 / temp_output_8_0_g368 ) ) * 1.0 ) ) ) ) + ( _MRE5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g367 - 1.0 ) / temp_output_7_0_g367 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g367 / temp_output_7_0_g367 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g367 - 1.0 ) / temp_output_8_0_g367 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g367 / temp_output_8_0_g367 ) ) * 1.0 ) ) ) ) + ( _MRE6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g370 - 1.0 ) / temp_output_7_0_g370 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g370 / temp_output_7_0_g370 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g370 - 1.0 ) / temp_output_8_0_g370 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g370 / temp_output_8_0_g370 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g366 - 1.0 ) / temp_output_7_0_g366 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g366 / temp_output_7_0_g366 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g366 - 1.0 ) / temp_output_8_0_g366 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g366 / temp_output_8_0_g366 ) ) * 1.0 ) ) ) ) + ( _MRE8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g369 - 1.0 ) / temp_output_7_0_g369 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g369 / temp_output_7_0_g369 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g369 - 1.0 ) / temp_output_8_0_g369 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g369 / temp_output_8_0_g369 ) ) * 1.0 ) ) ) ) + ( _MRE9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g371 - 1.0 ) / temp_output_7_0_g371 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g371 / temp_output_7_0_g371 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g371 - 1.0 ) / temp_output_8_0_g371 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g371 / temp_output_8_0_g371 ) ) * 1.0 ) ) ) ) ) );
			o.Emission = ( temp_output_155_0 * ( _EmissionPower * (temp_output_263_0).b ) ).rgb;
			o.Metallic = (temp_output_263_0).r;
			o.Smoothness = ( 1.0 - (temp_output_263_0).g );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18702
309;162;1266;746;-139.9272;1406.659;2.000664;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-339.6552,-1162.801;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;3,3;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;156;-369.1905,827.4952;Float;False;Property;_Color5;Color 5;4;0;Create;True;0;0;False;0;False;0.2669384,0.3207547,0.0226949,1;0.6226415,0.6226415,0.6226415,0.6;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;181;-243.7083,1591.022;Float;False;Property;_Color8;Color 8;7;0;Create;True;0;0;False;0;False;0.1544118,0.1602434,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;159;-367.2498,538.3683;Float;False;Property;_Color4;Color 4;3;0;Create;True;0;0;False;1;Space(10);False;0.9533468,1,0.1544118,1;0.6226415,0.6226415,0.6226415,0.6;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;201;-69.3144,-738.3137;Float;False;Property;_GradientIntensity;Gradient Intensity;20;0;Create;True;0;0;False;0;False;0.75;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;157;-235.251,1079.311;Float;False;Property;_Color6;Color 6;5;0;Create;True;0;0;False;0;False;1,0.4519259,0.1529412,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-380.4475,-229.5;Float;False;Property;_Color1;Color 1;0;0;Create;True;0;0;False;1;Header(Albedo (A Gradient));False;1,0.1544118,0.1544118,0;0.754717,0.754717,0.754717,0.454902;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-377.5372,262.0459;Float;False;Property;_Color3;Color 3;2;0;Create;True;0;0;False;0;False;0.2535501,0.1544118,1,1;0.6226415,0.6226415,0.6226415,0.6;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;202;-41.02644,-1165.228;Inherit;True;Property;_Gradient;Gradient;19;1;[SingleLineTexture];Create;True;0;0;False;1;Header(Gradient);False;-1;0f424a347039ef447a763d3d4b4782b0;0f424a347039ef447a763d3d4b4782b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;200;-60.35785,-921.4749;Float;False;Property;_GradientColor;Gradient Color;21;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;150;-391.0649,27.18103;Float;False;Property;_Color2;Color 2;1;0;Create;True;0;0;False;0;False;1,0.1544118,0.8017241,1;0.6226415,0.6226415,0.6226415,0.6;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;183;-251.6285,1359.862;Float;False;Property;_Color7;Color 7;6;0;Create;True;0;0;False;1;Space(10);False;0.9099331,0.9264706,0.6267301,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;256;-244.6775,1818.924;Float;False;Property;_Color9;Color 9;8;0;Create;True;0;0;False;0;False;0.1529412,0.9929401,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;232;25.81848,1594.321;Inherit;True;ColorShartSlot;-1;;362;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;238;2.790063,246.9754;Inherit;True;ColorShartSlot;-1;;365;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;257;28.45395,1819.095;Inherit;True;ColorShartSlot;-1;;364;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;231;11.13652,815.7118;Inherit;True;ColorShartSlot;-1;;363;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;239;-2.797049,-241.6734;Inherit;True;ColorShartSlot;-1;;361;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;235;25.18534,1368.447;Inherit;True;ColorShartSlot;-1;;358;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;204;301.5615,-792.5283;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;203;328.2687,-922.1614;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;240;14.66442,1076.863;Inherit;True;ColorShartSlot;-1;;359;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;236;-10.73773,16.68434;Inherit;True;ColorShartSlot;-1;;351;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;233;13.07732,530.6414;Inherit;True;ColorShartSlot;-1;;360;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;272;756.025,2958.429;Float;False;Property;_MRE9;MRE 9;17;0;Create;True;0;0;False;0;False;0,1,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;267;797.8815,1892.403;Float;False;Property;_MRE4;MRE 4;12;0;Create;True;0;0;False;1;Space(10);False;0,1,0,0;0.4392157,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;264;800.6055,1472.631;Float;False;Property;_MRE2;MRE 2;10;0;Create;True;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;266;764.9778,2103.387;Float;False;Property;_MRE5;MRE 5;13;0;Create;True;0;0;False;0;False;0,1,0,0;0.854902,0.7686275,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;269;794.9661,1683.904;Float;False;Property;_MRE3;MRE 3;11;0;Create;True;0;0;False;0;False;0,1,0,0;0.7137255,0.854902,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;265;762.6077,2329.814;Float;False;Property;_MRE6;MRE 6;14;0;Create;True;0;0;False;0;False;0,1,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;271;760.3356,2757.597;Float;False;Property;_MRE8;MRE 8;16;0;Create;True;0;0;False;0;False;0,1,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;205;508.7686,-952.5815;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;268;792.0692,1258.979;Float;False;Property;_MRE1;MRE 1;9;0;Create;True;0;0;False;1;Header(Metallic(R) Rough(G) Emmission(B));False;0,1,0,0;0.4823529,0.4901961,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;146;636.8021,241.9187;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;643.5082,470.012;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;193;639.0421,747.4011;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;270;762.3789,2542.473;Float;False;Property;_MRE7;MRE 7;15;0;Create;True;0;0;False;1;Space();False;0,1,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;279;1072.083,1685.052;Inherit;True;ColorShartSlot;-1;;374;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;155;891.6702,382.979;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;208;739.3197,-606.2744;Float;False;Property;_GradientOffset;Gradient Offset;23;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;207;733.4167,-702.0497;Float;False;Property;_GradientScale;Gradient Scale;22;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;275;1074.009,1474.637;Inherit;True;ColorShartSlot;-1;;373;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;278;1073.824,1263.506;Inherit;True;ColorShartSlot;-1;;372;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;274;1063.897,2751.832;Inherit;True;ColorShartSlot;-1;;369;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;281;1066.266,2314.42;Inherit;True;ColorShartSlot;-1;;370;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;280;1072.05,1897.946;Inherit;True;ColorShartSlot;-1;;368;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;277;1068.635,2106.349;Inherit;True;ColorShartSlot;-1;;367;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;276;1067.94,2529.334;Inherit;True;ColorShartSlot;-1;;366;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;206;758.7729,-955.8021;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;273;1065.782,2963.45;Inherit;True;ColorShartSlot;-1;;371;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;3;False;8;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;253;745.5572,-529.9987;Float;False;Property;_GradientPower;Gradient Power;24;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;209;1085.605,-947.4406;Inherit;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;289;1162.865,-645.6829;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;260;1506.911,1450.623;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;262;1509.151,1956.105;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;261;1513.617,1678.717;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;263;1761.779,1591.684;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;254;1397.934,-946.894;Inherit;True;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;294;1407.098,-640.0005;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;292;1690.576,-948.1953;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;286;2204.463,687.576;Inherit;True;False;False;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;285;2194.846,573.1987;Inherit;False;Property;_EmissionPower;Emission Power;18;0;Create;True;0;0;False;1;Header(Emmision);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;255;1957.11,-951.1956;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;2523.534,562.9523;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;291;1651.518,502.5378;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;283;2193.479,270.7187;Inherit;True;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;284;2862.686,251.0539;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;2835.367,473.9925;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;290;2187.83,1603.08;Inherit;True;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;2358.615,-332.7438;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;282;2187.501,72.77197;Inherit;True;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3622.263,125.6268;Float;False;True;-1;4;;0;0;Standard;Malbers/Color3x3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;202;1;258;0
WireConnection;232;38;181;0
WireConnection;238;38;152;0
WireConnection;257;38;256;0
WireConnection;231;38;156;0
WireConnection;239;38;23;0
WireConnection;235;38;183;0
WireConnection;204;0;201;0
WireConnection;203;0;202;0
WireConnection;203;1;200;0
WireConnection;240;38;157;0
WireConnection;236;38;150;0
WireConnection;233;38;159;0
WireConnection;205;0;203;0
WireConnection;205;1;204;0
WireConnection;146;0;239;0
WireConnection;146;1;236;0
WireConnection;146;2;238;0
WireConnection;164;0;233;0
WireConnection;164;1;231;0
WireConnection;164;2;240;0
WireConnection;193;0;235;0
WireConnection;193;1;232;0
WireConnection;193;2;257;0
WireConnection;279;38;269;0
WireConnection;155;0;146;0
WireConnection;155;1;164;0
WireConnection;155;2;193;0
WireConnection;275;38;264;0
WireConnection;278;38;268;0
WireConnection;274;38;271;0
WireConnection;281;38;265;0
WireConnection;280;38;267;0
WireConnection;277;38;266;0
WireConnection;276;38;270;0
WireConnection;206;0;205;0
WireConnection;273;38;272;0
WireConnection;209;0;206;0
WireConnection;209;1;207;0
WireConnection;209;2;208;0
WireConnection;289;0;155;0
WireConnection;260;0;278;0
WireConnection;260;1;275;0
WireConnection;260;2;279;0
WireConnection;262;0;276;0
WireConnection;262;1;274;0
WireConnection;262;2;273;0
WireConnection;261;0;280;0
WireConnection;261;1;277;0
WireConnection;261;2;281;0
WireConnection;263;0;260;0
WireConnection;263;1;261;0
WireConnection;263;2;262;0
WireConnection;254;0;209;0
WireConnection;254;1;253;0
WireConnection;294;0;289;0
WireConnection;292;0;254;0
WireConnection;292;1;294;0
WireConnection;286;0;263;0
WireConnection;255;0;292;0
WireConnection;287;0;285;0
WireConnection;287;1;286;0
WireConnection;291;0;155;0
WireConnection;283;0;263;0
WireConnection;284;0;283;0
WireConnection;288;0;291;0
WireConnection;288;1;287;0
WireConnection;290;0;263;0
WireConnection;210;0;155;0
WireConnection;210;1;255;0
WireConnection;282;0;263;0
WireConnection;0;0;210;0
WireConnection;0;2;288;0
WireConnection;0;3;282;0
WireConnection;0;4;284;0
ASEEND*/
//CHKSM=A389C007DA4FA74D88A09BDAA53A2205CDAA36A2