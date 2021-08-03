using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.DefaultEngineNamespace;
using Ox.Engine;

namespace GameTemplate
{
    /// <summary>
    /// Implements an XNA program by inheriting XNA Game. Since Ox mostly deprecates the role of
    /// the Game class, your game logic should be placed in GameScript.cs instead of in here.
    /// </summary>
    /// <remarks>
    /// Even though this class inherits from XNA's Game class, it represents an XNA program instead
    /// of a game. Unlike the XNA demos you find at Microsoft's website, this class is the wrong
    /// place to put your game logic. Because Ox is script-driven, your game will be implemented as
    /// a script. Fortunately, this script has been generated for you in GameScript.cs. Open it
    /// and place your game logic where specified.
    /// 
    /// For more information, please see the documentation on Ox.Engine.OxEngine's Game property.
    /// </remarks>
    public class Program : Game
    {
        public Program()
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

        public const string DomainName = "Program";

        protected override void Initialize()
        {
            object[] engineParameters = new object[] { this, deviceManager, Assembly.GetExecutingAssembly() };
            engine = OxConfiguration.EngineConstructionToken.Construct<OxEngine>(engineParameters);
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

        private static void Main(string[] args)
        {
            using (Program program = new Program()) program.Run();
        }

        private readonly GraphicsDeviceManager deviceManager;
        private OxEngine engine;
        private bool disposed;
    }
}
