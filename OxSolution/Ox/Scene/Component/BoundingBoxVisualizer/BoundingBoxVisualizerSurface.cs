using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.Utility;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of an IBoundingBoxVisualizer.
    /// </summary>
    public class BoundingBoxVisualizerSurface : Surface<BoundingBoxVisualizer>
    {
        /// <summary>
        /// Create a BoundingBoxVisualizerSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public BoundingBoxVisualizerSurface(OxEngine engine, BoundingBoxVisualizer component)
            : base(engine, component, "Ox/Effects/oxLine")
        {
            AddGarbage(effect = new BaseEffect(engine.GraphicsDevice, engine.Load<Effect>("Ox/Effects/oxLine", DomainName)));
            AddGarbage(vertexDeclaration = new ManagedVertexDeclaration(engine.GraphicsDevice, VertexPositionColor.VertexElements));
            DrawProperties = DrawProperties.None;
        }

        /// <summary>
        /// The color used to draw the bounding boxes.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
            }
        }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return string.Empty; }
            set { }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            BoundingBox bounds = camera.BoundingBox;

            sceneSystem.CollectComponents(nonBoundlessPredicate, bounds, cachedComponents);
            {
                EnsureVertexCapacity(cachedComponents);
                PopulateVertices(cachedComponents);
                DrawVertices(camera, cachedComponents);
            }
            cachedComponents.Clear();
        }

        private void DrawVertices(Camera camera, IList<SceneComponent> components)
        {
            if (components.Count == 0) return;
            BaseEffect baseEffect = OxHelper.Cast<BaseEffect>(Effect);
            Matrix identity = Matrix.Identity;
            baseEffect.PopulateTransform(camera, ref identity);

            baseEffect.Begin();
            {
                EffectPassCollection passes = baseEffect.CurrentTechnique.Passes;
                for (int i = 0; i < passes.Count; ++i)
                {
                    EffectPass pass = passes[i];
                    pass.Begin();
                    {
                        int lineCount = components.Count * verticesPerBox;
                        int primitiveCount = PrimitiveType.LineList.GetPrimitiveCount(lineCount);
                        vertexDeclaration.Activate();
                        Engine.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, primitiveCount);
                    }
                    pass.End();
                }
            }
            baseEffect.End();
        }

        private void PopulateVertices(IList<SceneComponent> components)
        {
            for (int i = 0; i < components.Count; ++i) PopulateVertices(components[i], i);
        }

        private void PopulateVertices(SceneComponent component, int boxIndex)
        {
            BoundingBox boundingBox = component.BoundingBoxWorld;
            Vector3 min = boundingBox.Min;
            Vector3 max = boundingBox.Max;
            int offset = boxIndex * verticesPerBox;
            vertices[offset + 0].Position = min;
            vertices[offset + 1].Position = new Vector3(max.X, min.Y, min.Z);
            vertices[offset + 2].Position = new Vector3(min.X, max.Y, min.Z);
            vertices[offset + 3].Position = new Vector3(max.X, max.Y, min.Z);
            vertices[offset + 4].Position = new Vector3(min.X, max.Y, max.Z);
            vertices[offset + 5].Position = max;
            vertices[offset + 6].Position = new Vector3(min.X, min.Y, max.Z);
            vertices[offset + 7].Position = new Vector3(max.X, min.Y, max.Z);
            vertices[offset + 8].Position = min;
            vertices[offset + 9].Position = new Vector3(min.X, max.Y, min.Z);
            vertices[offset + 10].Position = new Vector3(max.X, min.Y, min.Z);
            vertices[offset + 11].Position = new Vector3(max.X, max.Y, min.Z);
            vertices[offset + 12].Position = new Vector3(max.X, min.Y, max.Z);
            vertices[offset + 13].Position = max;
            vertices[offset + 14].Position = new Vector3(min.X, min.Y, max.Z);
            vertices[offset + 15].Position = new Vector3(min.X, max.Y, max.Z);
            vertices[offset + 16].Position = min;
            vertices[offset + 17].Position = new Vector3(min.X, min.Y, max.Z);
            vertices[offset + 18].Position = new Vector3(min.X, max.Y, min.Z);
            vertices[offset + 19].Position = new Vector3(min.X, max.Y, max.Z);
            vertices[offset + 20].Position = new Vector3(max.X, max.Y, min.Z);
            vertices[offset + 21].Position = max;
            vertices[offset + 22].Position = new Vector3(max.X, min.Y, min.Z);
            vertices[offset + 23].Position = new Vector3(max.X, min.Y, max.Z);
        }

        private void EnsureVertexCapacity(IList<SceneComponent> components)
        {
            int requiredCapacity = components.Count * verticesPerBox;
            if (requiredCapacity > vertices.Length) IncreaseVertexCapacity(requiredCapacity);
        }

        private void IncreaseVertexCapacity(int requiredCapacity)
        {
            vertices = new VertexPositionColor[requiredCapacity * 2]; // MAGICVALUE
            UpdateColor();
        }

        private void UpdateColor()
        {
            for (int i = 0; i < vertices.GetLength(0); ++i) vertices[i].Color = Color;
        }

        private static readonly Func<SceneComponent, bool> nonBoundlessPredicate = x => !x.Boundless;
        private const int verticesPerBox = 24;

        private readonly IList<SceneComponent> cachedComponents = new List<SceneComponent>();
        private readonly ManagedVertexDeclaration vertexDeclaration;
        private readonly Effect effect;
        private VertexPositionColor[] vertices = new VertexPositionColor[0];
        private Color _color = Color.Red;
    }
}
