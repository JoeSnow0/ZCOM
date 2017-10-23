//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth, slipster216@gmail.com
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   [CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
   [ExecuteInEditMode]
   public class TextureArrayConfig : ScriptableObject 
   {
      public enum AllTextureChannel
      {
         R = 0,
         G,
         B,
         A,
         Custom
      }


      public enum TextureChannel
      {
         R = 0,
         G,
         B,
         A
      }

      public enum Compression
      {
         AutomaticCompressed,
         Uncompressed
      }

      public enum TextureSize
      {
         k2048 = 2048,
         k1024 = 1024,
         k512 = 512,
         k256 = 256,
      }

      // for the interface
      public enum TextureMode
      {
         Basic,
         PBR
      }

      [HideInInspector]
      public TextureMode textureMode = TextureMode.Basic;

      [HideInInspector]
      public int hash;

      public int GetNewHash()
      {
         unchecked
         {
            int h = 17;
            h = h * Application.platform.GetHashCode() * 31;
            h = h * Application.unityVersion.GetHashCode() * 37;
            #if UNITY_EDITOR
            h = h * UnityEditor.EditorUserBuildSettings.activeBuildTarget.GetHashCode() * 13;
            #endif
            return h;
         }
      }

      static List<TextureArrayConfig> sAllConfigs = new List<TextureArrayConfig>();
      void Awake()
      {
         sAllConfigs.Add(this);
      }

      void OnDestroy()
      {
         sAllConfigs.Remove(this);
      }

      #if UNITY_EDITOR
      public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
      {
         List<T> assets = new List<T>();
         string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace("UnityEngine.", "")));
         for( int i = 0; i < guids.Length; i++ )
         {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath( guids[i] );
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if( asset != null )
            {
               assets.Add(asset);
            }
         }
         return assets;
      }
      #endif

      public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
      {
         #if UNITY_EDITOR
         if (sAllConfigs.Count == 0)
         {
            sAllConfigs = FindAssetsByType<TextureArrayConfig>();
         }
         #endif

         for (int i = 0; i < sAllConfigs.Count; ++i)
         {
            if (sAllConfigs[i].diffuseArray == diffuse)
            {
               return sAllConfigs[i];
            }
         }
         return null;
      }

      [HideInInspector]
      public Texture2DArray diffuseArray;
      [HideInInspector]
      public Texture2DArray normalSAOArray;

      public TextureSize diffuseTextureSize = TextureSize.k1024;
      public Compression diffuseCompression = Compression.AutomaticCompressed;
      public FilterMode diffuseFilterMode = FilterMode.Bilinear;
      public int diffuseAnisoLevel = 1;

      public TextureSize normalSAOTextureSize = TextureSize.k1024;
      public Compression normalCompression = Compression.AutomaticCompressed;
      public FilterMode normalFilterMode = FilterMode.Trilinear;
      public int normalAnisoLevel = 1;



      [HideInInspector]
      public AllTextureChannel allTextureChannelHeight = AllTextureChannel.G;
      [HideInInspector]
      public AllTextureChannel allTextureChannelSmoothness = AllTextureChannel.G;
      [HideInInspector]
      public AllTextureChannel allTextureChannelAO = AllTextureChannel.G;

      [System.Serializable]
      public class TextureEntry
      {
         public ProceduralMaterial substance;
         public Texture2D diffuse;
         public Texture2D height;
         public TextureChannel heightChannel = TextureChannel.G;
         public Texture2D normal;
         public Texture2D smoothness;
         public TextureChannel smoothnessChannel = TextureChannel.G;
         public bool isRoughness;
         public Texture2D ao;
         public TextureChannel aoChannel = TextureChannel.G;

         public void Reset()
         {
            substance = null;
            diffuse = null;
            height = null;
            normal = null;
            smoothness = null;
            ao = null;
            isRoughness = false;
            heightChannel = TextureChannel.G;
            smoothnessChannel = TextureChannel.G;
            aoChannel = TextureChannel.G;
         }

         public bool HasTextures()
         {
            return (substance != null || diffuse != null || height != null || normal != null || smoothness != null || ao != null);
         }
      }

      [HideInInspector]
      public List<TextureEntry> sourceTextures = new List<TextureEntry>();
   }
}
