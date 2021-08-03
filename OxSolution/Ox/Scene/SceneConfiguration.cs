using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;

namespace Ox.Scene
{
    /// <summary>
    /// A serializable token for SceneConfiguration.
    /// </summary>
    public class SceneConfigurationToken
    {
        [DefaultValue(1)] // missing from old config files
        public float WaterReflectionMapSize { get { return waterReflectionMapSize; } set { waterReflectionMapSize = value; } }
        [DefaultValue(1)] // missing from old config files
        public float DirectionalShadowMapSize { get { return directionalShadowMapSize; } set { directionalShadowMapSize = value; } }
        [DefaultValue(0.02f)] // missing from old config files
        public float DirectionalShadowDepthBias { get { return directionalShadowDepthBias; } set { directionalShadowDepthBias = value; } }
        public float DirectionalShadowRange { get; set; }
        public int DirectionalShadowCount { get; set; }
        public int DirectionalLightCount { get; set; }
        public int PointLightCount { get; set; }
        public Vector2 DirectionalShadowSize { get; set; }
        public ConstructionTokens SceneConstructionTokens { get; set; }

        private float waterReflectionMapSize = 1;
        private float directionalShadowMapSize = 1;
        private float directionalShadowDepthBias = 0.02f;
    }

    /// <summary>
    /// Exposes a set of values used to configure the scene system.
    /// </summary>
    public static class SceneConfiguration
    {
        /// <summary>
        /// Initialize SceneConfiguration.
        /// </summary>
        static SceneConfiguration()
        {
            try
            {
                // populate from file
                XmlSerializer serializer = new XmlSerializer(typeof(SceneConfigurationToken));
                using (XmlReader reader = XmlReader.Create("Configuration/Scene.xml"))
                {
                    SceneConfigurationToken token = OxHelper.Cast<SceneConfigurationToken>(serializer.Deserialize(reader));
                    WaterReflectionMapSize = token.WaterReflectionMapSize;
                    DirectionalShadowMapSize = token.DirectionalShadowMapSize;
                    DirectionalShadowDepthBias = token.DirectionalShadowDepthBias;
                    DirectionalShadowRange = token.DirectionalShadowRange;
                    DirectionalLightCount = token.DirectionalLightCount;
                    DirectionalShadowCount = token.DirectionalShadowCount;
                    PointLightCount = token.PointLightCount;
                    DirectionalShadowSize = token.DirectionalShadowSize;
                    SceneConstructionDictionary = new ConstructionDictionary(token.SceneConstructionTokens);
                }
            }
            catch (FileNotFoundException)
            {
                // populate from code
                WaterReflectionMapSize = 1;
                DirectionalShadowMapSize = 2;
                DirectionalShadowDepthBias = 0.001f;
                DirectionalShadowRange = 512;
                DirectionalLightCount = 4;
                DirectionalShadowCount = 1;
                PointLightCount = 8;
                DirectionalShadowSize = new Vector2(1024, 1024);
                ConstructionTokens sceneConstructionTokens = new ConstructionTokens();
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.LightNamespace.AmbientLightToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.LightNamespace.DirectionalLightToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.LightNamespace.DirectionalLightWithShadowToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.LightNamespace.PointLightToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.SceneComponentToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.BasicModelToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.StandardModelToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.AnimatedModelToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.SkyboxToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.WaterToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.TerrainToken"));
                sceneConstructionTokens.Add(new ConstructionToken("Ox.Scene.dll", "Ox.Scene.Component.ParticleEmitterToken"));
                SceneConstructionDictionary = new ConstructionDictionary(sceneConstructionTokens);
            }
        }

        /// <summary>
        /// The size multiplier of the water drawing surface.
        /// </summary>
        public static readonly float WaterReflectionMapSize;
        /// <summary>
        /// The size multiplier of the directional shadow surface.
        /// </summary>
        public static readonly float DirectionalShadowMapSize;
        /// <summary>
        /// The bias applied during a depth test to determine is a pixel is in a shadow.
        /// </summary>
        public static readonly float DirectionalShadowDepthBias;
        /// <summary>
        /// The far range of all directional shadows (the near range is always 0).
        /// Changing this may require you to adjust DirectionalShadowDepthBias.
        /// </summary>
        public static readonly float DirectionalShadowRange;
        /// <summary>
        /// The number of directional lights that can light each scene component.
        /// </summary>
        public static readonly int DirectionalLightCount;
        /// <summary>
        /// The number of shadows that can be cast by each scene component.
        /// </summary>
        public static readonly int DirectionalShadowCount;
        /// <summary>
        /// The number of point lights that can light each scene component.
        /// </summary>
        public static readonly int PointLightCount;
        /// <summary>
        /// The size (width, height) of all directional shadows.
        /// </summary>
        public static readonly Vector2 DirectionalShadowSize;
        /// <summary>
        /// The construction dictionary used to create the scene component tokens.
        /// </summary>
        public static readonly ConstructionDictionary SceneConstructionDictionary;
        /// <summary>
        /// The string representation of the scene document type.
        /// </summary>
        public const string SceneDocumentType = "Scene";
    }
}
