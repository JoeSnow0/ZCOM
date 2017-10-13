using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace JBooth.MicroSplat
{
   public abstract class FeatureDescriptor
   {
      /// <summary>
      /// All versions must match for module to be active
      /// </summary>
      /// <returns>The version.</returns>
      public abstract string GetVersion();


      // used when you have compiler ordering issues
      public virtual int CompileSortOrder() 
      {
         return 0;
      }

      public virtual int DisplaySortOrder()
      {
         return 0;
      }

      public abstract string ModuleName();

      /// <summary>
      /// Requireses the shader model46.
      /// </summary>
      /// <returns><c>true</c>, if shader model46 was requiresed, <c>false</c> otherwise.</returns>
      public virtual bool RequiresShaderModel46() { return false; }

      /// <summary>
      /// DrawGUI for shader compiler feature options
      /// </summary>
      /// <param name="mat">Mat.</param>
      public abstract void DrawFeatureGUI(Material mat);

      /// <summary>
      /// Draw the editor for the shaders options
      /// </summary>
      /// <param name="shaderGUI">Shader GU.</param>
      /// <param name="mat">Mat.</param>
      /// <param name="materialEditor">Material editor.</param>
      /// <param name="props">Properties.</param>
      public abstract void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, Material mat, MaterialEditor materialEditor, MaterialProperty[] props);


      /// <summary>
      /// Got per texture properties? Draw the GUI for them here..
      /// </summary>
      /// <param name="index">Index.</param>
      /// <param name="shaderGUI">Shader GU.</param>
      /// <param name="mat">Mat.</param>
      /// <param name="materialEditor">Material editor.</param>
      /// <param name="props">Properties.</param>
      public virtual void DrawPerTextureGUI(int index, Material mat, MicroSplatPropData propData)
      {
      }

      /// <summary>
      /// Unpack your keywords from the material
      /// </summary>
      /// <param name="keywords">Keywords.</param>
      public abstract void Unpack(string[] keywords);

      /// <summary>
      /// pack keywords to a string[]
      /// </summary>
      public abstract string[] Pack();

      /// <summary>
      /// Init yourself
      /// </summary>
      /// <param name="paths">Paths.</param>
      public abstract void InitCompiler(string[] paths);

      /// <summary>
      /// write property definitions to the shader
      /// </summary>
      /// <param name="features">Features.</param>
      /// <param name="sb">Sb.</param>
      public abstract void WriteProperties(string[] features, StringBuilder sb);

      /// <summary>
      /// Write the core functions you use to the shader
      /// </summary>
      /// <param name="sb">Sb.</param>
      public abstract void WriteFunctions(StringBuilder sb);

      /// <summary>
      /// Compute rough cost parameters for your section of the shader
      /// </summary>
      /// <param name="features">List of material features.<param> 
      /// <param name="arraySampleCount">Array sample count.</param>
      /// <param name="textureSampleCount">Texture sample count.</param>
      /// <param name="maxSamples">Max samples.</param>
      /// <param name="tessellationSamples">Tessellation samples.</param>
      /// <param name="depTexReadLevel">Dep tex read level.</param>
      public abstract void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, 
                                               ref int tessellationSamples, ref int depTexReadLevel);


      public void Pack(Material m)
      {
         var pck = Pack();
         for (int i = 0; i < pck.Length; ++i)
         {
            m.EnableKeyword(pck[i]);
         }
      }


      public enum Channel
      {
         R = 0,
         G,
         B,
         A
      }

      bool PerTexToggle(Material mat, string keyword)
      {
         bool enabled = mat.IsKeywordEnabled(keyword);
         bool newEnabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
         if (enabled != newEnabled)
         {
            if (newEnabled)
               mat.EnableKeyword(keyword);
            else
               mat.DisableKeyword(keyword);
         }
         return newEnabled;
      }
         
      protected void InitPropData(int pixel, MicroSplatPropData propData, Color defaultValues)
      {
         if (propData == null)
         {
            return;
         }
         // we reserve the last row of potential values as an initialization bit. 
         if (propData.GetValue(pixel, 15) == new Color(0,0,0,0))
         {
            for (int i = 0; i < 16; ++i)
            {
               propData.SetValue(i, pixel, defaultValues);
            }
            propData.SetValue(pixel, 15, Color.white);
         }
      }
         
      protected bool DrawPerTexFloatSlider(int curIdx, int pixel, string keyword, Material mat, MicroSplatPropData propData, Channel channel, 
         GUIContent label, float min = 0, float max = 0)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(mat, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         float v = c[(int)channel];
         float nv = v;
         if (min != max)
         {
            nv = EditorGUILayout.Slider(label, v, min, max);
         }
         else
         {
            nv = EditorGUILayout.FloatField(label, v);
         }
         if (nv != v)
         {
            c[(int)channel] = nv;
            propData.SetValue(curIdx, pixel, c);

         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < 16; ++i)
            {
               propData.SetValue(i, pixel, (int)channel, nv);
            }
         }

         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }


      protected void DrawPerTexFloatSliderNoToggle(int curIdx, int pixel, string keyword, Material mat, MicroSplatPropData propData, Channel channel, 
         GUIContent label, float min = 0, float max = 0)
      {
         EditorGUILayout.BeginHorizontal();
         EditorGUILayout.LabelField("", GUILayout.Width(20));
         bool enabled = mat.IsKeywordEnabled(keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         float v = c[(int)channel];
         float nv = v;
         if (min != max)
         {
            nv = EditorGUILayout.Slider(label, v, min, max);
         }
         else
         {
            nv = EditorGUILayout.FloatField(label, v);
         }
         if (nv != v)
         {
            c[(int)channel] = nv;
            propData.SetValue(curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < 16; ++i)
            {
               propData.SetValue(i, pixel, (int)channel, nv);
            }
         }

         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();
      }

      protected enum V2Cannel
      {
         RG = 0,
         BA
      }

      protected bool DrawPerTexVector2(int curIdx, int pixel, string keyword, Material mat, MicroSplatPropData propData, V2Cannel channel, 
         GUIContent label)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(mat, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         Vector2 v2 = new Vector2(c.r, c.g);
         if (channel == V2Cannel.BA)
         {
            v2.x = c.b;
            v2.y = c.a;
         }
         Vector2 nv = v2;

         nv = EditorGUILayout.Vector2Field(label, v2);

         if (nv != v2)
         {
            if (channel == V2Cannel.RG)
            {
               c.r = nv.x;
               c.g = nv.y;
            }
            else
            {
               c.b = nv.x;
               c.a = nv.y;
            }
            propData.SetValue(curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < 16; ++i)
            {
               // don't erase other pixels..
               var fv = propData.GetValue(i, pixel);
               if (channel == V2Cannel.RG)
               {
                  c.r = nv.x;
                  c.g = nv.y;
               }
               else
               {
                  c.b = nv.x;
                  c.a = nv.y;
               }
               propData.SetValue(i, pixel, fv);
            }
         }
         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }


      protected bool DrawPerTexVector2Vector2(int curIdx, int pixel, string keyword, Material mat, MicroSplatPropData propData,
         GUIContent label, GUIContent label2)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(mat, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         Vector2 v1 = new Vector2(c.r, c.g);
         Vector2 v2 = new Vector2(c.b, c.a);
         Vector2 nv1 = v1;
         Vector2 nv2 = v2;
         EditorGUILayout.BeginVertical();
         nv1 = EditorGUILayout.Vector2Field(label, v1);
         nv2 = EditorGUILayout.Vector2Field(label2, v2);
         EditorGUILayout.EndVertical();

         if (nv1 != v1 || nv2 != v2)
         {
            c.r = nv1.x;
            c.g = nv1.y;
            c.b = nv2.x;
            c.a = nv2.y;
            propData.SetValue(curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            c.r = nv1.x;
            c.g = nv1.y;
            c.b = nv2.x;
            c.a = nv2.y;
            for (int i = 0; i < 16; ++i)
            {
               propData.SetValue(i, pixel, c);
            }
         }
         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }

      protected bool DrawPerTexColor(int curIdx, int pixel, string keyword, Material mat, MicroSplatPropData propData, 
         GUIContent label, bool hasAlpha)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(mat, keyword);
         GUI.enabled = enabled;
         Color c = propData.GetValue(curIdx, pixel);
         Color nv = EditorGUILayout.ColorField(label, c);
         if (nv != c)
         {
            if (!hasAlpha)
            {
               nv.a = c.a;
            }
            propData.SetValue(curIdx, pixel, nv);

         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < 16; ++i)
            {
               if (!hasAlpha)
               {
                  nv.a = propData.GetValue(i, pixel).a;
               }
               propData.SetValue(i, pixel, nv);
            }
         }

         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }
   }
}