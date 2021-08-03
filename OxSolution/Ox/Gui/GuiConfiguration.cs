using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;

namespace Ox.Gui
{
    /// <summary>
    /// A serializable token for GuiConfiguration.
    /// </summary>
    public class GuiConfigurationToken
    {
        public ConstructionTokens GuiConstructionTokens { get; set; }
    }

    /// <summary>
    /// Exposes a set of values used to configure the gui system.
    /// </summary>
    public static class GuiConfiguration
    {
        /// <summary>
        /// Initialize GuiConfiguration.
        /// </summary>
        static GuiConfiguration()
        {
            try
            {
                // populate from file
                XmlSerializer serializer = new XmlSerializer(typeof(GuiConfigurationToken));
                using (XmlReader reader = XmlReader.Create("Configuration/Gui.xml"))
                {
                    GuiConfigurationToken token = OxHelper.Cast<GuiConfigurationToken>(serializer.Deserialize(reader));
                    GuiConstructionDictionary = new ConstructionDictionary(token.GuiConstructionTokens);
                }
            }
            catch (FileNotFoundException)
            {
                // populate from code
                ConstructionTokens guiConstructionTokens = new ConstructionTokens();
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.GuiComponentToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.ButtonToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.CheckBoxToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.DialogToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.FillBarToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.LabelToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.PanelToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.RadioButtonToken"));
                guiConstructionTokens.Add(new ConstructionToken("Ox.Gui.dll", "Ox.Gui.Component.TextBoxToken"));
                GuiConstructionDictionary = new ConstructionDictionary(guiConstructionTokens);
            }
        }

        /// <summary>
        /// The position of the frame rate label on the screen.
        /// </summary>
        public static readonly Vector2 FrameRateLabelPosition = new Vector2(64);
        /// <summary>
        /// The scale of the frame rate label on the screen.
        /// </summary>
        public static readonly Vector2 FrameRateLabelScale = new Vector2(128, 32);
        /// <summary>
        /// The scale of the frame rate label text.
        /// </summary>
        public static readonly Vector2 FrameRateLabelTextScale = new Vector2(16, 32);
        /// <summary>
        /// The construction dictionary used to create the gui component tokens.
        /// </summary>
        public static readonly ConstructionDictionary GuiConstructionDictionary;
        /// <summary>
        /// The string representation of the gui document type.
        /// </summary>
        public const string GuiDocumentType = "Gui";
        /// <summary>
        /// The standard Z difference between a parent and child gui component.
        /// </summary>
        public const float ZGap = 0.001f;
    }
}
