//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   [InitializeOnLoad]
   public class MicroSplatBaseFeatures : FeatureDescriptor
   {
      public override int DisplaySortOrder()
      {
         return -1000;
      }

      public override string ModuleName()
      {
         return "Core";
      }

      public enum DefineFeature
      {
         _MICROSPLAT = 0,
         _MAX3LAYER,
         _MAX2LAYER,
         _MAX4TEXTURES,
         _MAX8TEXTURES,
         _MAX12TEXTURES,
         _PERTEXTINT,
         _PERTEXBRIGHTNESS,
         _PERTEXCONTRAST,
         _PERTEXAOSTR,
         _PERTEXNORMSTR,
         _PERTEXSMOOTHSTR,
         _PERTEXMETALLIC,
         _PERTEXUVSCALEOFFSET, 
         _PERTEXINTERPCONTRAST,
         _BDRF1,
         _BDRF2,
         _BDRF3,
         _DISABLEHEIGHTBLENDING,
         _WORLDUV,
         _DEBUG_OUTPUT_ALBEDO,
         _DEBUG_OUTPUT_HEIGHT,
         _DEBUG_OUTPUT_NORMAL,
         _DEBUG_OUTPUT_METAL,
         _DEBUG_OUTPUT_SMOOTHNESS,
         _DEBUG_OUTPUT_AO,
         _DEBUG_OUTPUT_EMISSION,
         kNumFeatures,
      }
         

      public enum MaxTextureCount
      {
         Four = 4,
         Eight = 8,
         Twelve = 12,
         Sixteen = 16,
      }

      public enum PerformanceMode
      {
         BestQuality,
         Balanced,
         Fastest
      }
         
      public enum UVMode
      {
         UV, 
         WorldSpace
      }

      public enum LightingMode
      {
         Automatic = 0,
         StandardShader,
         Simplified,
         BlinnPhong
      }

      public enum DebugOutput
      {
         None = 0,
         Albedo,
         Height,
         Normal,
         Metallic,
         Smoothness,
         AO,
         Emission,
      }

      // state for the shader generation
      public PerformanceMode perfMode = PerformanceMode.BestQuality;
      public MaxTextureCount maxTextureCount = MaxTextureCount.Sixteen;
      public bool perTexTint;
      public bool perTexBrightness;
      public bool perTexContrast;
      public bool perTexAOStr;
      public bool perTexNormStr;
      public bool perTexSmoothStr;
      public bool perTexMetallic;
      public bool perTexUVScale;
      public bool perTexInterpContrast;
      public bool disableHeightBlend;
      public UVMode uvMode = UVMode.UV;

      public LightingMode lightingMode;
      public DebugOutput debugOutput = DebugOutput.None;

      // files to include
      static TextAsset properties_splat;


      GUIContent CInterpContrast = new GUIContent("Interpolation Contrast", "Controls how much hight map based blending is used");
      GUIContent CShaderPerfMode = new GUIContent("Blend Quality", "Can be used to reduce the number of textures blended to increase speed");
      GUIContent CMaxTexCount = new GUIContent("Max Texture Count", "How many textures your terrain is allowed to use");
      GUIContent CLightingMode = new GUIContent("Lighting Model", "Override Unity's automatic selection of a BDRF function to a fixed one. This will force the shader to render in forward rendering mode");
      GUIContent CDisableHeightBlend = new GUIContent("Disable Height Blending", "Disables height based blending, which can be a speed boost on low end platforms");
      GUIContent CUVMode = new GUIContent("UV Mode", "Mode for Splat UV coordinates");
      //GUIContent CShaderDebugOutput = new GUIContent("Debug Output", "Used by Render Baking, but useful when you want to debug the output of the shader. Allows you to see the different channels (albedo, normal, ao, etc) instead of the final result");

      // Can we template these somehow?
      public static string GetFeatureName(DefineFeature feature)
      {
         return System.Enum.GetName(typeof(DefineFeature), feature);
      }

      public static bool HasFeature(string[] keywords, DefineFeature feature)
      {
         string f = GetFeatureName(feature);
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords[i] == f)
               return true;
         }
         return false;
      }
         
      public override string GetVersion()
      {
         return MicroSplatShaderGUI.MicroSplatVersion;
      }

      public override void WriteFunctions(System.Text.StringBuilder sb)
      {

      }

      public override void DrawFeatureGUI(Material mat)
      {
         perfMode = (PerformanceMode)EditorGUILayout.EnumPopup(CShaderPerfMode, perfMode);
         maxTextureCount = (MaxTextureCount)EditorGUILayout.EnumPopup(CMaxTexCount, maxTextureCount);
         lightingMode = (LightingMode)EditorGUILayout.EnumPopup(CLightingMode, lightingMode);
         uvMode = (UVMode)EditorGUILayout.EnumPopup(CUVMode, uvMode);
         disableHeightBlend = EditorGUILayout.Toggle(CDisableHeightBlend, disableHeightBlend);
         //debugOutput = (DebugOutput)EditorGUILayout.EnumPopup(CShaderDebugOutput, debugOutput);
      }

      static GUIContent CAlbedoTex = new GUIContent("Albedo/Height Array", "Texture Array which contains albedo and height information");
      static GUIContent CNoramlSpec = new GUIContent("Normal/Smooth/AO Array", "Texture Array with normal, smoothness, and ambient occlusion");

      public override void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (MicroSplatUtilities.DrawRollup("Splats"))
         {
            var albedoMap = shaderGUI.FindProp("_Diffuse", props);
            var normalMap = shaderGUI.FindProp("_NormalSAO", props);
            materialEditor.TexturePropertySingleLine(CAlbedoTex, albedoMap);
            materialEditor.TexturePropertySingleLine(CNoramlSpec, normalMap);

            if (!disableHeightBlend)
            {
               var contrastProp = shaderGUI.FindProp("_Contrast", props);
               contrastProp.floatValue = EditorGUILayout.Slider(CInterpContrast, contrastProp.floatValue, 1.0f, 0.0001f);
            }
            if (!mat.IsKeywordEnabled("_TRIPLANAR"))
            {
               EditorGUI.BeginChangeCheck();
               Vector4 uvScale = shaderGUI.FindProp("_UVScale", props).vectorValue;
               Vector2 scl = new Vector2(uvScale.x, uvScale.y);
               Vector2 offset = new Vector2(uvScale.z, uvScale.w);
               scl = EditorGUILayout.Vector2Field("Global UV Scale", scl);
               offset = EditorGUILayout.Vector2Field("Global UV Offset", offset);
               if (EditorGUI.EndChangeCheck())
               {
                  uvScale.x = scl.x;
                  uvScale.y = scl.y;
                  uvScale.z = offset.x;
                  uvScale.w = offset.y;
                  shaderGUI.FindProp("_UVScale", props).vectorValue = uvScale;
                  EditorUtility.SetDirty(mat);
               }
            }

         }
      }

      public override string[] Pack()
      {
         List<string> features = new List<string>();
         features.Add(GetFeatureName(DefineFeature._MICROSPLAT));



         if (perfMode == PerformanceMode.Balanced)
         {
            features.Add(GetFeatureName(DefineFeature._MAX3LAYER));
         }
         else if (perfMode == PerformanceMode.Fastest)
         {
            features.Add(GetFeatureName(DefineFeature._MAX2LAYER));
         }
         if (disableHeightBlend)
         {
            features.Add(GetFeatureName(DefineFeature._DISABLEHEIGHTBLENDING));
         }
         if (maxTextureCount == MaxTextureCount.Four)
         {
            features.Add(GetFeatureName(DefineFeature._MAX4TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.Eight)
         {
            features.Add(GetFeatureName(DefineFeature._MAX8TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.Twelve)
         {
            features.Add(GetFeatureName(DefineFeature._MAX12TEXTURES));
         }

         if (lightingMode == LightingMode.StandardShader)
         {
            features.Add(GetFeatureName(DefineFeature._BDRF1));
         }
         else if (lightingMode == LightingMode.Simplified)
         {
            features.Add(GetFeatureName(DefineFeature._BDRF2));
         }
         else if (lightingMode == LightingMode.BlinnPhong)
         {
            features.Add(GetFeatureName(DefineFeature._BDRF3));
         }

         if (perTexUVScale)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXUVSCALEOFFSET));
         }

         if (uvMode == UVMode.WorldSpace)
         {
            features.Add(GetFeatureName(DefineFeature._WORLDUV));
         }

         if (perTexInterpContrast)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXINTERPCONTRAST));
         }
         if (perTexTint)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXTINT));
         }
         if (perTexBrightness)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXBRIGHTNESS));
         }
         if (perTexContrast)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXCONTRAST));
         }
         if (perTexAOStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXAOSTR));
         }
         if (perTexNormStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXNORMSTR));
         }
         if (perTexSmoothStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXSMOOTHSTR));
         }
         if (perTexMetallic)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXMETALLIC));
         }

         if (debugOutput != DebugOutput.None)
         {
            if (debugOutput == DebugOutput.Albedo)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_ALBEDO));
            }
            else if (debugOutput == DebugOutput.Height)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_HEIGHT));
            }
            else if (debugOutput == DebugOutput.Normal)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_NORMAL));
            }
            else if (debugOutput == DebugOutput.Metallic)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_METAL));
            }
            else if (debugOutput == DebugOutput.Smoothness)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SMOOTHNESS));
            }
            else if (debugOutput == DebugOutput.AO)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_AO));
            }
            else if (debugOutput == DebugOutput.Emission)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_EMISSION));
            }
         }
         return features.ToArray();
      }

      public override void Unpack(string[] keywords)
      {
         if (HasFeature(keywords, DefineFeature._MAX2LAYER))
         {
            perfMode = PerformanceMode.Fastest;
         }
         else if (HasFeature(keywords, DefineFeature._MAX3LAYER))
         {
            perfMode = PerformanceMode.Balanced;
         }
         else
         {
            perfMode = PerformanceMode.BestQuality;
         }

         uvMode = HasFeature(keywords, DefineFeature._WORLDUV) ? UVMode.WorldSpace : UVMode.UV;

         if (HasFeature(keywords, DefineFeature._MAX4TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Four;
         }
         else if (HasFeature(keywords, DefineFeature._MAX8TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Eight;
         }
         else if (HasFeature(keywords, DefineFeature._MAX12TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Twelve;
         }
         else
         {
            maxTextureCount = MaxTextureCount.Sixteen;
         }

         disableHeightBlend = HasFeature(keywords, DefineFeature._DISABLEHEIGHTBLENDING);

         lightingMode = LightingMode.Automatic;
         if (HasFeature(keywords, DefineFeature._BDRF1))
         {
            lightingMode = LightingMode.StandardShader;
         }
         else if (HasFeature(keywords, DefineFeature._BDRF2))
         {
            lightingMode = LightingMode.Simplified;
         }
         else if (HasFeature(keywords, DefineFeature._BDRF3))
         {
            lightingMode = LightingMode.BlinnPhong;
         }
            
         perTexUVScale = (HasFeature(keywords, DefineFeature._PERTEXUVSCALEOFFSET));
         perTexInterpContrast = HasFeature(keywords, DefineFeature._PERTEXINTERPCONTRAST);
         perTexBrightness = HasFeature(keywords, DefineFeature._PERTEXBRIGHTNESS);
         perTexContrast = HasFeature(keywords, DefineFeature._PERTEXCONTRAST);
         perTexAOStr = (HasFeature(keywords, DefineFeature._PERTEXAOSTR));
         perTexMetallic = (HasFeature(keywords, DefineFeature._PERTEXMETALLIC));
         perTexNormStr = (HasFeature(keywords, DefineFeature._PERTEXNORMSTR));
         perTexSmoothStr = (HasFeature(keywords, DefineFeature._PERTEXSMOOTHSTR));
         perTexTint = (HasFeature(keywords, DefineFeature._PERTEXTINT));

         debugOutput = DebugOutput.None;
         if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_ALBEDO))
         {
            debugOutput = DebugOutput.Albedo;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_HEIGHT))
         {
            debugOutput = DebugOutput.Height;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_NORMAL))
         {
            debugOutput = DebugOutput.Normal;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SMOOTHNESS))
         {
            debugOutput = DebugOutput.Smoothness;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_METAL))
         {
            debugOutput = DebugOutput.Metallic;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_AO))
         {
            debugOutput = DebugOutput.AO;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_EMISSION))
         {
            debugOutput = DebugOutput.Emission;
         }
      }

      public override void InitCompiler(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_properties_splat.txt"))
            {
               properties_splat = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }

      }

      public override void WriteProperties(string[] features, System.Text.StringBuilder sb)
      {
         sb.AppendLine(properties_splat.text);
      }

      public override void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
         textureSampleCount += 4; // control textures
         if (perfMode == PerformanceMode.BestQuality)
         {
            arraySampleCount += 8;
         }
         else if (perfMode == PerformanceMode.Balanced)
         {
            arraySampleCount += 6;
         }
         else if (perfMode == PerformanceMode.Fastest)
         {
            arraySampleCount += 4;
         }
      }

      static GUIContent CPerTexUV = new GUIContent("UV Scale", "UV Scale for the texture");
      static GUIContent CPerTexUVOffset = new GUIContent("UV Offset", "UV Offset for each texture");
      static GUIContent CPerTexInterp = new GUIContent("Interpolation Contrast", "Control blend of sharpness vs other textures");
      static GUIContent CPerTexTint = new GUIContent("Tint", "Tint color for albedo");
      static GUIContent CPerTexNormStr = new GUIContent("Normal Strength", "Increase or decrease strength of normal mapping");
      static GUIContent CPerTexAOStr = new GUIContent("AO Strength", "Increase or decrease strength of the AO map");
      static GUIContent CPerTexSmoothStr = new GUIContent("Smoothness Strength", "Increase or decrease strength of the smoothness");
      static GUIContent CPerTexMetallic = new GUIContent("Metallic", "Set the metallic value of the texture");
      static GUIContent CPerTexBrightness = new GUIContent("Brightness", "Brightness of texture");
      static GUIContent CPerTexContrast = new GUIContent("Contrast", "Contrast of texture");


      public override void DrawPerTextureGUI(int index, Material mat, MicroSplatPropData propData)
      {
         InitPropData(0, propData, new Color(1.0f, 1.0f, 0.0f, 0.0f)); // uvscale2, uvOffset
         InitPropData(1, propData, new Color(1.0f, 1.0f, 1.0f, 0.0f)); // tint, interp contrast
         InitPropData(2, propData, new Color(1.0f, 0.0f, 1.0f, 0.0f)); // norm str, smooth str, ao str, metal values
         InitPropData(3, propData, new Color(0.0f, 1.0f, 0.4f, 1.0f)); // brightness, contrast, porosity, foam

         if (perTexUVScale)
         {
            EditorGUILayout.HelpBox("It is best to limit per-texture UVs between the 0.5 and 2.0 range. Going below 0.5 may cause textures to be blury, or above 2.0 may cause them to alias. See documentation for more info.", MessageType.Info);
         }

         perTexUVScale = DrawPerTexVector2Vector2(index, 0, GetFeatureName(DefineFeature._PERTEXUVSCALEOFFSET), 
            mat, propData, CPerTexUV, CPerTexUVOffset);

         if (!disableHeightBlend)
         {
            perTexInterpContrast = DrawPerTexFloatSlider(index, 1, GetFeatureName(DefineFeature._PERTEXINTERPCONTRAST), 
               mat, propData, Channel.A, CPerTexInterp, -1.0f, 1.0f);
         }

         perTexTint = DrawPerTexColor(index, 1, GetFeatureName(DefineFeature._PERTEXTINT), 
            mat, propData, CPerTexTint, false);

         perTexBrightness = DrawPerTexFloatSlider(index, 3, GetFeatureName(DefineFeature._PERTEXBRIGHTNESS), 
            mat, propData, Channel.R, CPerTexBrightness, -1.0f, 1.0f);

         perTexContrast = DrawPerTexFloatSlider(index, 3, GetFeatureName(DefineFeature._PERTEXCONTRAST), 
            mat, propData, Channel.G, CPerTexContrast, 0.1f, 3.0f);

         perTexNormStr = DrawPerTexFloatSlider(index, 2, GetFeatureName(DefineFeature._PERTEXNORMSTR), 
            mat, propData, Channel.R, CPerTexNormStr, 0.0f, 3.0f);
         
         perTexSmoothStr = DrawPerTexFloatSlider(index, 2, GetFeatureName(DefineFeature._PERTEXSMOOTHSTR), 
            mat, propData, Channel.G, CPerTexSmoothStr, -1.0f, 1.0f);

         perTexAOStr = DrawPerTexFloatSlider(index, 2, GetFeatureName(DefineFeature._PERTEXAOSTR), 
            mat, propData, Channel.B, CPerTexAOStr, 0.5f, 3.0f);

         perTexMetallic = DrawPerTexFloatSlider(index, 2, GetFeatureName(DefineFeature._PERTEXMETALLIC), 
            mat, propData, Channel.A, CPerTexMetallic, 0, 1);


      }
   }   


}