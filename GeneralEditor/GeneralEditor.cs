using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.ServicesNamespace;
using SysWinForms = System.Windows.Forms;

namespace GeneralEditorNamespace
{
    public class GeneralEditor : Game
    {
        public GeneralEditor()
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

            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.FormBorderStyle = SysWinForms.FormBorderStyle.None;
            applicationForm.Shown += delegate(object sender, EventArgs e) { OxHelper.Cast<SysWinForms.Form>(sender).Hide(); };

            controller = new GeneralEditorController(engine, domainName, true) { Parent = engine.Root };

            view = new GeneralEditorForm(engine, controller);
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
            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.Bounds = view.Bounds;
            ResolutionManager resolutionManager = engine.ResolutionManager;
            resolutionManager.Resolution = new Point(applicationForm.Bounds.Width, applicationForm.Bounds.Height);
        }

        private const string domainName = "GeneralEditor";

        private readonly GraphicsDeviceManager deviceManager;
        private GeneralEditorController controller;
        private GeneralEditorForm view;
        private OxEngine engine;
        private bool disposed;
    }
}
