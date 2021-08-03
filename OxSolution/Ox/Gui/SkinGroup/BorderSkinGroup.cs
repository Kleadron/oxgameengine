using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Gui.ViewElement;

namespace Ox.Gui.SkinGroup
{
    /// <summary>
    /// Controls the visual properties of a group of borders.
    /// </summary>
    public class BorderSkinGroup
    {
        /// <summary>
        /// Create a BorderSkinGroup.
        /// </summary>
        public BorderSkinGroup()
        {
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieceSkinGroups[i] = new QuickSpriteSkinGroup();
        }

        /// <summary>
        /// The thickness of each border sprite.
        /// </summary>
        public float Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        /// <summary>
        /// The drawing inset for each border sprite (used to prevent texture bleeding).
        /// </summary>
        public float Inset
        {
            get { return inset; }
            set { inset = value; }
        }

        /// <summary>
        /// Add a border to this group.
        /// </summary>
        public void AddBorder(Border border)
        {
            OxHelper.ArgumentNullCheck(border);
            for (int i = 0; i < (int)BorderPiece.Count; ++i) pieceSkinGroups[i].AddSprite(border.GetPiece((BorderPiece)i));
            SynchronizeBorder(border);
            borders.Add(border);
        }

        /// <summary>
        /// Remove a border from this group.
        /// </summary>
        public bool RemoveBorder(Border border)
        {
            OxHelper.ArgumentNullCheck(border);
            bool result = borders.Remove(border);
            if (result)
                for (int i = 0; i < (int)BorderPiece.Count; ++i)
                    pieceSkinGroups[i].RemoveSprite(border.GetPiece((BorderPiece)i));
            return result;
        }

        /// <summary>
        /// Get the specified border piece sprite.
        /// </summary>
        public QuickSpriteSkinGroup GetPieceSkinGroup(BorderPiece piece)
        {
            return pieceSkinGroups[(int)piece];
        }

        /// <summary>
        /// Synchronize the properties of the borders with those defined by this group.
        /// </summary>
        public void SynchronizeBorders()
        {
            for (int i = 0; i < borders.Count; ++i) SynchronizeBorder(borders[i]);
        }

        /// <summary>
        /// Decorate the border automatically. TODO: document better!
        /// </summary>
        public void Decorate(Vector2 tileOffset, Vector2 tileSize, string textureFileName)
        {
            pieceSkinGroups[(int)BorderPiece.Northwest].SourcePosition = new Vector2(0, 0);
            pieceSkinGroups[(int)BorderPiece.North].SourcePosition = new Vector2(tileSize.X, 0);
            pieceSkinGroups[(int)BorderPiece.Northeast].SourcePosition = new Vector2(tileSize.X * 2, 0);
            pieceSkinGroups[(int)BorderPiece.East].SourcePosition = new Vector2(tileSize.X * 2, tileSize.Y);
            pieceSkinGroups[(int)BorderPiece.Southeast].SourcePosition = new Vector2(tileSize.X * 2, tileSize.Y * 2);
            pieceSkinGroups[(int)BorderPiece.South].SourcePosition = new Vector2(tileSize.X, tileSize.Y * 2);
            pieceSkinGroups[(int)BorderPiece.Southwest].SourcePosition = new Vector2(0, tileSize.Y * 2);
            pieceSkinGroups[(int)BorderPiece.West].SourcePosition = new Vector2(0, tileSize.Y);
            for (int i = 0; i < (int)BorderPiece.Count; ++i)
            {
                QuickSpriteSkinGroup piece = pieceSkinGroups[i];
                piece.TextureFileName = textureFileName;
                piece.SourceSize = tileSize;
                piece.SourcePosition += tileOffset;
                piece.SynchronizeSprites();
            }
        }

        private void SynchronizeBorder(Border border)
        {
            border.Thickness = thickness;
            border.Inset = inset;
        }

        private readonly IList<Border> borders = new List<Border>();
        private readonly QuickSpriteSkinGroup[] pieceSkinGroups = new QuickSpriteSkinGroup[(int)BorderPiece.Count];
        private float thickness = 12;
        private float inset;
    }
}
