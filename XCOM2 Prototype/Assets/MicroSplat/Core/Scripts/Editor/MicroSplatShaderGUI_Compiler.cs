//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using JBooth.MicroSplat;
using System.Collections.Generic;
using System.Linq;

public partial class MicroSplatShaderGUI : ShaderGUI 
{
   static TextAsset terrainBody;
   static TextAsset sharedInc;
   static TextAsset terrainBlendBody;

   const string declareTerrain        = "      #pragma surface surf Standard vertex:vert fullforwardshadows noforwardadd addshadow";
   const string declareTerrainDebug   = "      #pragma surface surf Unlit vertex:vert nofog";
   const string declareTerrainTess    = "      #pragma surface surf Standard vertex:disp tessellate:TessDistance fullforwardshadows noforwardadd addshadow";

   const string declareBlend        = "      #pragma surface blendSurf TerrainBlendable fullforwardshadows noforwardadd addshadow decal:blend";


   [MenuItem ("Assets/Create/Shader/MicroSplat Shader")]
   static void NewShader2()
   {
      NewShader();
   }

   [MenuItem ("Assets/Create/MicroSplat/MicroSplat Shader")]
   public static Shader NewShader()
   {
      string path = "Assets";
      foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
      {
         path = AssetDatabase.GetAssetPath(obj);
         if (System.IO.File.Exists(path))
         {
            path = System.IO.Path.GetDirectoryName(path);
         }
         break;
      }
      path = path.Replace("\\", "/");
      path = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplat.shader");
      string name = path.Substring(path.LastIndexOf("/"));
      name = name.Substring(0, name.IndexOf("."));
      MicroSplatCompiler compiler = new MicroSplatCompiler();
      compiler.Init();
      string ret = compiler.Compile(new string[0], name);
      System.IO.File.WriteAllText(path, ret);
      AssetDatabase.Refresh();
      return AssetDatabase.LoadAssetAtPath<Shader>(path);
   }

   public static Material NewShaderAndMaterial(Terrain t)
   {
      string path = MicroSplatUtilities.RelativePathFromAsset(t.terrainData);
      string shaderPath = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplat.shader");
      string shaderBasePath = shaderPath.Replace(".shader", "_Base.shader");
      string matPath = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplat.mat");

      string name = t.name;

      MicroSplatCompiler compiler = new MicroSplatCompiler();
      compiler.Init();

      string baseName = "Hidden/MicroSplat/" + name + "_Base";

      string baseShader = compiler.Compile(new string[0], baseName);
      string regularShader = compiler.Compile(new string[0], name, baseName);
      System.IO.File.WriteAllText(shaderPath, regularShader);
      System.IO.File.WriteAllText(shaderBasePath, baseShader);
      AssetDatabase.Refresh();
      Shader s = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);



      Material m = new Material(s);
      AssetDatabase.CreateAsset(m, matPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      return AssetDatabase.LoadAssetAtPath<Material>(matPath);
   }

   public class MicroSplatCompiler
   {
      public List<FeatureDescriptor> extensions = new List<FeatureDescriptor>();

      public string GetShaderModel()
      {
         for (int i = 0; i < extensions.Count; ++i)
         {
            if (extensions[i].RequiresShaderModel46())
            {
               return "4.6";
            }
         }
         return "3.5";
      }

      public void Init()
      {
         if (terrainBody == null || extensions.Count == 0)
         {
            string[] paths = AssetDatabase.FindAssets("microsplat_ t:TextAsset");
            for (int i = 0; i < paths.Length; ++i)
            {
               paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }

            for (int i = 0; i < paths.Length; ++i)
            {
               var p = paths[i];

               if (p.EndsWith("microsplat_terrain_body.txt"))
               {
                  terrainBody = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
               }
               if (p.EndsWith("microsplat_shared.txt"))
               {
                  sharedInc = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
               }
               if (p.EndsWith("microsplat_terrainblend_body.txt"))
               {
                  terrainBlendBody = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
               }
            }

            // init extensions
            var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var possible = (from System.Type type in types
                                 where type.IsSubclassOf(typeof(FeatureDescriptor))
                                 select type).ToArray();

            for (int i = 0; i < possible.Length; ++i)
            {
               var typ = possible[i];
               FeatureDescriptor ext = System.Activator.CreateInstance(typ) as FeatureDescriptor;
               ext.InitCompiler(paths);
               extensions.Add(ext);
            }
            extensions.Sort(delegate(FeatureDescriptor p1, FeatureDescriptor p2)
            {
               if (p1.DisplaySortOrder() != 0 || p2.DisplaySortOrder() != 0)
               {
                  return p1.DisplaySortOrder().CompareTo(p2.DisplaySortOrder());
               }
               return p1.GetType().Name.CompareTo(p2.GetType().Name);
            });

         }
      }

