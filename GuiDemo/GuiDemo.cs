using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.DefaultEngineNamespace;
using Ox.Engine;
using Ox.Engine.MathNamespace;
using Ox.Gui;
using Ox.Gui.Component;

namespace GuiDemoNamespace
{
    public class GuiDemo : Game
    {
        /// <summary>
        /// Create a GuiDemo.
        /// </summary>
        public GuiDemo()
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

        /// <summary>
        /// The domain in which to store the demo's components.
        /// </summary>
        public const string DomainName = "GuiDemo";

        /// <inheritdoc />
        protected override void Initialize()
        {
            CreateEngine();
            ShowFrameRateLabel();
            CreateGuiComponents();
            SaveGuiComponents();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);            
            engine.Update(gameTime);
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            engine.Draw(gameTime);
        }

        /// <summary>
        /// Create the game engine.
        /// </summary>
        private void CreateEngine()
        {
            object[] engineParameters = new object[] { this, deviceManager, Assembly.GetExecutingAssembly() };
            engine = OxConfiguration.EngineConstructionToken.Construct<OxEngine>(engineParameters);   
        }

        /// <summary>
        /// Show the frame rate label.
        /// </summary>
        private void ShowFrameRateLabel()
        {
            engine.GetService<GuiSystem>().ShowFrameRateLabel();
        }

        /// <summary>
        /// Create the gui component to compose a simple dialog.
        /// </summary>
        private void CreateGuiComponents()
        {
            GuiSystem guiSystem = engine.GetService<GuiSystem>();

            Panel screen = new Panel(engine, DomainName, true) { Parent = engine.Root };
            screen.Scale = OxConfiguration.VirtualResolutionFloat;
            screen.Color = new Color();
            engine.GetService<GuiSystem>().Screen = screen;
            {
                Dialog dialog = new Dialog(engine, DomainName, true) { Parent = screen };
                dialog.Scale = new Vector2(384, 564);
                dialog.Position = new Vector2(192, 64);
                dialog.Text = "It's a dialog :)";
                dialog.CloseClicked += x => Exit();
                {
                    Button buttonShowBorder = new Button(engine, DomainName, true) { Parent = dialog };
                    buttonShowBorder.Scale = new Vector2(352, 56);
                    buttonShowBorder.Position = new Vector2(16, 56);
                    buttonShowBorder.Text = "Make Root Blue";
                    buttonShowBorder.Clicked += x => screen.Color = Color.Blue;
                    buttonShowBorder.FocusByOtherInput();

                    Button buttonHideBorder = new Button(engine, DomainName, true) { Parent = dialog };
                    buttonHideBorder.Scale = new Vector2(352, 56);
                    buttonHideBorder.Position = new Vector2(16, 128);
                    buttonHideBorder.Text = "Make Root Red";
                    buttonHideBorder.Clicked += x => screen.Color = Color.Red;

                    FillBar fillBar = new FillBar(engine, DomainName, true) { Parent = dialog };
                    fillBar.Scale = new Vector2(352, 56);
                    fillBar.Position = new Vector2(16, 200);
                    fillBar.FillMode = Direction2D.Right;
                    fillBar.Fill = 0.333f;

                    string radioButtonGroup = "RadioButtonGroup";
                    {
                        RadioButton radioButton = new RadioButton(engine, DomainName, true) { Parent = dialog };
                        radioButton.RadioButtonGroup = radioButtonGroup;
                        radioButton.Position = new Vector2(16, 272);

                        RadioButton anotherRadioButton = new RadioButton(engine, DomainName, true) { Parent = dialog };
                        anotherRadioButton.RadioButtonGroup = radioButtonGroup;
                        anotherRadioButton.Position = new Vector2(16, 344);
                    }

                    TextBox textBox = new TextBox(engine, DomainName, true) { Parent = dialog };
                    textBox.CharacterLimit = 18;
                    textBox.Scale = new Vector2(352, 56);
                    textBox.Position = new Vector2(16, 416);

                    Button msgButton = new Button(engine, DomainName, true) { Parent = dialog };
                    msgButton.Scale = new Vector2(352, 56);
                    msgButton.Position = new Vector2(16, 488);
                    msgButton.Text = "Pop Up!";
                    msgButton.Clicked += x => guiSystem.ShowMessageBox(MessageBoxType.OkOnly, "Pop Up Hideo!", "This be yer message, arr!", null, msgButton);
                }
            }
        }

        /// <summary>
        /// Save a gui document with all the gui component instances created above.
        /// You can open the save document in the gui editor or load it at play-time.
        /// </summary>
        private void SaveGuiComponents()
        {
            var document = new GuiDocument(engine);
            var instances = engine.CollectComponents(x => x.DomainName == DomainName && x.OwnedByDomain, new List<BaseGuiComponent>());
            document.CreateComponentsFrom(instances);
            document.Save("GuiDemo.xml");
        }

        private readonly GraphicsDeviceManager deviceManager;
        private OxEngine engine;
        private bool disposed;
    }
}
