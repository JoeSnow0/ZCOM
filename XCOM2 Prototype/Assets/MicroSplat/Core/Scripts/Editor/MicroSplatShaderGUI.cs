//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using JBooth.MicroSplat;
using System.Linq;


public partial class MicroSplatShaderGUI : ShaderGUI 
{
   public static readonly string MicroSplatVersion = "1.3";

   MicroSplatCompiler compiler = new MicroSplatCompiler();

   public MaterialProperty FindProp(string name, MaterialProperty[] props)
   {
      return FindProperty(name, props);
   }

   GUIContent CShaderName = new GUIContent("Name", "Menu path with name for the shader");



   bool needsCompile = false;
   int perTexIndex = 0;
   System.Text.StringBuilder builder = new System.Text.StringBuilder(1024);
   public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
   {
      EditorGUI.BeginChangeCheck(); // sync materials
      Material targetMat = materialEditor.target as Material;
      Texture2DArray diff = targetMat.GetTexture("_Diffuse") as Texture2DArray;


      compiler.Init();
      // must unpack everything before the generator draws- otherwise we get IMGUI errors
      for (int i = 0; i < compiler.extensions.Count; ++i)
      {
         var ext = compiler.extensions[i];
         ext.Unpack(targetMat.shaderKeywords);
      }
         
      string shaderName = targetMat.shader.name;
      DrawModules();

      EditorGUI.BeginChangeCheck(); // needs compile

      if (MicroSplatUtilities.DrawRollup("Shader Generator"))
      {
         shaderName = EditorGUILayout.DelayedTextField(CShaderName, shaderName);

         for (int i = 0; i < compiler.extensions.Count; ++i)
         {
            var e = compiler.extensions[i];
            if (e.GetVersion() == MicroSplatVersion)
            {
               e.DrawFeatureGUI(targetMat);
            }
            else
            {
               EditorGUILayout.HelpBox("Extension : " + e + " is version " + e.GetVersion() + " and MicroSplat is version " + MicroSplatVersion + ", please update", MessageType.Error);
            }
         }
      }
      needsCompile = EditorGUI.EndChangeCheck();

      int featureCount = targetMat.shaderKeywords.Length;
      // note, ideally we wouldn't draw the GUI for the rest of stuff if we need to compile.
      // But we can't really do that without causing IMGUI to split warnings about
      // mismatched GUILayout blocks
      if (!needsCompile)
      {
         for (int i = 0; i < compiler.extensions.Count; ++i)
         {
            var ext = compiler.extensions[i];
            if (ext.GetVersion() == MicroSplatVersion)
            {
               ext.DrawShaderGUI(this, targetMat, materialEditor, props);
            }
            else
            {
               EditorGUILayout.HelpBox("Extension : " + ext + " is version " + ext.GetVersion() + " and MicroSplat is version " + MicroSplatVersion + ", please update so that all modules are using the same version.", MessageType.Error);
            }

         }


         if (diff != null && MicroSplatUtilities.DrawRollup("Per Texture Properties"))
         {
            var propTex = FindOrCreatePropTex(targetMat);
            perTexIndex = MicroSplatUtilities.DrawTextureSelector(perTexIndex, diff);
            for (int i = 0; i < compiler.extensions.Count; ++i)
            {
               var ext = compiler.extensions[i];
               if (ext.GetVersion() == MicroSplatVersion)
               {
                  ext.DrawPerTextureGUI(perTexIndex, targetMat, propTex);
               }
            }
         }
      }

      if (!needsCompile)
      {
         if (featureCount != targetMat.shaderKeywords.Length)
         {
            needsCompile = true;
         }
      }
         
         
      int arraySampleCount = 0;
      int textureSampleCount = 0;
      int maxSamples = 0;
      int tessSamples = 0;
      int depTexReadLevel = 0;
      builder.Length = 0;
      for (int i = 0; i < compiler.extensions.Count; ++i)
      {
         var ext = compiler.extensions[i];
         if (ext.GetVersion() == MicroSplatVersion)
         {
            ext.ComputeSampleCounts(targetMat.shaderKeywords, ref arraySampleCount, ref textureSampleCount, ref maxSamples, ref tessSamples, ref depTexReadLevel);
         }
      }
      if (MicroSplatUtilities.DrawRollup("Debug"))
      {
         string shaderModel = compiler.GetShaderModel();
         builder.Append("Shader Model : ");
         builder.AppendLine(shaderModel);
         if (maxSamples != arraySampleCount)
         {
            builder.Append("Texture Array Samples : ");
            builder.AppendLine(arraySampleCount.ToString());

            builder.Append("Regular Samples : ");
            builder.AppendLine(textureSampleCount.ToString());
         }
         else
         {
            builder.Append("Texture Array Samples : ");
            builder.AppendLine(arraySampleCount.ToString());
            builder.Append("Regular Samples : ");
            builder.AppendLine(textureSampleCount.ToString());
         }
         if (tessSamples > 0)
         {
            builder.Append("Tessellation Samples : ");
            builder.AppendLine(tessSamples.ToString());
         }
         if (depTexReadLevel > 0)
         {
            builder.Append(depTexReadLevel.ToString());
            builder.AppendLine(" areas with dependent texture reads");
         }

         EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
      }
         
      if (EditorGUI.EndChangeCheck() && !needsCompile)
      {
         MicroSplatTerrain.SyncAll();
      }

      if (needsCompile)
      {
         needsCompile = false;
         targetMat.shaderKeywords = null;
         for (int i = 0; i < compiler.extensions.Count; ++i)
         {
            compiler.extensions[i].Pack(targetMat);
         }

         // horrible workaround to GUI warning issues
         compileMat = targetMat;
         compileName = shaderName;
         targetCompiler = compiler;
         EditorApplication.delayCall += TriggerCompile;
      }
   }

