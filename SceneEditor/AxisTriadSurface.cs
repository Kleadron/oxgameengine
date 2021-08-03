using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace SceneEditorNamespace
{
    /// <summary>
    /// The surface of an AxisTriad.
    /// </summary>
    public class AxisTriadSurface : Surface<AxisTriad>
    {
        /// <summary>
        /// Create an AxisTriadSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public AxisTriadSurface(OxEngine engine, AxisTriad component)
            : base(engine, component, "Ox/Effects/oxLine")
        {
            AddGarbage(effect = new BaseEffect(engine.GraphicsDevice, engine.Load<Effect>("Ox/Effects/oxLine", DomainName)));
            AddGarbage(vertexDeclaration = new ManagedVertexDeclaration(engine.GraphicsDevice, VertexPositionColor.VertexElements));
            DrawProperties = DrawProperties.None;
        }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return string.Empty; }
            set { }
        }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return effect; } }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            /*if (drawMode != "Normal") return;

            xLine.Segment = new Segment(Vector3.Zero, Vector3.UnitX * lineLength);
            yLine.Segment = new Segment(Vector3.Zero, Vector3.UnitY * lineLength);
            zLine.Segment = new Segment(Vector3.Zero, Vector3.UnitZ * lineLength);

            Vector3 origin = Find3DOrigin(camera);

            xLine.ApplyTranslation(origin);
            yLine.ApplyTranslation(origin);
            zLine.ApplyTranslation(origin);

            vertices[0].Position = xLine.Start;
            vertices[1].Position = xLine.End;
            vertices[2].Position = yLine.Start;
            vertices[3].Position = yLine.End;
            vertices[4].Position = zLine.Start;
            vertices[5].Position = zLine.End;
            vertices[0].Color = xColor;
            vertices[1].Color = xColor;
            vertices[2].Color = yColor;
            vertices[3].Color = yColor;
            vertices[4].Color = zColor;
            vertices[5].Color = zColor;

            vertices[6].Position = xLine.End + Vector3.UnitX * labelOffset + xLabelLine1Start.X * camera.Right + xLabelLine1Start.Y * camera.Up;
            vertices[7].Position = xLine.End + Vector3.UnitX * labelOffset + xLabelLine1End.X * camera.Right + xLabelLine1End.Y * camera.Up;
            vertices[8].Position = xLine.End + Vector3.UnitX * labelOffset + xLabelLine2Start.X * camera.Right + xLabelLine2Start.Y * camera.Up;
            vertices[9].Position = xLine.End + Vector3.UnitX * labelOffset + xLabelLine2End.X * camera.Right + xLabelLine2End.Y * camera.Up;
            vertices[6].Color = xColor;
            vertices[7].Color = xColor;
            vertices[8].Color = xColor;
            vertices[9].Color = xColor;

            vertices[10].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine1Start.X * camera.Right + yLabelLine1Start.Y * camera.Up;
            vertices[11].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine1End.X * camera.Right + yLabelLine1End.Y * camera.Up;
            vertices[12].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine2Start.X * camera.Right + yLabelLine2Start.Y * camera.Up;
            vertices[13].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine2End.X * camera.Right + yLabelLine2End.Y * camera.Up;
            vertices[14].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine3Start.X * camera.Right + yLabelLine3Start.Y * camera.Up;
            vertices[15].Position = yLine.End + Vector3.UnitY * labelOffset + yLabelLine3End.X * camera.Right + yLabelLine3End.Y * camera.Up;
            vertices[10].Color = yColor;
            vertices[11].Color = yColor;
            vertices[12].Color = yColor;
            vertices[13].Color = yColor;
            vertices[14].Color = yColor;
            vertices[15].Color = yColor;

            vertices[16].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine1Start.X * camera.Right + zLabelLine1Start.Y * camera.Up;
            vertices[17].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine1End.X * camera.Right + zLabelLine1End.Y * camera.Up;
            vertices[18].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine2Start.X * camera.Right + zLabelLine2Start.Y * camera.Up;
            vertices[19].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine2End.X * camera.Right + zLabelLine2End.Y * camera.Up;
            vertices[20].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine3Start.X * camera.Right + zLabelLine3Start.Y * camera.Up;
            vertices[21].Position = zLine.End + Vector3.UnitZ * labelOffset + zLabelLine3End.X * camera.Right + zLabelLine3End.Y * camera.Up;
            vertices[16].Color = zColor;
            vertices[17].Color = zColor;
            vertices[18].Color = zColor;
            vertices[19].Color = zColor;
            vertices[20].Color = zColor;
            vertices[21].Color = zColor;

            Matrix identity = Matrix.Identity;
            BaseEffect baseEffect = OxHelper.Cast<BaseEffect>(effect);
            baseEffect.PopulateTransform(camera, ref identity);

            baseEffect.Begin();
            {
                EffectPassCollection passes = baseEffect.CurrentTechnique.Passes;
                for (int i = 0; i < passes.Count; ++i)
                {
                    EffectPass pass = passes[i];
                    pass.Begin();
                    {
                        vertexDeclaration.Activate();
                        Engine.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 11);
                    }
                    pass.End();
                }
            }
            baseEffect.End();*/
        }

        /*private Vector3 Find3DOrigin(Camera camera)
        {
            Viewport viewport = Engine.GraphicsDevice.Viewport;
            Vector2 viewportPosition = viewport.FromVirtual(new Vector2(viewport.Width - rightDistance, topDistance));
            Segment pickLine = viewport.ToWorld(camera, new Vector2(viewport.Width - rightDistance, topDistance));
            return pickLine.Start + camera.Forward * forwardOffset;
        }*/

        private static readonly Vector2 xLabelLine1Start = new Vector2(-labelWidth, labelHeight);
        private static readonly Vector2 xLabelLine1End = new Vector2(labelWidth, -labelHeight);
        private static readonly Vector2 xLabelLine2Start = new Vector2(-labelWidth, -labelHeight);
        private static readonly Vector2 xLabelLine2End = new Vector2(labelWidth, labelHeight);
        private static readonly Vector2 yLabelLine1Start = new Vector2(-labelWidth, labelHeight);
        private static readonly Vector2 yLabelLine1End = Vector2.Zero;
        private static readonly Vector2 yLabelLine2Start = new Vector2(labelWidth, labelHeight);
        private static readonly Vector2 yLabelLine2End = Vector2.Zero;
        private static readonly Vector2 yLabelLine3Start = new Vector2(0, -labelHeight);
        private static readonly Vector2 yLabelLine3End = Vector2.Zero;
        private static readonly Vector2 zLabelLine1Start = new Vector2(-labelWidth, labelHeight);
        private static readonly Vector2 zLabelLine1End = new Vector2(labelWidth, labelHeight);
        private static readonly Vector2 zLabelLine2Start = new Vector2(labelWidth, labelHeight);
        private static readonly Vector2 zLabelLine2End = new Vector2(-labelWidth, -labelHeight);
        private static readonly Vector2 zLabelLine3Start = new Vector2(-labelWidth, -labelHeight);
        private static readonly Vector2 zLabelLine3End = new Vector2(labelWidth, -labelHeight);
        private static readonly Color xColor = Color.Red;
        private static readonly Color yColor = Color.LimeGreen;
        private static readonly Color zColor = Color.Blue;
        private const float rightDistance = 60f;
        private const float topDistance = 60f;
        private const float forwardOffset = 0.1f;
        private const float lineLength = 0.06f;
        private const float labelOffset = 0.025f;
        private const float labelWidth = 0.008f;
        private const float labelHeight = 0.01f;

        private readonly VertexPositionColor[] vertices = new VertexPositionColor[22];
        private readonly ManagedVertexDeclaration vertexDeclaration;
        private readonly SegmentPrimitive xLine = new SegmentPrimitive();
        private readonly SegmentPrimitive yLine = new SegmentPrimitive();
        private readonly SegmentPrimitive zLine = new SegmentPrimitive();
        private readonly Effect effect;
    }
}
