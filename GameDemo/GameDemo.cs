using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.DefaultEngineNamespace;
using Ox.Engine;
using Ox.Gui;

namespace GameDemoNamespace
{
    public class GameDemo : Game
    {
        public GameDemo()
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

        public const string DomainName = "GameDemo";

        protected override void Initialize()
        {
            object[] engineParameters = new object[] { this, deviceManager, Assembly.GetExecutingAssembly() };
            engine = OxConfiguration.EngineConstructionToken.Construct<OxEngine>(engineParameters);
            engine.GetService<GuiSystem>().ShowFrameRateLabel();
            engine.LoadDocument("GameDocument.xml", OxConfiguration.GeneralDocumentType, DomainName);
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
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            engine.Draw(gameTime);
        }

        private readonly GraphicsDeviceManager deviceManager;
        private OxEngine engine;
        private bool disposed;
    }
}
