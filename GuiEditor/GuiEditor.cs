using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Gui;
using SysWinForms = System.Windows.Forms;
using Ox.Engine.ServicesNamespace;

namespace GuiEditorNamespace
{
    public class GuiEditor : Game
    {
        public GuiEditor()
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

        protected override void Initialize()
        {
            object[] engineParameters = new object[] { this, deviceManager, Assembly.GetExecutingAssembly() };
            engine = OxConfiguration.EngineConstructionToken.Construct<OxEngine>(engineParameters);

            GuiSystem guiSystem = engine.GetService<GuiSystem>();
            guiSystem.ShowFrameRateLabel();
            guiSystem.HideMouse();

            controller = new GuiEditorController(engine, domainName, true) { Parent = engine.Root };

            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.FormBorderStyle = SysWinForms.FormBorderStyle.None;
            applicationForm.Shown += delegate(object sender, EventArgs e) { OxHelper.Cast<SysWinForms.Form>(sender).Hide(); };

            view = new GuiEditorForm(engine, controller);
            view.HandleDestroyed += delegate { Exit(); };
            view.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    engine.DestroyDomain(domainName);
                    engine.Dispose();
                }
                disposed = true;
            }
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            controller.ActivateEventPanel();
            engine.Update(gameTime);
            UpdateResolution();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            controller.ActivateDocumentPanel();
            engine.Draw(gameTime);
            GraphicsDevice.Present(view.PanelHandle);
        }

        private void UpdateResolution()
        {
            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.Bounds = view.PanelTransform;
            engine.ResolutionManager.Resolution = new Point(applicationForm.Bounds.Width, applicationForm.Bounds.Height);
        }

        private const string domainName = "GuiEditor";

        private readonly GraphicsDeviceManager deviceManager;
        private GuiEditorController controller;
        private GuiEditorForm view;
        private OxEngine engine;
        private bool disposed;
    }
}
