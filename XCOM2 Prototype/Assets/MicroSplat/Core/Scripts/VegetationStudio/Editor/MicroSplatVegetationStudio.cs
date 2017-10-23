//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   #if __MICROSPLAT__ && VEGETATION_STUDIO
   [InitializeOnLoad]
   public class MicroSplatVegetationStudio : FeatureDescriptor
   {
      public override string ModuleName()
      {
         return "Vegetation Studio";
      }

      public enum DefineFeature
      {
         _VSGRASSMAP,
         kNumFeatures,
      }
 
      public bool grassMap;


      public TextAsset properties;
      public TextAsset function;

      GUIContent CShaderGrassMap = new GUIContent("Vegetation Studio GrassMap", "Enable texturing of distant grasses");

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
         return "1.3";
      }

      public override void DrawFeatureGUI(Material mat)
      {
         grassMap = EditorGUILayout.Toggle(CShaderGrassMap, grassMap);
      }

      GUIContent CShaderTint = new GUIContent("Grass Mask Tint", "Tint the grass overlay color, or reduce it's overall effect with the alpha");
      public override void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (grassMap && MicroSplatUtilities.DrawRollup("Vegetation Studio"))
         {
            if (mat.HasProperty("_VSTint"))
            {
               EditorGUI.BeginChangeCheck();
               var c = mat.GetColor("_VSTint");
              // c = c.gamma;
               c = EditorGUILayout.ColorField(CShaderTint, c);
              // c = c.linear;
               if (EditorGUI.EndChangeCheck())
               {
                  mat.SetColor("_VSTint", c);
                  EditorUtility.SetDirty(mat);
               }
            }
         }
      }


      public override void DrawPerTextureGUI(int index, Material mat, MicroSplatPropData propData)
      {
         
      }

      public override void InitCompiler(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_properties_vegetationstudio.txt"))
            {
               properties = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_func_vegetationstudio.txt"))
            {
               function = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
             
         }
      } 

      public override void WriteProperties(string[] features, System.Text.StringBuilder sb)
      {
         if (grassMap)
         {
            sb.Append(properties.text);
         }

      }

      public override void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
         if (grassMap)
         {
            textureSampleCount++;
         }
      }

      public override string[] Pack()
      {
         List<string> features = new List<string>();
         if (grassMap)
         {
            features.Add(GetFeatureName(DefineFeature._VSGRASSMAP));
         }

         return features.ToArray();
      }

      public override void WriteFunctions(System.Text.StringBuilder sb)
      {
         if (grassMap)
         {
            sb.AppendLine(function.text);
         }

      }

      public override void Unpack(string[] keywords)
      {
         grassMap = HasFeature(keywords, DefineFeature._VSGRASSMAP);
      }

   }   
   #endif


}