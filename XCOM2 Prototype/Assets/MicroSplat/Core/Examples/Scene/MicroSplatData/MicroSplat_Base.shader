//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//
// Auto-generated shader code, don't hand edit!
//   Compiled with MicroSplat 1.1
//   Unity : 5.6.1p2
//   Platform : OSXEditor
//////////////////////////////////////////////////////

Shader "Hidden/MicroSplat/Example_Base-1347635300" {
   Properties {
      
      [HideInInspector] _Control0 ("Control (RGBA)", 2D) = "red" {}
      [HideInInspector] _Control1 ("Control1 (RGBA)", 2D) = "black" {}
      [HideInInspector] _Control2 ("Control2 (RGBA)", 2D) = "black" {}
      [HideInInspector] _Control3 ("Control3 (RGBA)", 2D) = "black" {}
      // Splats
      [NoScaleOffset]_Diffuse ("Diffuse Array", 2DArray) = "white" {}
      [NoScaleOffset]_NormalSAO ("Normal Array", 2DArray) = "bump" {}
      [NoScaleOffset]_PerTexProps("Per Texture Properties", 2D) = "black" {}
      _Contrast("Blend Contrast", Range(0.01, 0.99)) = 0.4
      _UVScale("UV Scales", Vector) = (45, 45, 0, 0)












   }

   CGINCLUDE
   ENDCG

   SubShader {
      Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+100" }
      Cull Back
      ZTest LEqual
      CGPROGRAM
      #pragma exclude_renderers d3d9
      #pragma surface surf Standard vertex:vert fullforwardshadows noforwardadd addshadow

      #pragma target 3.5

      #define _MICROSPLAT 1


      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
      #include "Lighting.cginc"
      #include "UnityPBSLighting.cginc"
      #include "UnityStandardBRDF.cginc"

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      half _Contrast;
      UNITY_DECLARE_TEX2D(_Control0);
      #if !_MAX4TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control1);
      #endif
      #if !_MAX4TEXTURES && !_MAX8TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control2);
      #endif

      #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control3);
      #endif
      sampler2D_half _PerTexProps;
      float2 uv_Control0;

      float4 _UVScale; // scale and offset

      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;
      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
      };


      struct appdata 
      {
         float4 vertex : POSITION;
         float4 tangent : TANGENT;
         float3 normal : NORMAL;
         float2 texcoord : TEXCOORD0;
         float2 texcoord1 : TEXCOORD1;
         float2 texcoord2 : TEXCOORD2;
      };

      struct Input 
      {
         float2 uv_Control0;
         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         #if _TERRAINBLENDING 
         fixed4 color : COLOR;
         #endif
         INTERNAL_DATA
      };

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
      }




      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/16, pixel/16, 0, 0)); \
            varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/16, pixel/16, 0, 0)); \
            varName##2 = tex2Dlod(_PerTexProps, float4(config.uv2.z/16, pixel/16, 0, 0)); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/16, pixel/16, 0, 0)); \
            varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/16, pixel/16, 0, 0)); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/16, pixel/16, 0, 0)); \
            half4 varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/16, pixel/16, 0, 0)); \
            half4 varName##2 = tex2Dlod(_PerTexProps, float4(config.uv2.z/16, pixel/16, 0, 0)); \
            half4 varName##3 = tex2Dlod(_PerTexProps, float4(config.uv3.z/16, pixel/16, 0, 0)); \

      #endif

      // 2 component normal blend?
      half2 BlendNormal2(half2 base, half2 blend) { return normalize(float3(base.xy + blend.xy, 1)).xy; }
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }


      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //const half epsilon = 1.0f / 1024.0f;
             half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);

             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);
             // Normalize weights.
             half weightScale = 1.0f / (weights.x + weights.y + weights.z + weights.w);
             weights *= weightScale;
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #else
         #define TEXCOUNT 16
      #endif

      void Setup(out half4 weights, float2 uv, out Config config, fixed4 w0, fixed4 w1, fixed4 w2, fixed4 w3)
      {
         UNITY_INITIALIZE_OUTPUT(Config,config);
         half4 indexes = 0;



         fixed splats[TEXCOUNT];


         splats[0] = w0.x;
         splats[1] = w0.y;
         splats[2] = w0.z;
         splats[3] = w0.w;
         #if !_MAX4TEXTURES
            splats[4] = w1.x;
            splats[5] = w1.y;
            splats[6] = w1.z;
            splats[7] = w1.w;
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
            splats[8] = w2.x;
            splats[9] = w2.y;
            splats[10] = w2.z;
            splats[11] = w2.w;
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            splats[12] = w3.x;
            splats[13] = w3.y;
            splats[14] = w3.z;
            splats[15] = w3.w;
         #endif


         weights[0] = 0;
         weights[1] = 0;
         weights[2] = 0;
         weights[3] = 0;
         indexes[0] = 0;
         indexes[1] = 0;
         indexes[2] = 0;
         indexes[3] = 0;

         for (int i = 0; i < TEXCOUNT; ++i)
         {
            fixed w = splats[i];
            if (w >= weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = i;
            }
            else if (w >= weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = i;
            }
            else if (w >= weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = i;
            }
            else if (w >= weights[3])
            {
               weights[3] = w;
               indexes[3] = i;
            }
         }

         config.uv = uv;
         float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         config.uv0 = float3(scaledUV, indexes.x);
         config.uv1 = float3(scaledUV, indexes.y);
         config.uv2 = float3(scaledUV, indexes.z);
         config.uv3 = float3(scaledUV, indexes.w);

         #if _MAX2LAYER
         weights.zw = 0;
         #endif
         #if _MAX3LAYER
         weights.w = 0;
         #endif
      }

      inline fixed2 UnpackNormal2(fixed4 packednormal)
      {
         #if defined(UNITY_NO_DXT5nm)
          return packednormal.xy * 2 - 1;
         #else
          return packednormal.wy * 2 - 1;
         #endif
      }

      #define MICROSPLAT_SAMPLE_TEX2DARRAY(tex, u, tc, pN) UNITY_SAMPLE_TEX2DARRAY(tex, u)
      #define MICROSPLAT_SAMPLE_TEX2DARRAY_LOD(tex, u, tc, pN, l) UNITY_SAMPLE_TEX2DARRAY_LOD(tex, u, l)


   
      #if _TRIPLANAR
         #undef MICROSPLAT_SAMPLE_TEX2DARRAY
         #define MICROSPLAT_SAMPLE_TEX2DARRAY(tex, uv, tc, pN) (UNITY_SAMPLE_TEX2DARRAY(tex, tc[0]) * pN.x + UNITY_SAMPLE_TEX2DARRAY(tex, tc[1]) * pN.y + UNITY_SAMPLE_TEX2DARRAY(tex, tc[2]) * pN.z)
      #undef MICROSPLAT_SAMPLE_TEX2DARRAY_LOD
         #define MICROSPLAT_SAMPLE_TEX2DARRAY_LOD(tex, uv, tc, pN, l) (UNITY_SAMPLE_TEX2DARRAY_LOD(tex, tc[0], l) * pN.x + UNITY_SAMPLE_TEX2DARRAY_LOD(tex, tc[1], l) * pN.y + UNITY_SAMPLE_TEX2DARRAY_LOD(tex, tc[2], l) * pN.z)
      #endif

      // man I wish unity would wrap everything instead of only what they use. Just seems like a landmine for
      // people like myself..
      #if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL)
         #define MICROSPLAT_SAMPLE_TEX2D_LOD(tex,coord, lod) tex.SampleLevel (sampler##tex,coord, lod)
         #define MICROSPLAT_SAMPLE_TEX2D_SAMPLER_LOD(tex,samplertex,coord, lod) tex.SampleLevel (sampler##samplertex,coord, lod)
      #else
         #define MICROSPLAT_SAMPLE_TEX2D_LOD(tex,coord,lod) tex2D (tex,coord,lod)
         #define MICROSPLAT_SAMPLE_TEX2D_SAMPLER_LOD(tex,samplertex,coord,lod) tex2D (tex,coord,lod)
      #endif



      
      // surface shaders + tessellation, do not pass go, or
      // collect $500 - sucks it up and realize you can't use
      // an Input struct, so you have to hack UV coordinates
      // and live with only the magic keywords..
      void vert (inout appdata i) 
      {
         #if _TRIPLANAR
         half3 normal  = UnityObjectToWorldNormal(i.normal);
         float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
         float3 sgn = sign(normal);
         half3 tnorm = max(pow(abs(normal), 10), 0.0001);
         tnorm /= dot(tnorm, half3(1,1,1));

         i.tangent.xyz = cross(i.normal, mul(unity_WorldToObject, fixed4(0, sgn.x, 0, 0)).xyz * tnorm.x)
                 + cross(i.normal, mul(unity_WorldToObject, fixed4(0, 0, sgn.y, 0)).xyz * tnorm.y)
                 + cross(i.normal, mul(unity_WorldToObject, fixed4(0, sgn.z, 0, 0)).xyz * tnorm.z);
         i.tangent.w = -1;
         #else
         float4 tangent;
         tangent.xyz = cross(UnityObjectToWorldNormal( i.normal ), float3(0,0,1));
         tangent.w = -1;
         i.tangent = tangent;
         #endif

      }

      void SampleAlbedo(Config config, TriplanarConfig tc, inout RawSamples s)
      {
         s.albedo0 = MICROSPLAT_SAMPLE_TEX2DARRAY(_Diffuse, config.uv0, tc.uv0, tc.pN);
         s.albedo1 = MICROSPLAT_SAMPLE_TEX2DARRAY(_Diffuse, config.uv1, tc.uv1, tc.pN);
         #if !_MAX2LAYER
         s.albedo2 = MICROSPLAT_SAMPLE_TEX2DARRAY(_Diffuse, config.uv2, tc.uv2, tc.pN); 
         #endif
         #if !_MAX3LAYER || !_MAX2LAYER
         s.albedo3 = MICROSPLAT_SAMPLE_TEX2DARRAY(_Diffuse, config.uv3, tc.uv3, tc.pN);
         #endif
      }

      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s)
      {
         s.normSAO0 = MICROSPLAT_SAMPLE_TEX2DARRAY(_NormalSAO, config.uv0, tc.uv0, tc.pN).garb;
         s.normSAO1 = MICROSPLAT_SAMPLE_TEX2DARRAY(_NormalSAO, config.uv1, tc.uv1, tc.pN).garb;
         s.normSAO0.xy = s.normSAO0.xy * 2 - 1;
         s.normSAO1.xy = s.normSAO1.xy * 2 - 1;
         #if !_MAX2LAYER
         s.normSAO2 = MICROSPLAT_SAMPLE_TEX2DARRAY(_NormalSAO, config.uv2, tc.uv2, tc.pN).garb;
         s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
         #endif
         #if !_MAX3LAYER || !_MAX2LAYER
         s.normSAO3 = MICROSPLAT_SAMPLE_TEX2DARRAY(_NormalSAO, config.uv3, tc.uv3, tc.pN).garb;
         s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
         #endif
      }


      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex)
      {
         half4 fxLevels = half4(0,0,0,0);
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         half burnLevel = 0;
         half wetLevel = 0;
         fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         #if _TRIPLANAR
         PrepTriplanar(worldNormalVertex, i.worldPos, config, tc, weights);
         #endif



         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif
         #endif




         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         SampleAlbedo(config, tc, samples);

         #if _STREAMS || _PARALLAX
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         half3 waterNormalFoam = half3(0, 0, 0);
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX
            DoParallax(i, earlyHeight, config, tc, samples, weights);
         #endif



         // Blend results
         #if _PERTEXINTERPCONTRAST
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = contrast;
            contrast = clamp(contrast + _Contrast, 0.0001, 1.2); 
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, contrast);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples);
         #endif

         SampleNormal(config, tc, samples);


         #if _DISTANCERESAMPLE
         DistanceResample(samples, config, tc, camDist, fxLevels);
         #endif

         // PerTexture sampling goes here, passing the samples structure

         #if _PERTEXTINT
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif

         half porosity = 0.4;
         float streamFoam = 1.0f;

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = ((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5;
               samples.albedo1.rgb = ((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5;
               #if !_MAX2LAYER
                 samples.albedo2.rgb = ((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = ((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5;
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb += ptBC0.r;
               samples.albedo1.rgb += ptBC1.r;
               #if !_MAX2LAYER
                  samples.albedo2.rgb += ptBC2.r;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb += ptBC3.r;
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if _PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR

            samples.normSAO0.xy *= perTexMatSettings0.r;
            samples.normSAO1.xy *= perTexMatSettings1.r;
            #if !_MAX2LAYER
               samples.normSAO2.xy *= perTexMatSettings2.r;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR
            samples.normSAO0.a = pow(samples.normSAO0.a, perTexMatSettings0.b);
            samples.normSAO1.a = pow(samples.normSAO1.a, perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(samples.normSAO2.a, perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(samples.normSAO3.a, perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         #if ((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)
         ApplyDetailDistanceNoisePerTex(samples, config, camDist);
         #endif

         #if _GEOMAP && _PERTEXGEO
         GeoTexturePerTex(samples, i.worldPos, config);
         #endif

         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH
         GlobalTintTexturePerTex(samples, config, camDist);
         #endif

         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH
         GlobalNormalTexturePerTex(samples, config, camDist);
         #endif



         #if _PERTEXMETALLIC
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         // Blend em..
         half4 albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
         half4 normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);
 
         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
         ApplyDetailDistanceNoise(albedo.rgb, normSAO, config, camDist);
         #endif

         #if _GEOMAP && !_PERTEXGEO
         GeoTexture(albedo.rgb, i.worldPos, config);
         #endif

         #if _GLOBALTINT && !_PERTEXTINTSTRENGTH
         GlobalTintTexture(albedo.rgb, config, camDist);
         #endif

         #if _VSGRASSMAP
         VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMAL
         GlobalNormalTexture(normSAO, config, camDist);
         #endif

         o.Albedo = albedo.rgb;
         o.Height = albedo.a;
         o.Normal = half3(normSAO.xy, 1);
         o.Smoothness = normSAO.b;
         o.Occlusion = normSAO.a;

         half pud = 0;


         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         #if _SNOW
         DoSnow(o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, config, weights);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         return o;
      }

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _ALPHABELOWHEIGHT
         ClipWaterLevel(i.worldPos);
         #endif
         float2 origUV = i.uv_Control0;
         half4 weights;

         Config config;
         fixed4 w0 = UNITY_SAMPLE_TEX2D(_Control0, origUV);
         fixed4 w1 = 0; fixed4 w2 = 0; fixed4 w3 = 0;

         #if !_MAX4TEXTURES
         w1 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control1, _Control0, origUV);
         #endif

         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         w2 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control2, _Control0, origUV);
         #endif

         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         w3 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control3, _Control0, origUV);
         #endif

         Setup(weights, origUV, config, w0, w1, w2, w3);
         #if _ALPHAHOLE
         ClipAlphaHole(config.uv0.z);
         #endif



         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex);



         return l;

      }


      void surf (Input i, inout SurfaceOutputStandard o) 
      {
         o.Normal = float3(0,0,1);
         float3 worldNormalVertex = float3(0,1,0);
         #if _SNOW || _TRIPLANAR || _STREAMS || _LAVA
         worldNormalVertex = WorldNormalVector(i, float3(0,0,1));
         #endif

         MicroSplatLayer l = SurfImpl(i, worldNormalVertex);

         // always write to o.Normal to keep i.viewDir consistent
         o.Normal = half3(0, 0, 1);

         #if _DEBUG_OUTPUT_ALBEDO
            o.Albedo = l.Albedo;
         #elif _DEBUG_OUTPUT_NORMAL
            o.Albedo = l.Normal * 0.5 + 0.5;
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            o.Albedo = l.Smoothness.xxx;
         #elif _DEBUG_OUTPUT_METAL
            o.Albedo = l.Metallic.xxx;
         #elif _DEBUG_OUTPUT_AO
            o.Albedo = l.Occlusion.xxx;
         #elif _DEBUG_OUTPUT_EMISSION
            o.Albedo = l.Emission;
         #elif _DEBUG_OUTPUT_HEIGHT
            o.Albedo = l.Height.xxx;
         #else
            o.Albedo = l.Albedo;
            o.Normal = l.Normal;
            o.Smoothness = l.Smoothness;
            o.Metallic = l.Metallic;
            o.Occlusion = l.Occlusion;
            o.Emission = l.Emission;
         #endif

      }

      half4 LightingUnlit(SurfaceOutputStandard s, half3 lightDir, half atten)
      {
         return half4(s.Albedo, 1);
      }


   
ENDCG

   }
}