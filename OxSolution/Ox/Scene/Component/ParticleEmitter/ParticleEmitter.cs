using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Updates a particles emitter at the time of emission.
    /// </summary>
    public delegate void Emission(float secondsElapsed, ParticleEmitter emitter);
    /// <summary>
    /// Initializes a particle.
    /// </summary>
    public delegate void InitializeParticle(ParticleEmitter emitter, Particle particle);
    /// <summary>
    /// Updates a particle.
    /// </summary>
    public delegate void UpdateParticle(float secondsElapsed, Particle particle);

    /// <summary>
    /// Various default values use by ParticleEmitters.
    /// </summary>
    public static class ParticleEmitterDefaults
    {
        public const string EffectFileName = "Ox/Effects/oxPointSprite";
        public const string DiffuseMapFileName = "Ox/Textures/blue";
    }

    /// <summary>
    /// A particle emitter that can be placed in a scene.
    /// </summary>
    public class ParticleEmitter : SingleSurfaceComponent<ParticleEmitterSurface>
    {
        /// <summary>
        /// Create a ParticleEmitter.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public ParticleEmitter(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain, new CloneBoundingBoxBuilder())
        {
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateSurface<ParticleEmitterSurface>(this));
        }

        /// <summary>
        /// The delegate that defines what happens to the emitter when a particle is emitted.
        /// </summary>
        public Emission Emission
        {
            get { return surface.Emission; }
            set { surface.Emission = value; }
        }

        /// <summary>
        /// The delegate that defines what happens to a particle when it is emitted.
        /// May be null.
        /// </summary>
        public InitializeParticle InitializeParticle
        {
            get { return surface.InitializeParticle; }
            set { surface.InitializeParticle = value; }
        }

        /// <summary>
        /// The delegate that defines what happens when a particle is updated.
        /// May be null.
        /// </summary>
        public UpdateParticle UpdateParticle
        {
            get { return surface.UpdateParticle; }
            set { surface.UpdateParticle = value; }
        }

        /// <summary>
        /// The maximum scale any particle will become.
        /// </summary>
        public float ParticleScaleMax
        {
            get { return surface.ParticleScaleMax; }
            set { surface.ParticleScaleMax = value; }
        }

        /// <summary>
        /// The maximum number of particles that can be emitted.
        /// </summary>
        public int ParticleMax
        {
            get { return surface.ParticleMax; }
            set { surface.ParticleMax = value; }
        }

        /// <summary>
        /// The number of particles currently emitted.
        /// </summary>
        public int ActiveParticlesCount { get { return surface.ActiveParticlesCount; } }

        /// <summary>
        /// The rate at which particles are emitted.
        /// </summary>
        public int EmitRate
        {
            get { return surface.EmitRate; }
            set { surface.EmitRate = value; }
        }

        /// <inheritdoc />
        protected override ParticleEmitterSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new ParticleEmitterToken();
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            UpdateBoundingBox();
        }

        private void UpdateBoundingBox()
        {
            BoundingBox boundingBox = new BoundingBox();
            if (VisibleWorld) boundingBox = surface.GenerateBoundingBox();
            BoundingBoxBuilder.TrySetBoundingBox(boundingBox);
        }

        private readonly ParticleEmitterSurface surface;
    }
}
