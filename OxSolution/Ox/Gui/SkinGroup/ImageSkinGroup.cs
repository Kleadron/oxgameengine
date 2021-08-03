using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Utility;
using Ox.Gui.ViewElement;

namespace Ox.Gui.SkinGroup
{
    /// <summary>
    /// Controls the visual properties of a group of images.
    /// </summary>
    public class ImageSkinGroup : IReadIndexable<Image>
    {
        /// <inheritdoc />
        public Image this[int index] { get { return images[index]; } }

        /// <summary>
        /// The texture's position on the texture sheet.
        /// </summary>
        public Vector2 SourcePosition
        {
            get { return sourcePosition; }
            set { sourcePosition = value; }
        }

        /// <summary>
        /// The texture's size on the texture sheet.
        /// </summary>
        public Vector2 SourceSize
        {
            get { return sourceSize; }
            set { sourceSize = value; }
        }

        /// <summary>
        /// The file name of the texture used to draw the images.
        /// </summary>
        public string TextureFileName
        {
            get { return textureFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                textureFileName = value;
            }
        }

        /// <summary>
        /// The drawing inset for the image (prevents texture bleeding).
        /// </summary>
        public float Inset
        {
            get { return inset; }
            set { inset = value; }
        }

        /// <inheritdoc />
        public int Count { get { return images.Count; } }

        /// <summary>
        /// Add an image to this group.
        /// </summary>
        public void AddImage(Image image)
        {
            images.Add(image);
            SynchronizeImage(image);
        }

        /// <summary>
        /// Remove an image from this group.
        /// </summary>
        public bool RemoveImage(Image image)
        {
            return images.Remove(image);
        }

        /// <summary>
        /// Synchronize the properties of the images with those defined by this group.
        /// </summary>
        public void SynchronizeImages()
        {
            for (int i = 0; i < images.Count; ++i) SynchronizeImage(images[i]);
        }

        private void SynchronizeImage(Image image)
        {
            image.SourcePosition = sourcePosition;
            image.SourceSize = sourceSize;
            image.TextureFileName = textureFileName;
            image.Inset = inset;
        }

        private readonly IList<Image> images = new List<Image>();
        private Vector2 sourcePosition = Vector2.Zero;
        private Vector2 sourceSize = Vector2.One;
        private string textureFileName = "Ox/Textures/blue";
        private float inset;
    }
}