   static Material compileMat;
   static string compileName;
   static MicroSplatCompiler targetCompiler;
   protected void TriggerCompile()
   {
      targetCompiler.Compile(compileMat, compileName);
   }


   class Module
   {
      public Module(string url, string img)
      {
         assetStore = url;
         texture = Resources.Load<Texture2D>(img);
      }
      public string assetStore;
      public Texture2D texture;
   }

   void InitModules()
   {
      if (modules.Count == 0)
      {
         //
         #if !__MICROSPLAT_GLOBALTEXTURE__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/96482?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_globaltexture"));
         #endif
         #if !__MICROSPLAT_SNOW__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/96486?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_snow"));
         #endif
         #if !__MICROSPLAT_TESSELLATION__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/96484?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_tessellation"));
         #endif
         #if !__MICROSPLAT_DETAILRESAMPLE__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/96480?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_detailresample"));
         #endif
         #if !__MICROSPLAT_TERRAINBLEND__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/97364?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_terrainblending"));
         #endif
         #if !__MICROSPLAT_STREAMS__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/97993?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_streams"));
         #endif
         #if !__MICROSPLAT_ALPHAHOLE__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/97495?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_alphahole"));
         #endif
         #if !__MICROSPLAT_TRIPLANAR__
         modules.Add(new Module("https://www.assetstore.unity3d.com/#!/content/96777?aid=1011l37NJ&pubref=1011l37NJ", "microsplat_module_triplanaruvs"));
         #endif

         int n = modules.Count;
         if (n > 1)
         {
            System.Random rnd = new System.Random((int)(UnityEngine.Random.value * 1000)); 
            while (n > 1)
            {  
               n--;  
               int k = rnd.Next(n + 1);  
               var value = modules[k];  
               modules[k] = modules[n];  
               modules[n] = value;  
            } 
         }
      }
       

   }

   List<Module> modules = new List<Module>();

   Module openModule;
   void DrawModule(Module m)
   {
      if (GUILayout.Button(m.texture, GUI.skin.box, GUILayout.Width(128), GUILayout.Height(128)))
      {
         Application.OpenURL(m.assetStore);
      }
   }
   Vector2 moduleScroll;
   void DrawModules()
   {
      InitModules();
      if (modules.Count == 0)
      {
         return;
      }

      EditorGUILayout.LabelField("Want more features? Add them here..");

      moduleScroll = EditorGUILayout.BeginScrollView(moduleScroll, GUILayout.Height(156));
      GUILayout.BeginHorizontal();
      for (int i = 0; i < modules.Count; ++i)
      {
         DrawModule(modules[i]);
      }
      GUILayout.EndHorizontal();
      EditorGUILayout.EndScrollView();

   }
}

