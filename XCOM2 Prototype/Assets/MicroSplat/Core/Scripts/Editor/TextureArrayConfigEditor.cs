//////////////////////////////////////////////////////
// MicroSplat - 256 texture splat mapping
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   [CustomEditor(typeof(TextureArrayConfig))]
   public class TextureArrayConfigEditor : Editor 
   {

      void DrawHeader(TextureArrayConfig cfg)
      {
         if (cfg.textureMode == TextureArrayConfig.TextureMode.PBR)
         {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("", GUILayout.Width(30));
            EditorGUILayout.LabelField("Channel", GUILayout.Width(64));
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(20));
            EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(64));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Height"), GUILayout.Width(64));
            cfg.allTextureChannelHeight = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelHeight, GUILayout.Width(64));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Smoothness"), GUILayout.Width(64));
            cfg.allTextureChannelSmoothness = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelSmoothness, GUILayout.Width(64));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("AO"), GUILayout.Width(64));
            cfg.allTextureChannelAO = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelAO, GUILayout.Width(64));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            GUILayout.Box(Texture2D.blackTexture, GUILayout.Height(3), GUILayout.ExpandWidth(true));
         }
      }

      bool DrawTextureEntry(TextureArrayConfig cfg, TextureArrayConfig.TextureEntry e, int i)
      {
         bool ret = false;

         EditorGUILayout.BeginHorizontal();

         if (e.HasTextures())
         {
            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
            EditorGUILayout.LabelField(e.diffuse != null ? e.diffuse.name : "empty");
            ret = GUILayout.Button("Clear Entry");
         }
         else
         {
            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
            EditorGUILayout.HelpBox("Removing an entry completely can cause texture choices to change on existing terrains. You can leave it blank to preserve the texture order and MicroSplat will put a dummy texture into the array.", MessageType.Warning);
            ret = (GUILayout.Button("Delete Entry"));
         }
         EditorGUILayout.EndHorizontal();

         EditorGUILayout.BeginHorizontal();

         if (cfg.textureMode == TextureArrayConfig.TextureMode.PBR)
         {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Substance"), GUILayout.Width(64));
            e.substance = (ProceduralMaterial)EditorGUILayout.ObjectField(e.substance, typeof(ProceduralMaterial), false, GUILayout.Width(64), GUILayout.Height(64));
            EditorGUILayout.EndVertical();
         }

         EditorGUILayout.BeginVertical();
         EditorGUILayout.LabelField(new GUIContent("Diffuse"), GUILayout.Width(64));
         e.diffuse = (Texture2D)EditorGUILayout.ObjectField(e.diffuse, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
         EditorGUILayout.EndVertical();

         EditorGUILayout.BeginVertical();
         EditorGUILayout.LabelField(new GUIContent("Normal"), GUILayout.Width(64));
         e.normal = (Texture2D)EditorGUILayout.ObjectField(e.normal, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
         EditorGUILayout.EndVertical();

         if (cfg.textureMode == TextureArrayConfig.TextureMode.PBR)
         {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Height"), GUILayout.Width(64));
            e.height = (Texture2D)EditorGUILayout.ObjectField(e.height, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            if (cfg.allTextureChannelHeight == TextureArrayConfig.AllTextureChannel.Custom)
            {
               e.heightChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.heightChannel, GUILayout.Width(64));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("Smoothness"), GUILayout.Width(64));
            e.smoothness = (Texture2D)EditorGUILayout.ObjectField(e.smoothness, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            if (cfg.allTextureChannelSmoothness == TextureArrayConfig.AllTextureChannel.Custom)
            {
               e.smoothnessChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.smoothnessChannel, GUILayout.Width(64));
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Invert", GUILayout.Width(44));
            e.isRoughness = EditorGUILayout.Toggle(e.isRoughness, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("AO"), GUILayout.Width(64));
            e.ao = (Texture2D)EditorGUILayout.ObjectField(e.ao, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            if (cfg.allTextureChannelAO == TextureArrayConfig.AllTextureChannel.Custom)
            {
               e.aoChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.aoChannel, GUILayout.Width(64));
            }
            EditorGUILayout.EndVertical();
         }
         EditorGUILayout.EndHorizontal();
         GUILayout.Box(Texture2D.blackTexture, GUILayout.Height(3), GUILayout.ExpandWidth(true));
         return ret;
      }

      public static bool GetFromTerrain(TextureArrayConfig cfg, Terrain t)
      {
         if (t != null && cfg.sourceTextures.Count == 0 && t.terrainData != null)
         {
            int count = t.terrainData.splatPrototypes.Length;
            for (int i = 0; i < count; ++i)
            {
               var proto = t.terrainData.splatPrototypes[i];
               var e = new TextureArrayConfig.TextureEntry();
               e.diffuse = proto.texture;
               e.normal = proto.normalMap;
               cfg.sourceTextures.Add(e);
            }
            return true;
         }
         return false;
      }

      static void GetFromTerrain(TextureArrayConfig cfg)
      {
         Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();
         for (int x = 0; x < terrains.Length; ++x)
         {
            var t = terrains[x];
            if (GetFromTerrain(cfg, t))
               return;
         }
      }


      public static TextureArrayConfig CreateConfig(Terrain t)
      {
         TextureArrayConfig cfg = TextureArrayConfig.CreateInstance<TextureArrayConfig>();
         GetFromTerrain(cfg, t);
         string path = MicroSplatUtilities.RelativePathFromAsset(t.terrainData);
         string configPath = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplatConfig.asset");
         AssetDatabase.CreateAsset(cfg, configPath);
         AssetDatabase.SaveAssets();
         AssetDatabase.Refresh();
         cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(configPath);
         CompileConfig(cfg);
         return cfg;

      }

      static GUIContent CTextureMode = new GUIContent("Texturing Mode", "Do you have just diffuse and normal, or a fully PBR pipeline with height, smoothness, and ao textures?");
      static GUIContent CTextureSize = new GUIContent("Texture Size", "Size for all textures");
      public override void OnInspectorGUI()
      {
         var cfg = target as TextureArrayConfig;
         EditorGUI.BeginChangeCheck();
         cfg.textureMode = (TextureArrayConfig.TextureMode)EditorGUILayout.EnumPopup(CTextureMode, cfg.textureMode);
         if (cfg.textureMode == TextureArrayConfig.TextureMode.PBR)
         {
            DrawDefaultInspector();
         }
         else
         {
            EditorGUILayout.HelpBox("Select PBR mode to use substances or provide custom height, smoothness, and ao textures to greatly increase quality!", MessageType.Info);
            cfg.diffuseTextureSize = (TextureArrayConfig.TextureSize)EditorGUILayout.EnumPopup(CTextureSize, cfg.diffuseTextureSize);
            cfg.normalSAOTextureSize = cfg.diffuseTextureSize;
         }

         if (MicroSplatUtilities.DrawRollup("Textures", true))
         {
            EditorGUILayout.HelpBox("Don't have a normal map? Any missing textures will be generated automatically from the best available source texture", MessageType.Info);

            DrawHeader(cfg);
            for (int i = 0; i < cfg.sourceTextures.Count; ++i)
            {
               if (DrawTextureEntry(cfg, cfg.sourceTextures[i], i))
               {
                  var e = cfg.sourceTextures[i];
                  if (!e.HasTextures())
                  {
                     cfg.sourceTextures.RemoveAt(i);
                     i--;
                  }
                  else
                  {
                     e.Reset();
                  }
               }
            }
            if (GUILayout.Button("Add Textures"))
            {
               var entry = new TextureArrayConfig.TextureEntry();
               cfg.sourceTextures.Add(entry);
               entry.aoChannel = cfg.sourceTextures[0].aoChannel;
               entry.heightChannel = cfg.sourceTextures[0].heightChannel;
               entry.smoothnessChannel = cfg.sourceTextures[0].smoothnessChannel;

            }
         }
         if (GUILayout.Button("Grab From Scene Terrain"))
         {
            cfg.sourceTextures.Clear();
            GetFromTerrain(cfg);
         }
         if (GUILayout.Button("Update"))
         {
            staticConfig = cfg;
            EditorApplication.delayCall += DelayedCompileConfig;
         }
         if (EditorGUI.EndChangeCheck())
         {
            EditorUtility.SetDirty(cfg);
         }
      }

      static bool IsLinear(TextureImporter ti)
      {
         return ti.sRGBTexture == false;
      }

      static Texture2D ResizeTexture(Texture2D source, int width, int height, bool linear)
      {
         RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
         rt.DiscardContents();
         GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear) && !linear;
         Graphics.Blit(source, rt);
         GL.sRGBWrite = false;
         RenderTexture.active = rt;
         Texture2D ret = new Texture2D(width, height, TextureFormat.ARGB32, true, linear);
         ret.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
         ret.Apply(true);
         RenderTexture.active = null;
         rt.Release();
         DestroyImmediate(rt);
         return ret;
      }

      static TextureFormat GetTextureFormat()
      {
         var platform = EditorUserBuildSettings.activeBuildTarget;
         if (platform == BuildTarget.Android)
         {
            return TextureFormat.ETC2_RGBA8;
         }
         else if (platform == BuildTarget.iOS)
         {
            return TextureFormat.PVRTC_RGBA4;
         }
         else
         {
            return TextureFormat.DXT5;
         }
      }

      static Texture2D RenderMissingTexture(Texture2D src, string shaderPath, int width, int height, int channel = -1)
      {
         Texture2D res = new Texture2D(width, height, TextureFormat.ARGB32, true, true);
         RenderTexture resRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
         resRT.DiscardContents();
         Shader s = Shader.Find(shaderPath);
         if (s == null)
         {
            Debug.LogError("Could not find shader " + shaderPath);
            res.Apply();
            return res;
         }
         Material genMat = new Material(Shader.Find(shaderPath));
         if (channel >= 0)
         {
            genMat.SetInt("_Channel", channel);
         }

         GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
         Graphics.Blit(src, resRT, genMat);
         GL.sRGBWrite = false;

         RenderTexture.active = resRT;
         res.ReadPixels(new Rect(0, 0, width, height), 0, 0);
         res.Apply();
         RenderTexture.active = null;
         resRT.Release();
         DestroyImmediate(resRT);
         DestroyImmediate(genMat);
         return res;
      }

      static void MergeInChannel(Texture2D target, int targetChannel, 
         Texture2D merge, int mergeChannel, bool linear, bool invert = false)
      {
         Texture2D src = ResizeTexture(merge, target.width, target.height, linear);
         Color[] sc = src.GetPixels();
         Color[] tc = target.GetPixels();

         for (int i = 0; i < tc.Length; ++i)
         {
            Color s = sc[i];
            Color t = tc[i];
            t[targetChannel] = s[mergeChannel];
            tc[i] = t;
         }
         if (invert)
         {
            for (int i = 0; i < tc.Length; ++i)
            {
               Color t = tc[i];
               t[targetChannel] = 1.0f - t[targetChannel];
               tc[i] = t;
            }
         }

         target.SetPixels(tc);
         target.Apply();
         DestroyImmediate(src);
      }

      static Texture2D BakeSubstance(string path, ProceduralTexture pt, bool linear = true, bool isNormal = false, bool invert = false)
      {
         string texPath = path + pt.name + ".tga";
         TextureImporter ti = TextureImporter.GetAtPath(texPath) as TextureImporter;
         if (ti != null)
         {
            bool changed = false;
            if (ti.sRGBTexture == true && linear)
            {
               ti.sRGBTexture = false;
               changed = true;
            }
            else if (ti.sRGBTexture == false && !linear)
            {
               ti.sRGBTexture = true;
               changed = true;
            }
            if (isNormal && ti.textureType != TextureImporterType.NormalMap)
            {
               ti.textureType = TextureImporterType.NormalMap;
               changed = true;
            }
            if (changed)
            {
               ti.SaveAndReimport();
            }
         }
         var srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
         return srcTex;
      }

      static void PreprocessTextureEntries(TextureArrayConfig cfg, bool diffuseIsLinear)
      {
         var src = cfg.sourceTextures;
         for (int i = 0; i < src.Count; ++i)
         {
            var e = src[i];
            // fill out substance data if it exists
            if (e.substance != null)
            {
               e.substance.isReadable = true;
               e.substance.RebuildTexturesImmediately();
               string srcPath = AssetDatabase.GetAssetPath(e.substance);

               e.substance.SetProceduralVector("$outputsize", new Vector4(11, 11, 0, 0)); // in mip map space, so 2048

               SubstanceImporter si = AssetImporter.GetAtPath(srcPath) as SubstanceImporter;
              
               si.SetMaterialScale(e.substance, new Vector2(2048, 2048));
               string path = AssetDatabase.GetAssetPath(cfg);
               path = path.Replace("\\", "/");
               path = path.Substring(0, path.LastIndexOf("/"));
               path += "/SubstanceExports/";
               System.IO.Directory.CreateDirectory(path);
               si.ExportBitmaps(e.substance, path, true);
               AssetDatabase.Refresh();

               Texture[] textures = e.substance.GetGeneratedTextures();
               for (int tidx = 0; tidx < textures.Length; tidx++)
               {
                  ProceduralTexture pt = e.substance.GetGeneratedTexture(textures[tidx].name);

                  if (pt.GetProceduralOutputType() == ProceduralOutputType.Diffuse)
                  {
                     e.diffuse = BakeSubstance(path, pt, diffuseIsLinear);
                  }
                  else if (pt.GetProceduralOutputType() == ProceduralOutputType.Height)
                  {
                     e.height = BakeSubstance(path, pt);
                  }
                  else if (pt.GetProceduralOutputType() == ProceduralOutputType.AmbientOcclusion)
                  {
                     e.ao = BakeSubstance(path, pt);
                  }
                  else if (pt.GetProceduralOutputType() == ProceduralOutputType.Normal)
                  {
                     e.normal = BakeSubstance(path, pt, true, true);
                  }
                  else if (pt.GetProceduralOutputType() == ProceduralOutputType.Smoothness)
                  {
                     e.smoothness = BakeSubstance(path, pt);
                     e.isRoughness = false;
                  }
                  else if (pt.GetProceduralOutputType() == ProceduralOutputType.Roughness)
                  {
                     e.smoothness = BakeSubstance(path, pt, true, false);
                     e.isRoughness = true;
                  }
               }
            }

         }
      }

      static TextureArrayConfig staticConfig;
      void DelayedCompileConfig()
      {
         CompileConfig(staticConfig);
      }

      public static void CompileConfig(TextureArrayConfig cfg)
      {
         bool diffuseIsLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;

         PreprocessTextureEntries(cfg, diffuseIsLinear);

         int diffuseWidth = (int)cfg.diffuseTextureSize;
         int diffuseHeight = (int)cfg.diffuseTextureSize;
         int normalWidth = (int)cfg.normalSAOTextureSize;
         int normalHeight = (int)cfg.normalSAOTextureSize;

         int diffuseAnisoLevel = cfg.diffuseAnisoLevel;
         int normalAnisoLevel = cfg.normalAnisoLevel;
         FilterMode diffuseFilter = cfg.diffuseFilterMode;
         FilterMode normalFilter = cfg.normalFilterMode;

         int diffuseMipCount = 11;
         if (diffuseWidth == 2048)
            diffuseMipCount = 12;
         else if (diffuseWidth == 1024)
            diffuseMipCount = 11;
         else if (diffuseWidth == 512)
            diffuseMipCount = 10;
         else if (diffuseWidth == 256)
            diffuseMipCount = 9;
            
         int normalMipCount = 11;
         if (normalWidth == 2048)
            normalMipCount = 12;
         else if (normalWidth == 1024)
            normalMipCount = 11;
         else if (normalWidth == 512)
            normalMipCount = 10;
         else if (normalWidth == 256)
            normalMipCount = 9;

         int texCount = cfg.sourceTextures.Count; 
         if (texCount < 1)
            texCount = 1;
         Texture2DArray diffuseArray = new Texture2DArray(diffuseWidth, diffuseHeight, texCount, 
            cfg.diffuseCompression == TextureArrayConfig.Compression.AutomaticCompressed ? GetTextureFormat() : TextureFormat.ARGB32, 
            true, diffuseIsLinear);
         
         diffuseArray.wrapMode = TextureWrapMode.Repeat;
         diffuseArray.filterMode = diffuseFilter;
         diffuseArray.anisoLevel = diffuseAnisoLevel;



         Texture2DArray normalSAOArray = new Texture2DArray(normalWidth, normalHeight, texCount,
            cfg.normalCompression == TextureArrayConfig.Compression.AutomaticCompressed ? GetTextureFormat() : TextureFormat.ARGB32, 
            true, true);
         
         normalSAOArray.wrapMode = TextureWrapMode.Repeat;
         normalSAOArray.filterMode = normalFilter;
         normalSAOArray.anisoLevel = normalAnisoLevel;

         for (int i = 0; i < cfg.sourceTextures.Count; ++i)
         {
            try
            {
               EditorUtility.DisplayProgressBar("Packing textures...", "", (float)i/(float)cfg.sourceTextures.Count);

               // first, generate any missing data. We generate a full NSAO map from diffuse or height map
               // if no height map is provided, we then generate it from the resulting or supplied normal. 
               var e = cfg.sourceTextures[i];
               Texture2D diffuse = e.diffuse;
               if (diffuse == null)
               {
                  diffuse = Texture2D.whiteTexture;
               }

               // resulting maps
               Texture2D diffuseHeightTex = ResizeTexture(diffuse, diffuseWidth, diffuseHeight, diffuseIsLinear);
               Texture2D normalSAOTex = null;
               int heightChannel = (int)e.heightChannel;
               int aoChannel = (int)e.aoChannel;
               int smoothChannel = (int)e.smoothnessChannel;
               if (cfg.allTextureChannelHeight != TextureArrayConfig.AllTextureChannel.Custom)
               {
                  heightChannel = (int)cfg.allTextureChannelHeight;
               }
               if (cfg.allTextureChannelAO != TextureArrayConfig.AllTextureChannel.Custom)
               {
                  aoChannel = (int)cfg.allTextureChannelAO;
               }
               if (cfg.allTextureChannelSmoothness != TextureArrayConfig.AllTextureChannel.Custom)
               {
                  smoothChannel = (int)cfg.allTextureChannelSmoothness;
               }

               if (e.normal == null)
               {
                  if (e.height == null)
                  {
                     normalSAOTex = RenderMissingTexture(diffuse, "Hidden/MicroSplat/NormalSAOFromDiffuse", normalWidth, normalHeight);
                  }
                  else
                  {
                     normalSAOTex = RenderMissingTexture(e.height, "Hidden/MicroSplat/NormalSAOFromHeight", normalWidth, normalHeight, heightChannel);
                  }
               }
               else
               {
                  // copy, but go ahead and generate other channels in case they aren't provided later.
                  normalSAOTex = RenderMissingTexture(e.normal, "Hidden/MicroSplat/NormalSAOFromNormal", normalWidth, normalHeight);
               }

               bool destroyHeight = false;
               Texture2D height = e.height;
               if (height == null)
               {
                  destroyHeight = true;
                  height = RenderMissingTexture(normalSAOTex, "Hidden/MicroSplat/HeightFromNormal", diffuseHeight, diffuseWidth);
               }
                  
               MergeInChannel(diffuseHeightTex, (int)TextureArrayConfig.TextureChannel.A, height, heightChannel, diffuseIsLinear);


               if (e.ao != null)
               {
                  MergeInChannel(normalSAOTex, (int)TextureArrayConfig.TextureChannel.B, e.ao, aoChannel, true); 
               }

               if (e.smoothness != null)
               {
                  MergeInChannel(normalSAOTex, (int)TextureArrayConfig.TextureChannel.R, e.smoothness, smoothChannel, true, e.isRoughness); 
               }
    

               if (cfg.normalCompression != TextureArrayConfig.Compression.Uncompressed)
               {
                  EditorUtility.CompressTexture(normalSAOTex, GetTextureFormat(), TextureCompressionQuality.Normal);
               }

               if (cfg.diffuseCompression != TextureArrayConfig.Compression.Uncompressed)
               {
                  EditorUtility.CompressTexture(diffuseHeightTex, GetTextureFormat(), TextureCompressionQuality.Normal);
               }

               normalSAOTex.Apply();
               diffuseHeightTex.Apply();

               for (int mip = 0; mip < diffuseMipCount; ++mip)
               {
                  Graphics.CopyTexture(diffuseHeightTex, 0, mip, diffuseArray, i, mip);
               }
               for (int mip = 0; mip < normalMipCount; ++mip)
               {
                  Graphics.CopyTexture(normalSAOTex, 0, mip, normalSAOArray, i, mip);
               }
               DestroyImmediate(diffuseHeightTex);
               DestroyImmediate(normalSAOTex);
               if (destroyHeight)
               {
                  DestroyImmediate(height);
               }
            }
            finally
            {
               EditorUtility.ClearProgressBar();
            }

         }
         EditorUtility.ClearProgressBar();

         diffuseArray.Apply(false, true);
         normalSAOArray.Apply(false, true);

         string path = AssetDatabase.GetAssetPath(cfg);
         // create array path
         path = path.Replace("\\", "/");
         string diffPath = path.Replace(".asset", "_diff_tarray.asset");
         string normSAOPath = path.Replace(".asset", "_normSAO_tarray.asset");
         {
            var existing = AssetDatabase.LoadAssetAtPath<Texture2DArray>(diffPath);
            if (existing != null)
            {
               EditorUtility.CopySerialized(diffuseArray, existing);
            }
            else
            {
               AssetDatabase.CreateAsset(diffuseArray, diffPath);
            }
         }
            
         {
            var existing = AssetDatabase.LoadAssetAtPath<Texture2DArray>(normSAOPath);
            if (existing != null)
            {
               EditorUtility.CopySerialized(normalSAOArray, existing);
            }
            else
            {
               AssetDatabase.CreateAsset(normalSAOArray, normSAOPath);
            }
         }
         cfg.diffuseArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(diffPath);
         cfg.normalSAOArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(normSAOPath);

         EditorUtility.SetDirty(cfg);
         AssetDatabase.Refresh();
         AssetDatabase.SaveAssets();

         MicroSplatUtilities.ClearPreviewCache();
      }

   }
}
