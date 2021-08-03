using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Gui.QuickSpriteNamespace
{
    /// <summary>
    /// A collection of sprites that can be drawn with a single XNA Draw call.
    /// </summary>
    public class QuickSpriteBatch : Disposable2
    {
        /// <summary>
        /// Create a QuickSpriteBatch.
        /// </summary>
        /// <param name="device">The graphics device.</param>
        /// <param name="blendMode">See property BlendMode.</param>
        /// <param name="effect">See property Effect.</param>
        /// <param name="textureFileName">See property TextureFileName.</param>
        public QuickSpriteBatch(GraphicsDevice device, SpriteBlendMode blendMode, BaseEffect effect, string textureFileName)
        {
            OxHelper.ArgumentNullCheck(device, effect, textureFileName);
            this.device = device;
            this.blendMode = blendMode;
            this.effect = effect;
            this.textureFileName = textureFileName;
            AddGarbage(vertexDeclaration = new ManagedVertexDeclaration(device, VertexQuickSprite.VertexElements));
        }

        /// <summary>
        /// The sprites in the batch.
        /// </summary>
        public IList<QuickSprite> QuickSprites { get { return sprites; } }

        /// <summary>
        /// The manner in which the sprite will be blended.
        /// </summary>
        public SpriteBlendMode BlendMode { get { return blendMode; } }

        /// <summary>
        /// The shader effect used to draw the sprites.
        /// </summary>
        public BaseEffect Effect { get { return effect; } }

        /// <summary>
        /// The drawing Z of the sprites (or float.MaxValue if there are no sprites in this
        /// batch)
        /// </summary>
        public float Z { get { return sprites.Count != 0 ? sprites[0].Z : float.MaxValue; } }

        /// <summary>
        /// The file name of the texture used to render the sprites.
        /// </summary>
        public string TextureFileName { get { return textureFileName; } }

        /// <summary>
        /// Draw the sprites.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the sprites are viewed.</param>
        public void Draw(GameTime gameTime, Camera camera)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);
            EnsureVertexCapacity();
            EnsureIndexCapacity();
            CreateSpriteBatch();
            DrawSpriteBatch(camera);
        }

        /// <summary>
        /// Is a sprite eligible to add to the batch?
        /// </summary>
        public bool IsCompatible(QuickSprite sprite)
        {
            //OxHelper.ArgumentNullCheck(sprite); // OPTIMIZATION: dummied for speed            
            return
                (Z == float.MaxValue || sprite.Z == Z) &&
                sprite.BlendMode == blendMode &&
                sprite.TextureFileName == textureFileName;
        }

        private void CreateSpriteBatch()
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                // OPTIMIZATION: locally cache data from properties / calculations.
                QuickSprite sprite = sprites[i];
                Vector2 position = sprite.Position;
                Vector2 scale = sprite.Scale;
                Vector2 sourcePosition = sprite.SourcePosition;
                Vector2 sourceSize = sprite.SourceSize;
                Rect bounds = sprite.Bounds;
                float z = sprite.Z;
                float inset = sprite.Inset;
                int vertexOffset = i * 4;
                int indexOffset = i * 6;

                // factor in the inset for source position
                sourcePosition.X += inset * 0.5f;
                sourcePosition.Y += inset * 0.5f;

                // factor in the inset for source size
                sourceSize.X -= inset;
                sourceSize.Y -= inset;

                // factor in the bounds for position
                Vector2 postBoundsPosition = new Vector2(
                    Math.Max(position.X, bounds.Left),
                    Math.Max(position.Y, bounds.Top));

                // factor in the bounds for scale
                Vector2 postBoundsBottomRight = new Vector2(
                    Math.Min(position.X + scale.X, bounds.Right),
                    Math.Min(position.Y + scale.Y, bounds.Bottom));

                // make sure post bounds bottom right is >= postBoundsPosition.
                postBoundsBottomRight.X = Math.Max(postBoundsBottomRight.X, postBoundsPosition.X);
                postBoundsBottomRight.Y = Math.Max(postBoundsBottomRight.Y, postBoundsPosition.Y);

                // calculate post bounds size
                Vector2 postBoundsSize = postBoundsBottomRight - postBoundsPosition;

                // factor in the bounds for source position
                Vector2 loss = postBoundsPosition - position;
                Vector2 lossPercent = loss / scale;
                Vector2 sourceAdjustment = sourceSize * lossPercent;
                Vector2 postBoundsSourcePosition = sourcePosition + sourceAdjustment;

                // factor in the bounds for source scale...

                // ...set up some initial variables that are adjusted with the changes made to the
                // source position...
                Vector2 adjustedSourceSize = sourceSize - sourceAdjustment;
                Vector2 sizeAdjustment = postBoundsPosition - position;
                Vector2 adjustedSize = scale - sizeAdjustment;

                // ...calculate the post-bounds source scale.
                Vector2 adjustedLoss = adjustedSize - postBoundsSize;
                Vector2 multipler = Vector2.One - adjustedLoss / adjustedSize;
                Vector2 postBoundsSourceSize = adjustedSourceSize * multipler;

                // add source sprite to batch using a vertex generic format
                PopulateVertexPositions(vertices, vertexOffset, postBoundsPosition, postBoundsSize, z);
                PopulateVertexColors(vertices, vertexOffset, sprite);
                PopulateVertexEffectColors(vertices, vertexOffset, sprite);
                PopulateVertexTexture(vertices, vertexOffset, postBoundsSourcePosition, postBoundsSourceSize);
                PopulateIndices(indices, indexOffset, vertexOffset);
            }
        }

        private void DrawSpriteBatch(Camera camera)
        {
            if (sprites.Count == 0) return;

            device.RenderState.CullMode = CullMode.CullClockwiseFace;
            {
                BeginSpriteBlendMode(device, blendMode);
                {
                    Matrix world = Matrix.Identity;
                    effect.PopulateTransform(camera, ref world);
                    effect.Begin();
                    {
                        int vertexCount = sprites.Count * 4;
                        int indexCount = sprites.Count * 2;
                        vertexDeclaration.Activate();
                        EffectPassCollection passes = effect.CurrentTechnique.Passes;
                        for (int i = 0; i < passes.Count; ++i)
                        {
                            passes[i].Begin();
                            {
                                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                    vertices, 0, vertexCount, indices, 0, indexCount);
                            }
                            passes[i].End();
                        }
                    }
                    effect.End();
                }
                EndSpriteBlendMode(device);
            }
            device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }

        private static void BeginSpriteBlendMode(GraphicsDevice device, SpriteBlendMode blendMode)
        {
            device.RenderState.DepthBufferEnable = false;
            switch (blendMode)
            {
                case SpriteBlendMode.AlphaBlend: BeginAlphaBlendMode(device); break;
                case SpriteBlendMode.Additive: BeginAdditiveBlendMode(device); break;
                case SpriteBlendMode.None: BeginOpaqueMode(device); break;
            }
        }

        private static void BeginOpaqueMode(GraphicsDevice device)
        {
            device.RenderState.AlphaBlendEnable = false;
        }

        private static void BeginAlphaBlendMode(GraphicsDevice device)
        {
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Greater;
            device.RenderState.ReferenceAlpha = 0;
        }

        private static void BeginAdditiveBlendMode(GraphicsDevice device)
        {
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.SourceBlend = Blend.One;
            device.RenderState.DestinationBlend = Blend.One;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Always;
            device.RenderState.ReferenceAlpha = 0;
        }

        private static void EndSpriteBlendMode(GraphicsDevice device)
        {
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.SourceBlend = Blend.One;
            device.RenderState.DestinationBlend = Blend.Zero;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaFunction = CompareFunction.Always;
        }

        private static void PopulateVertexPositions(
            VertexQuickSprite[] vertices, int vertexOffset, Vector2 position, Vector2 scale, float z)
        {
            vertices[vertexOffset + 0].Position = new Vector3(position.X, z, position.Y);
            vertices[vertexOffset + 1].Position = new Vector3(position.X, z, position.Y + scale.Y);
            vertices[vertexOffset + 2].Position = new Vector3(position.X + scale.X, z, position.Y);
            vertices[vertexOffset + 3].Position = new Vector3(position.X + scale.X, z, position.Y + scale.Y);
        }

        private static void PopulateVertexColors(VertexQuickSprite[] vertices, int vertexOffset, QuickSprite sprite)
        {
            vertices[vertexOffset + 0].Color =
            vertices[vertexOffset + 1].Color =
            vertices[vertexOffset + 2].Color =
            vertices[vertexOffset + 3].Color = sprite.Color;
        }

        private static void PopulateVertexEffectColors(VertexQuickSprite[] vertices, int vertexOffset, QuickSprite sprite)
        {
            vertices[vertexOffset + 0].EffectColor =
            vertices[vertexOffset + 1].EffectColor =
            vertices[vertexOffset + 2].EffectColor =
            vertices[vertexOffset + 3].EffectColor = sprite.EffectColor;
        }

        private static void PopulateVertexTexture(
            VertexQuickSprite[] vertices, int vertexOffset, Vector2 sourcePosition, Vector2 sourceSize)
        {
            vertices[vertexOffset + 0].Texture = new Vector2(sourcePosition.X, sourcePosition.Y);
            vertices[vertexOffset + 1].Texture = new Vector2(sourcePosition.X, sourcePosition.Y + sourceSize.Y);
            vertices[vertexOffset + 2].Texture = new Vector2(sourcePosition.X + sourceSize.X, sourcePosition.Y);
            vertices[vertexOffset + 3].Texture = new Vector2(sourcePosition.X + sourceSize.X, sourcePosition.Y + sourceSize.Y);
        }

        private static void PopulateIndices(int[] indices, int indexOffset, int vertexOffset)
        {
            indices[indexOffset + 0] = 0 + vertexOffset;
            indices[indexOffset + 1] = 1 + vertexOffset;
            indices[indexOffset + 2] = 2 + vertexOffset;
            indices[indexOffset + 3] = 1 + vertexOffset;
            indices[indexOffset + 4] = 3 + vertexOffset;
            indices[indexOffset + 5] = 2 + vertexOffset;
        }

        private void EnsureVertexCapacity()
        {
            int requiredCapacity = sprites.Count * 4;
            if (requiredCapacity > vertices.Length) vertices = new VertexQuickSprite[requiredCapacity * 2]; // MAGICVALUE
        }

        private void EnsureIndexCapacity()
        {
            int requiredCapacity = sprites.Count * 6;
            if (requiredCapacity > indices.Length) indices = new int[requiredCapacity * 2]; // MAGICVALUE
        }

        private readonly IList<QuickSprite> sprites = new List<QuickSprite>();
        private readonly ManagedVertexDeclaration vertexDeclaration;
        private readonly SpriteBlendMode blendMode;
        private readonly GraphicsDevice device;
        private readonly BaseEffect effect;
        private readonly string textureFileName;
        private VertexQuickSprite[] vertices = new VertexQuickSprite[0];
        private int[] indices = new int[0];
    }
}
