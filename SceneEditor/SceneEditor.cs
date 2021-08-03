using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.ServicesNamespace;
using Ox.Gui;
using Ox.Scene.Component;
using SysWinForms = System.Windows.Forms;

namespace SceneEditorNamespace
{
    public class SceneEditor : Game
    {
        public SceneEditor()
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

            boundingBoxes = new BoundingBoxVisualizer(engine, domainName, true) { Parent = engine.Root };

            controller = new SceneEditorController(engine, domainName, true) { Parent = engine.Root };

            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.FormBorderStyle = SysWinForms.FormBorderStyle.None;
            applicationForm.Shown += delegate(object sender, EventArgs e) { OxHelper.Cast<SysWinForms.Form>(sender).Hide(); };

            view = new SceneEditorForm(engine, controller);
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
            engine.Update(gameTime);
            UpdateResolution();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            engine.Draw(gameTime);
            GraphicsDevice.Present(view.PanelHandle);
        }

        private void UpdateResolution()
        {
            SysWinForms.Form applicationForm = OxHelper.Cast<SysWinForms.Form>(SysWinForms.Form.FromHandle(Window.Handle));
            applicationForm.Bounds = view.PanelTransform;
            engine.ResolutionManager.Resolution = new Point(applicationForm.Bounds.Width, applicationForm.Bounds.Height);
        }

        private const string domainName = "SceneEditor";

        private readonly GraphicsDeviceManager deviceManager;
        private BoundingBoxVisualizer boundingBoxes;
        private SceneEditorController controller;
        private SceneEditorForm view;
        private OxEngine engine;
        private bool disposed;
    }
}
