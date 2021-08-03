using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.DefaultEngineNamespace;
using Ox.Engine;
using Ox.Gui;
using Ox.Scene.Component;
using Ox.Scene.LightNamespace;

namespace PhysicsDemoNamespace
{
    public class PhysicsDemo : Game
    {
        public PhysicsDemo()
        {
            deviceManager = new GraphicsDeviceManager(this);
            deviceManager.SynchronizeWithVerticalRetrace = false;
            deviceManager.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs eventargs)
            {
                if (OxConfiguration.ReferenceDrawing)
                {
                    eventargs.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
                    eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.None;
                }
            };
        }

        public const string DomainName = "PhysicsDemo";

        protected override void Initialize()
        {
            object[] engineParameters = new object[] { this, deviceManager, Assembly.GetExecutingAssembly() };
            engine = OxConfiguration.EngineConstructionToken.Construct<OxEngine>(engineParameters);
            engine.GetService<GuiSystem>().ShowFrameRateLabel();
            terrain = new PhysicsTerrain(engine, DomainName, true) { Parent = engine.Root };
            skybox = new Skybox(engine, DomainName, true) { Parent = engine.Root };
            directionalLight = new DirectionalLight(engine, DomainName, true, false) { Parent = engine.Root };
            ambientLight = new AmbientLight(engine, DomainName, true) { Parent = engine.Root, Color = Color.Cyan };
            CreateBalls();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    engine.DestroyDomain(DomainName);
                    engine.Dispose();
                }
                disposed = true;
            }
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            engine.Update(gameTime);
            UpdateCamera();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            engine.Draw(gameTime);
        }

        private void CreateBalls()
        {
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < 10; ++j)
                    balls.Add(new PhysicsBall(engine, DomainName, true)
                    {
                        Parent = engine.Root,
                        Position = Vector3.Right * 16 * i + Vector3.Up * 256 + Vector3.Backward * 16 * j
                    });
        }

        private void UpdateCamera()
        {
            engine.Camera.SetTransformByLookForward(balls[balls.Count - 1].Position, Vector3.Up, Vector3.Forward);
        }

        private readonly IList<PhysicsBall> balls = new List<PhysicsBall>();
        private readonly GraphicsDeviceManager deviceManager;
        private DirectionalLight directionalLight;
        private PhysicsTerrain terrain;
        private AmbientLight ambientLight;
        private OxEngine engine;
        private Skybox skybox;
        private bool disposed;
    }
}
