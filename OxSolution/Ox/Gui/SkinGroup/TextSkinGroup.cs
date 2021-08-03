using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.Font;
using Ox.Gui.ViewElement;

namespace Ox.Gui.SkinGroup
{
    /// <summary>
    /// Controls the visual properties of text.
    /// </summary>
    public class TextSkinGroup : IReadIndexable<IText>
    {
        /// <summary>
        /// Create a TextSkinGroup with the specified font.
        /// </summary>
        public TextSkinGroup(IFont font)
        {
            OxHelper.ArgumentNullCheck(font);
            this.font = font;
        }

        /// <inheritdoc />
        public IText this[int index] { get { return texts[index]; } }

        /// <summary>
        /// The scale of each text's font.
        /// </summary>
        public Vector2 FontScale
        {
            get { return fontScale; }
            set { fontScale = value; }
        }

        /// <summary>
        /// The font used by each text.
        /// </summary>
        public IFont Font { get { return font; } }

        /// <summary>
        /// The drawing inset for each glyph (used to prevent texture bleeding).
        /// </summary>
        public float Inset
        {
            get { return inset; }
            set { inset = value; }
        }

        /// <inheritdoc />
        public int Count { get { return texts.Count; } }

        /// <summary>
        /// Add a text to this group.
        /// </summary>
        public void AddText(IText text)
        {
            OxHelper.ArgumentNullCheck(text);
            texts.Add(text);
            SyncronizeText(text);
        }

        /// <summary>
        /// Remove a text from this group.
        /// </summary>
        public bool RemoveText(IText text)
        {
            OxHelper.ArgumentNullCheck(text);
            return texts.Remove(text);
        }

        /// <summary>
        /// Synchronize the properties of the sprites with those defined by this group.
        /// </summary>
        public void SynchronizeTexts()
        {
            for (int i = 0; i < texts.Count; ++i) SyncronizeText(texts[i]);
        }

        private void SyncronizeText(IText text)
        {
            text.Scale = fontScale;
            text.Inset = inset;
        }

        private readonly IList<IText> texts = new List<IText>();
        private readonly IFont font;
        private Vector2 fontScale = new Vector2(16, 32);
        private float inset = 1.0f / 256;
    }
}