      void WriteHeader(string[] features, StringBuilder sb, bool blend)
      {
         sb.AppendLine();
         sb.AppendLine("   CGINCLUDE");

         if (features.Contains<string>("_BDRF1") || features.Contains<string>("_BDRF2") || features.Contains<string>("_BDRF3"))
         {
            if (features.Contains<string>("_BDRF1"))
            {
               sb.AppendLine("      #define UNITY_BRDF_PBS BRDF1_Unity_PBS");
            }
            else if (features.Contains<string>("_BDRF2"))
            {
               sb.AppendLine("      #define UNITY_BRDF_PBS BRDF2_Unity_PBS");
            }
            else if (features.Contains<string>("_BDRF3"))
            {
               sb.AppendLine("      #define UNITY_BRDF_PBS BRDF3_Unity_PBS");
            }
         }
         sb.AppendLine("   ENDCG");
         sb.AppendLine();

         sb.AppendLine("   SubShader {");

         sb.AppendLine("      Tags{ \"RenderType\" = \"Opaque\"  \"Queue\" = \"Geometry+100\" }");
         sb.AppendLine("      Cull Back");
         sb.AppendLine("      ZTest LEqual");
         if (blend)
         {
            sb.AppendLine("      BLEND ONE ONE");
         }
         sb.AppendLine("      CGPROGRAM");

         sb.AppendLine("      #pragma exclude_renderers d3d9");
      }



      void WriteFeatures(string[] features, StringBuilder sb)
      {
         sb.AppendLine();
         for (int i = 0; i < features.Length; ++i)
         {
            sb.AppendLine("      #define " + features[i] + " 1");
         }
         sb.AppendLine();

         sb.AppendLine(sharedInc.text);

         // sort for compile order
         extensions.Sort(delegate(FeatureDescriptor p1, FeatureDescriptor p2)
         {
            if (p1.CompileSortOrder() != p2.CompileSortOrder())
               return (p1.CompileSortOrder() < p2.CompileSortOrder()) ? -1 : 1;
            return p1.GetType().Name.CompareTo(p2.GetType().Name);
         });
            
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions[i];
            if (ext.GetVersion() == MicroSplatVersion)
            {
               extensions[i].WriteFunctions(sb);
            }
         }

         // sort by name, then display order..
         extensions.Sort(delegate(FeatureDescriptor p1, FeatureDescriptor p2)
         {
            if (p1.DisplaySortOrder() != 0 || p2.DisplaySortOrder() != 0)
            {
               return p1.DisplaySortOrder().CompareTo(p2.DisplaySortOrder());
            }
            return p1.GetType().Name.CompareTo(p2.GetType().Name);
         });

      }

      void WriteFooter(string[] features, StringBuilder b, string baseName, bool blendable)
      {
         if (blendable)
         {
            b.AppendLine("   CustomEditor \"MicroSplatBlendableMaterialEditor\"");
         }
         else if (baseName != null)
         {
            b.AppendLine("   Dependency \"AddPassShader\" = \"Hidden/MicroSplat/AddPass\"");
            b.AppendLine("   Dependency \"BaseMapShader\" = \"" + baseName + "\"");
            b.AppendLine("   CustomEditor \"MicroSplatShaderGUI\"");
         }

         b.Append("}");
      }

