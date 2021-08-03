using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine
{
    /// <summary>
    /// A serializable token for OxConfiguration data.
    /// </summary>
    public class OxConfigurationToken
    {
        public Point VirtualResolution { get; set; }
        public ConstructionToken EngineConstructionToken { get; set; }
        public string ScriptsFileName { get; set; }
        public string ContentRootDirectory { get; set; }
        public string Platform { get; set; }
        public float SafeZoneMultiplier { get; set; }
        public bool Development { get; set; }
        public bool ReferenceDrawing { get; set; }
        [DefaultValue(1)] // missing from old config files
        public float DepthStencilBufferSize { get { return depthStencilBufferSize; } set { depthStencilBufferSize = value; } }
        public bool EncapsulateScriptedComponents { get; set; }
        public bool IsFixedTimeStep { get; set; }
        public ConstructionTokens GeneralConstructionTokens { get; set; }

        private float depthStencilBufferSize = 1;
    }

    /// <summary>
    /// Exposes a set of values used to configure Ox.
    /// </summary>
    public static class OxConfiguration
    {
        /// <summary>
        /// Initialize OxConfiguration.
        /// </summary>
        static OxConfiguration()
        {
            try
            {
                // populate from file
                XmlSerializer serializer = new XmlSerializer(typeof(OxConfigurationToken));
                using (XmlReader reader = XmlReader.Create("Configuration/Ox.xml"))
                {
                    OxConfigurationToken token = OxHelper.Cast<OxConfigurationToken>(serializer.Deserialize(reader));
                    VirtualResolution = token.VirtualResolution;
                    EngineConstructionToken = token.EngineConstructionToken;
                    ScriptsFileName = token.ScriptsFileName;
                    ContentRootDirectory = token.ContentRootDirectory;
                    Platform = token.Platform;
                    SafeZoneMultiplier = token.SafeZoneMultiplier;
                    Development = token.Development;
                    ReferenceDrawing = token.ReferenceDrawing;
                    DepthStencilBufferSize = token.DepthStencilBufferSize;
                    EncapsulateScriptedComponents = token.EncapsulateScriptedComponents;
                    IsFixedTimeStep = token.IsFixedTimeStep;
                    GeneralConstructionDictionary = new ConstructionDictionary(token.GeneralConstructionTokens);
                }
            }
            catch (FileNotFoundException)
            {
                // populate from code
                VirtualResolution = new Point(1280, 720);
                EngineConstructionToken = new ConstructionToken("Ox.DefaultEngine.dll", "Ox.DefaultEngineNamespace.DefaultEngine");
                ScriptsFileName = "Ox.Scripts.dll";
                ContentRootDirectory = "Content";
                Platform = "Windows";
                SafeZoneMultiplier = 0.9f;
                Development = true;
                ReferenceDrawing = false;
                DepthStencilBufferSize = 1;
                EncapsulateScriptedComponents = true;
                IsFixedTimeStep = true;
                ConstructionTokens generalConstructionTokens = new ConstructionTokens();
                generalConstructionTokens.Add(new ConstructionToken("Ox.Engine.dll", "Ox.Engine.Component.ComponentToken"));
                generalConstructionTokens.Add(new ConstructionToken("Ox.Engine.dll", "Ox.Engine.Component.UpdateableComponentToken"));
                generalConstructionTokens.Add(new ConstructionToken("Ox.Engine.dll", "Ox.Engine.Component.TransformableComponentToken"));
                GeneralConstructionDictionary = new ConstructionDictionary(generalConstructionTokens);
            }
            // populate dependant values
            VirtualResolutionFloat = new Vector2(VirtualResolution.X, VirtualResolution.Y);
            VirtualScreen = new Rect(0, 0, (float)VirtualResolution.X, (float)VirtualResolution.Y);
            ContentRootDirectoryAbsolute = Path.GetFullPath(Path.Combine(StorageContainer.TitleLocation, ContentRootDirectory));
            float inset = (1.0f - SafeZoneMultiplier) * 0.5f;
            SafeZone = new Rect(VirtualScreen.Scale * inset, VirtualScreen.Scale * SafeZoneMultiplier);
        }

        /// <summary>
        /// The virtual viewport for the standard coordinate system in floating-point.
        /// </summary>
        public static readonly Vector2 VirtualResolutionFloat;
        /// <summary>
        /// The minimum resolution the game may be set to.
        /// </summary>
        public static readonly Point MinimumResolution = new Point(1, 1);
        /// <summary>
        /// The virtual viewport for the standard coordinate system.
        /// </summary>
        public static readonly Point VirtualResolution;
        /// <summary>
        /// The rectangle that describes the virtual screen recttangle.
        /// </summary>
        public static readonly Rect VirtualScreen;
        /// <summary>
        /// Represents the virtual screen's safe zone.
        /// </summary>
        public static readonly Rect SafeZone;
        /// <summary>
        /// The type of platform for which the game is built.
        /// </summary>
        public static readonly string Platform;
        /// <summary>
        /// The construction information used to create the engine type.
        /// </summary>
        public static readonly ConstructionToken EngineConstructionToken;
        /// <summary>
        /// The name of the game plugin type.
        /// </summary>
        public static readonly string EngineTypeName;
        /// <summary>
        /// The name of the scripts assembly file.
        /// </summary>
        public static readonly string ScriptsFileName;
        /// <summary>
        /// The relative root directory where content is found.
        /// </summary>
        public static readonly string ContentRootDirectory;
        /// <summary>
        /// The absolute root directory where content is found.
        /// </summary>
        public static readonly string ContentRootDirectoryAbsolute;
        /// <summary>
        /// Is the game goint to be run in a development environment or in production?
        /// </summary>
        public static readonly bool Development;
        /// <summary>
        /// Should the game be drawn in reference mode (AKA, software drawing)?
        /// </summary>
        public static readonly bool ReferenceDrawing;
        /// <summary>
        /// The size multiplier of the depth stencil buffer.
        /// </summary>
        public static readonly float DepthStencilBufferSize;
        /// <summary>
        /// Should the document loader encapsulate script components by setting their names to
        /// default? Enable this to increase the encapsulation within your game. Disabled by
        /// default for backward-compatibility.
        /// </summary>
        public static readonly bool EncapsulateScriptedComponents;
        /// <summary>
        /// Should the game run in fixed time steps?
        /// </summary>
        public static readonly bool IsFixedTimeStep;
        /// <summary>
        /// The multiplier that represnts the percentage of the virtual screen to use for the
        /// safe zone.
        /// </summary>
        public static readonly float SafeZoneMultiplier;
        /// <summary>
        /// The construction dictionary used to create the general component tokens.
        /// </summary>
        public static readonly ConstructionDictionary GeneralConstructionDictionary;
        /// <summary>
        /// The string representation of the General document type.
        /// </summary>
        public const string GeneralDocumentType = "General";
        /// <summary>
        /// The name of the global domain.
        /// </summary>
        public const string GlobalDomainName = "Global";
        /// <summary>
        /// The string that separated a component's name from its guid.
        /// </summary>
        public const string NameGuidSeparator = ":";
        /// <summary>
        /// The default item name of a component token.
        /// </summary>
        public const string DefaultItemName = "(unnamed)";
        /// <summary>
        /// The total number of keyboard keys.
        /// </summary>
        public const int KeyCount = 256;
        /// <summary>
        /// The maximum number of players.
        /// </summary>
        public const int PlayerMax = 4;
    }
}
