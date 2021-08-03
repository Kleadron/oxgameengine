using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.EffectNamespace;
using Ox.Scene.FogNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of an ParticleEmitter.
    /// </summary>
    public class ParticleEmitterSurface : Surface<ParticleEmitter>
    {
        /// <summary>
        /// Create a ParticleEmitterSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public ParticleEmitterSurface(OxEngine engine, ParticleEmitter component)
            : base(engine, component, ParticleEmitterDefaults.EffectFileName)
        {
            AddGarbage(vertexDeclaration = new ManagedVertexDeclaration(engine.GraphicsDevice, VertexPointSprite.VertexElements));
            DiffuseMapFileName = ParticleEmitterDefaults.DiffuseMapFileName;
            ParticleMax = defaultParticleMax;
        }
        
        /// <summary>
        /// The manner in which the particles will be blended into the background and each other.
        /// </summary>
        public SpriteBlendMode BlendMode
        {
            get { return blendMode; }
            set { blendMode = value; }
        }
        
        /// <summary>
        /// The delegate that defines what happens to the emitter when a particle is emitted.
        /// </summary>
        public Emission Emission
        {
            get { return emission; }
            set { emission = value; }
        }
        
        /// <summary>
        /// The delegate that defines what happens to a particle when it is emitted.
        /// May be null.
        /// </summary>
        public InitializeParticle InitializeParticle
        {
            get { return initializeParticle; }
            set { initializeParticle = value; }
        }
        
        /// <summary>
        /// The delegate that defines what happens when a particle is updated.
        /// May be null.
        /// </summary>
        public UpdateParticle UpdateParticle
        {
            get { return updateParticle; }
            set { updateParticle = value; }
        }
        
        /// <summary>
        /// The name of the diffuse map file.
        /// </summary>
        public string DiffuseMapFileName
        {
            get { return _diffuseMapFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMapFileName == value) return; // OPTIMIZATION
                Texture2D newTexture = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap = newTexture;
                _diffuseMapFileName = value;
            }
        }
        
        /// <summary>
        /// The maximum scale any particle will become.
        /// </summary>
        public float ParticleScaleMax
        {
            get { return particleScaleMax; }
            set { particleScaleMax = value; }
        }
        
        /// <summary>
        /// The maximum number of particles that can be emitted.
        /// </summary>
        public int ParticleMax
        {
            get { return _particleMax; }
            set
            {
                value = (int)MathHelper.Clamp(value, 0, int.MaxValue); // VALIDATION
                if (_particleMax == value && pool != null) return; // OPTIMIZATION
                _particleMax = value;
                if (pool != null)
                {
                    RemoveGarbage(pool);
                    pool.Dispose();
                }
                AddGarbage(pool = new FifoPool<Particle>(value, createParticle, cleanupParticle));
                cachedPositions = new Vector3[value];
            }
        }
        
        /// <summary>
        /// The number of particles currently emitted.
        /// </summary>
        public int ActiveParticlesCount
        {
            get { return pool.ActiveCount; }
        }
        
        /// <summary>
        /// The rate at which particles are emitted.
        /// </summary>
        public int EmitRate
        {
            get { return _emitRate; }
            set
            {
                value = (int)MathHelper.Clamp(value, 0, int.MaxValue); // VALIDATION
                _emitRate = value;
            }
        }

        /// <summary>
        /// Generate a bounding box that contains all the active particles.
        /// </summary>
        public BoundingBox GenerateBoundingBox()
        {
            if (pool.ActiveCount == 0) return new BoundingBox(
                PositionWorld - new Vector3(0.5f),
                PositionWorld + new Vector3(0.5f));

            int i = 0;

            // stream positions from activeParticles to cachedPositions
            pool.CollectActiveItems(cachedParticles);
            {
                for (int j = 0; j < cachedParticles.Count; ++j)
                {
                    cachedPositions[i] = cachedParticles[j].Position;
                    ++i;
                }
            }
            cachedParticles.Clear();

            // populate any unused for positions with the first cached position
            Vector3 firstParticlePosition = cachedPositions[0];
            for (; i < cachedPositions.Length; ++i) cachedPositions[i] = firstParticlePosition;

            // calculate the bounding box that contains all the positions
            BoundingBox result = cachedPositions.GenerateBoundingBox();

            // increase the scale of box so it overlaps the edges of the particles
            Vector3 overlap = new Vector3(particleScaleMax * 0.5f);
            result.Max += overlap;
            result.Min -= overlap;

            return result;
        }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return _effectFileName; }
            set
            {
                if (_effectFileName == value) return; // OPTIMIZATION
                Effect newEffect = CreateEffect(value);
                // EXCEPTIONSAFETYLINE
                _effect = newEffect;
                _effectFileName = value;
            }
        }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return _effect; } }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            float secondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateActiveParticles(secondsElapsed);
            UpdateEmission(secondsElapsed);
            UpdateParticleDeath();
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            // OPTIMIZATION: cache properties
            GraphicsDevice device = Engine.GraphicsDevice;
            FogReceiverEffect frEffect = OxHelper.Cast<FogReceiverEffect>(Effect);
            EffectPassCollection passes = frEffect.CurrentTechnique.Passes;

            pool.CollectActiveItems(cachedParticles);
            {
                if (cachedParticles.Count != 0)
                {
                    EnsureVertexCapacity();
                    PopulateVertices();
                    PopulateEffect(camera, frEffect);

                    BeginRenderState();
                    {
                        frEffect.Begin();
                        {
                            for (int i = 0; i < passes.Count; ++i)
                            {
                                EffectPass pass = passes[i]; pass.Begin();
                                {
                                    vertexDeclaration.Activate();
                                    device.DrawUserPrimitives(PrimitiveType.PointList, vertices, 0, cachedParticles.Count);
                                }
                                pass.End();
                            }
                        }
                        frEffect.End();
                    }
                    EndRenderState();
                }
            }
            cachedParticles.Clear();
        }

        private Texture2D DiffuseMap { get { return _diffuseMap; } }

        private void UpdateActiveParticles(float secondsElapsed)
        {
            pool.CollectActiveItems(cachedParticles);
            {
                for (int i = 0; i < cachedParticles.Count; ++i)
                    if (updateParticle != null)
                        updateParticle(secondsElapsed, cachedParticles[i]);
            }
            cachedParticles.Clear();
        }

        private void UpdateEmission(float secondsElapsed)
        {
            float timeToProcess = secondsElapsed + leftoverTime;
            if (EmitRate == 0 || timeToProcess == 0) return;

            float timeSlice = 1.0f / _emitRate;
            int emitCount = (int)(timeToProcess / timeSlice);
            float timeOfLastEmission = emitCount * timeSlice;
            float timeProcessed = timeSlice;
            leftoverTime = timeToProcess - timeOfLastEmission;

            while (timeProcessed <= timeOfLastEmission)
            {
                EmitParticle(timeToProcess - timeProcessed);
                if (emission != null) emission(timeSlice, Component);
                timeProcessed += timeSlice;
            }
        }

        private void UpdateParticleDeath()
        {
            pool.CollectActiveItems(cachedParticles);
            {
                for (int i = 0; i < cachedParticles.Count; ++i)
                    if (cachedParticles[i].Age >= 1.0f)
                        pool.Free();
            }
            cachedParticles.Clear();
        }

        private void EnsureVertexCapacity()
        {
            int requiredCapacity = cachedParticles.Count;
            if (requiredCapacity > vertices.Length) vertices = new VertexPointSprite[requiredCapacity * 2]; // MAGICVALUE
        }

        private void EmitParticle(float timeToCatchup)
        {
            Particle newParticle = pool.Allocate();
            if (newParticle == null) return;
            if (initializeParticle != null) initializeParticle(Component, newParticle);
            if (updateParticle != null) updateParticle(timeToCatchup, newParticle);
        }

        private void PopulateEffect(Camera camera, FogReceiverEffect effect)
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            effect.TrySetCurrentTechnique("Normal");
            effect.DiffuseMap = DiffuseMap;
            effect.Parameters["xViewportHeight"].TrySetValue(device.Viewport.Height);
            Matrix world = Matrix.Identity;
            Fog fog = Engine.GetService<SceneSystem>().Fog;
            effect.PopulateTransform(camera, ref world);
            effect.PopulateFogging(fog);
        }

        private void BeginRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.BeginFaceMode(FaceMode.AllFaces);
            device.RenderState.PointSpriteEnable = true;
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.DepthBufferWriteEnable = true;
            switch (blendMode)
            {
                case SpriteBlendMode.AlphaBlend: BeginAlphaBlendMode(); break;
                case SpriteBlendMode.Additive: BeginAdditiveBlendMode(); break;
                case SpriteBlendMode.None: BeginOpaqueMode(); break;
            }
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.RenderState.PointSpriteEnable = false;
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.SourceBlend = Blend.One;
            device.RenderState.DestinationBlend = Blend.Zero;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaFunction = CompareFunction.Always;
            device.EndFaceMode();
        }

        private void BeginOpaqueMode()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.RenderState.AlphaBlendEnable = false;
        }

        private void BeginAlphaBlendMode()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Greater;
            device.RenderState.ReferenceAlpha = 0;
        }

        private void BeginAdditiveBlendMode()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.SourceBlend = Blend.One;
            device.RenderState.DestinationBlend = Blend.One;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Always;
            device.RenderState.ReferenceAlpha = 0;
        }

        private void PopulateVertices()
        {
            for (int i = 0; i < cachedParticles.Count; ++i)
            {
                Particle particle = cachedParticles[i];
                vertices[i] = new VertexPointSprite(particle.Position, particle.Scale, particle.Color);
            }
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new FogReceiverEffect(Engine.GraphicsDevice, effectFromDisk);
        }
        
        private static readonly CleanupResource<Particle> cleanupParticle = delegate { };
        private static readonly CreateResource<Particle> createParticle = delegate { return new Particle(); };
        private const int defaultParticleMax = 32;

        private readonly IList<Particle> cachedParticles = new List<Particle>();
        private readonly ManagedVertexDeclaration vertexDeclaration;
        private FifoPool<Particle> pool;
        private VertexPointSprite[] vertices = new VertexPointSprite[0];
        private Vector3[] cachedPositions;
        private SpriteBlendMode blendMode = SpriteBlendMode.AlphaBlend;
        /// <summary>May be null.</summary>
        private Emission emission;
        /// <summary>May be null.</summary>
        private UpdateParticle updateParticle;
        /// <summary>May be null.</summary>
        private InitializeParticle initializeParticle;
        private float particleScaleMax = 1;
        private float leftoverTime;
        private Texture2D _diffuseMap;
        private Effect _effect;
        private string _effectFileName;
        private string _diffuseMapFileName;
        private int _particleMax;
        private int _emitRate;
    }
}
