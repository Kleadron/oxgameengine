using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.Component;

namespace Ox.Scripts
{
    public class ExampleParticles : ComponentScript<ParticleEmitter>
    {
        public ExampleParticles(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            emitterOrientation = new EularOrientation();
            emitterOrientation.OrientationChanged += delegate(EularOrientation sender, Matrix orientation)
            { Component.Orientation = orientation; };

            Component.ParticleMax = particleMax;
            Component.EmitRate = (int)(particleMax * agePerSecond);
            Component.Emission += this_Emission;
            Component.InitializeParticle += this_InitializeParticle;
            Component.UpdateParticle += this_UpdateParticle;
        }

        private void this_Emission(float secondsElapsed, ParticleEmitter emitter)
        {
            OxHelper.ArgumentNullCheck(emitter);
            float degAngle1 = (random.Next(emitterAngle * 2) - emitterAngle);
            float radAngle1 = MathHelper.ToRadians(degAngle1);
            emitterOrientation.Angle1 = radAngle1;
            float degAngle2 = (random.Next(emitterAngle * 2) - emitterAngle);
            float radAngle2 = MathHelper.ToRadians(degAngle2);
            emitterOrientation.Angle2 = radAngle2;
        }

        private void this_InitializeParticle(ParticleEmitter emitter, Particle particle)
        {
            OxHelper.ArgumentNullCheck(emitter, particle);
            particle.Age = 0;
            particle.Position = Component.PositionWorld;
            particle.Scale = initialScale;
            particle.Velocity = Vector3.Transform(initialVelocity, Component.OrientationWorld);
        }

        private void this_UpdateParticle(float secondsElapsed, Particle particle)
        {
            OxHelper.ArgumentNullCheck(particle);
            particle.Age += secondsElapsed * agePerSecond;
            particle.Position += particle.Velocity * secondsElapsed;
            particle.Velocity += gravity * secondsElapsed;
            particle.Scale = initialScale * (1.0f - particle.Age);
        }

        private static readonly Vector3 initialVelocity = Vector3.Up * 32;
        private static readonly Vector3 gravity = Vector3.Down * 9.8f;
        private const float agePerSecond = 1.5f;
        private const float initialScale = 8;
        private const int particleMax = 128;
        private const int emitterAngle = 90;

        private readonly EularOrientation emitterOrientation;
        private readonly Random random = new Random();
    }
}