      void WriteProperties(string[] features, StringBuilder sb, bool blendable)
      {
         sb.AppendLine("   Properties {");
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions[i];
            if (ext.GetVersion() == MicroSplatVersion)
            {
               ext.WriteProperties(features, sb);
            }
            sb.AppendLine("");
         }
         sb.AppendLine("   }");
      }

      static bool HasDebugFeature(string[] features)
      {
         return features.Contains("_DEBUG_OUTPUT_ALBEDO") ||
            features.Contains("_DEBUG_OUTPUT_NORMAL") ||
            features.Contains("_DEBUG_OUTPUT_HEIGHT") ||
            features.Contains("_DEBUG_OUTPUT_METAL") ||
            features.Contains("_DEBUG_OUTPUT_SMOOTHNESS") ||
            features.Contains("_DEBUG_OUTPUT_AO") ||
            features.Contains("_DEBUG_OUTPUT_EMISSION");

      }


      static StringBuilder sBuilder = new StringBuilder(256000);
      public string Compile(string[] features, string name, string baseName = null, bool blendable = false)
      {
         Init();
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions[i];
            ext.Unpack(features);
         }
         sBuilder.Length = 0;
         var sb = sBuilder;
         sb.AppendLine("//////////////////////////////////////////////////////");
         sb.AppendLine("// MicroSplat");
         sb.AppendLine("// Copyright (c) Jason Booth, slipster216@gmail.com");
         sb.AppendLine("//");
         sb.AppendLine("// Auto-generated shader code, don't hand edit!");
         sb.AppendLine("//   Compiled with MicroSplat " + MicroSplatVersion);
         sb.AppendLine("//   Unity : " + Application.unityVersion);
         sb.AppendLine("//   Platform : " + Application.platform);
         sb.AppendLine("//////////////////////////////////////////////////////");
         sb.AppendLine();

         if (!blendable && baseName == null)
         {
            sb.Append("Shader \"Hidden/MicroSplat/");
         }
         else
         {
            sb.Append("Shader \"MicroSplat/");
         }
         while (name.Contains("/"))
         {
            name = name.Substring(name.IndexOf("/") + 1);
         }
         sb.Append(name);
         if (blendable)
         { 
            sb.Append("_BlendWithTerrain");
         }
         sb.AppendLine("\" {");


         // props
         WriteProperties(features, sb, blendable);


         WriteHeader(features, sb, blendable);
         if (blendable)
         {  
            sb.Append(declareBlend);
         }
         else if (!features.Contains<string>("_TESSDISTANCE"))
         {
            if (HasDebugFeature(features))
            {
               sb.Append(declareTerrainDebug);
            }
            else
            {
               sb.Append(declareTerrain);
            }
         }
         else
         {
            sb.Append(declareTerrainTess);
         }

         if (!blendable)
         {
            if (features.Contains<string>("_BDRF1") || features.Contains<string>("_BDRF2") || features.Contains<string>("_BDRF3"))
            {
               sb.Append(" exclude_path:deferred");
            }
         }

         // don't remove
         sb.AppendLine();
         sb.AppendLine();
            
         sb.AppendLine("      #pragma target " + GetShaderModel());

         WriteFeatures(features, sb);
         sb.AppendLine(terrainBody.text);

         if (blendable && terrainBlendBody != null)
         {
            sb.AppendLine(terrainBlendBody.text);
         }

         sb.AppendLine("ENDCG\n\n   }");
         WriteFooter(features, sb, baseName, blendable);

         string output = sb.ToString();


         return output;
      }

      public void Compile(Material m, string shaderName = null)
      {
         int hash = 0;
         for (int i = 0; i < m.shaderKeywords.Length; ++i)
         {
            hash += 31 + m.shaderKeywords[i].GetHashCode();
         }
         var path = AssetDatabase.GetAssetPath(m.shader);
         string nm = m.shader.name;
         if (!string.IsNullOrEmpty(shaderName))
         {
            nm = shaderName;
         }
         string baseName = "Hidden/" + nm + "_Base" + hash.ToString();

         string terrainShader = Compile(m.shaderKeywords, nm, baseName);

         string blendShader = null;
         if (terrainBlendBody != null)
         {
            if (m.IsKeywordEnabled("_TERRAINBLENDING"))
            {
               List<string> blendKeywords = new List<string>(m.shaderKeywords);
               if (m.IsKeywordEnabled("_TBDISABLE_DETAILNOISE") && blendKeywords.Contains("_DETAILNOISE"))
               {
                  blendKeywords.Remove("_DETAILNOISE");
               }
               if (m.IsKeywordEnabled("_TBDISABLE_DISTANCENOISE") && blendKeywords.Contains("_DISTANCENOISE"))
               {
                  blendKeywords.Remove("_DISTANCENOISE");
               }
               if (m.IsKeywordEnabled("_TBDISABLE_DISTANCERESAMPLE") && blendKeywords.Contains("_DISTANCERESAMPLE"))
               {
                  blendKeywords.Remove("_DISTANCERESAMPLE");
               }

               blendShader = Compile(blendKeywords.ToArray(), nm, null, true);
            }
         }

         // generate fallback
         string[] oldKeywords = new string[m.shaderKeywords.Length];
         System.Array.Copy(m.shaderKeywords, oldKeywords, m.shaderKeywords.Length);
         m.DisableKeyword("_TESSDISTANCE");
         m.DisableKeyword("_TESSEDGE");
         m.DisableKeyword("_PARALLAX");
         m.DisableKeyword("_DETAILNOISE");

         // maybe reduce layers in distance? can cause a pop though..
         //m.DisableKeyword("_MAX3LAYER");
         //m.EnableKeyword("_MAX2LAYER");


         string fallback = Compile(m.shaderKeywords, baseName);
         m.shaderKeywords = oldKeywords;

         System.IO.File.WriteAllText(path, terrainShader);
         string fallbackPath = path.Replace(".shader", "_Base.shader");
         string terrainBlendPath = path.Replace(".shader", "_TerrainObjectBlend.shader");
         System.IO.File.WriteAllText(fallbackPath, fallback);
         if (blendShader != null)
         {
            System.IO.File.WriteAllText(terrainBlendPath, blendShader); 
         }

         EditorUtility.SetDirty(m);
         AssetDatabase.Refresh();
         MicroSplatTerrain.SyncAll(); 
      }
   }
}
