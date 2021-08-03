using System.Reflection;
using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.RenderTarget;
using Ox.Engine.ServicesNamespace;
using Ox.Engine.Spatial;
using Ox.Engine.Utility;
using Ox.Gui;
using Ox.Gui.Component;
using Ox.Gui.QuickSpriteNamespace;
using Ox.Scene;
using Ox.Scene.Component;
using Ox.Scene.SurfaceNamespace;

namespace Ox.DefaultEngineNamespace
{
    /// <summary>
    /// A default implementation of the Ox game engine. This implementation is fine for starting
    /// out, but if (when) you need to alter the internal workings of Ox to fit your specific
    /// needs, you should override its hook methods or create a different subclass of OxEngine.
    /// TODO: move keyboard context and frame rater to OxEngine.
    /// TODO: clean out the useless hooks.
    /// </summary>
    public class DefaultEngine : OxEngine
    {
        /// <summary>
        /// Create a DefaultEngine.
        /// </summary>
        /// <param name="game">The XNA game object.</param>
        /// <param name="deviceManager">The graphics device manager</param>
        /// <param name="entryAssembly">The entry assembly (where 'static void main' resides).</param>
        public DefaultEngine(Game game, GraphicsDeviceManager deviceManager, Assembly entryAssembly)
            : base(game, deviceManager, entryAssembly)
        {
            CreateDepthStencilBuffer();
            CreateBackBuffer();
            CreateGeneralServices();
            CreateGuiServices();
            CreateSceneServices();
            CreatePhysicsServices();
        }

        /// <summary>
        /// The depth stencil buffer.
        /// </summary>
        protected ManagedDepthStencilBuffer DepthStencilBuffer { get { return depthStencilBuffer; } }

        /// <summary>
        /// The back buffer.
        /// </summary>
        protected ManagedBackBuffer BackBuffer { get { return backBuffer; } }

        /// <summary>
        /// The quick sprite drawer.
        /// </summary>
        protected QuickSpriteDrawer QuickSpriteDrawer { get { return quickSpriteDrawer; } }

        /// <summary>
        /// The keyboard context.
        /// </summary>
        protected KeyboardContext KeyboardContext { get { return keyboardContext; } }

        /// <summary>
        /// The gui skin groups.
        /// </summary>
        protected GuiSkinGroups GuiSkinGroups { get { return guiSkinGroups; } }

        /// <summary>
        /// The physics system.
        /// </summary>
        protected PhysicsSystem PhysicsSystem { get { return physicsSystem; } }

        /// <summary>
        /// The scene system.
        /// </summary>
        protected SceneSystem SceneSystem { get { return sceneSystem; } }
        
        /// <summary>
        /// The frame rater.
        /// </summary>
        protected FrameRater FrameRater { get { return frameRater; } }
        
        /// <summary>
        /// The gui system.
        /// </summary>
        protected GuiSystem GuiSystem { get { return guiSystem; } }

        /// <summary>
        /// An overridable KeyboardContext creation hook.
        /// </summary>
        protected virtual KeyboardContext CreateKeyboardContextHook()
        {
            return new KeyboardContext(this);
        }

        /// <summary>
        /// An overridable FrameRater creation hook.
        /// </summary>
        protected virtual FrameRater CreateFrameRaterHook()
        {
            return new FrameRater();
        }

        /// <summary>
        /// An overridable VertexFactory creation hook.
        /// </summary>
        protected virtual VertexFactory CreateVertexFactoryHook()
        {
            return new VertexFactory();
        }

        /// <summary>
        /// An overridable QuickSpriteDrawer creation hook.
        /// </summary>
        protected virtual QuickSpriteDrawer CreateQuickSpriteDrawerHook()
        {
            return new QuickSpriteDrawer(this);
        }

        /// <summary>
        /// An overridable GuiViewFactory creation hook.
        /// </summary>
        protected virtual GuiViewFactory CreateGuiViewFactoryHook()
        {
            return new GuiViewFactory(this);
        }

        /// <summary>
        /// An overridable GuiSkinGroups creation hook.
        /// </summary>
        protected virtual GuiSkinGroups CreateGuiSkinGroupsHook()
        {
            return new GuiSkinGroups();
        }

        /// <summary>
        /// An overridable GuiSystem creation hook.
        /// </summary>
        protected virtual GuiSystem CreateGuiSystemHook()
        {
            return new GuiSystem(this);
        }

        /// <summary>
        /// An overridable SurfaceFactory creation hook.
        /// </summary>
        protected virtual SurfaceFactory CreateSurfaceFactoryHook()
        {
            return new SurfaceFactory(this);
        }

        /// <summary>
        /// An overridable SurfaceDrawer creation hook.
        /// </summary>
        protected virtual SurfaceDrawer CreateSurfaceDrawerHook()
        {
            return new SurfaceDrawer(this);
        }

        /// <summary>
        /// An overridable SceneSystem creation hook.
        /// </summary>
        protected virtual SceneSystem CreateSceneSystemHook()
        {
            return new SceneSystem(this,
                new Transfer<ISpatialContainer<SceneComponent>>(
                    new SimpleSpatialContainer<SceneComponent>(() =>
                        new FastQueriableCollection<SceneComponent>())));
        }

        /// <summary>
        /// An overridable PhysicsSystem creation hook.
        /// </summary>
        protected virtual PhysicsSystem CreatePhysicsSystemHook()
        {
            PhysicsSystem physicsSystem = new PhysicsSystem();
            physicsSystem.CollisionSystem = new CollisionSystemSAP();
            physicsSystem.SolverType = PhysicsSystem.Solver.Normal;
            return physicsSystem;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DestroyBackBuffer();
                DestroyDepthStencilBuffer();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void DestroyServicesHook()
        {
            DestroyPhysicsServices();
            DestroySceneServices();
            DestroyGuiServices();
            DestroyGeneralServices();
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            PreUpdate(gameTime);
            {
                UpdateGameState(gameTime);
            }
            PostUpdate(gameTime);
        }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime)
        {
            depthStencilBuffer.Activate();
            sceneSystem.PreDraw(gameTime, Camera);

            backBuffer.Activate();
            {
                GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 1.0f, 0);
                sceneSystem.Draw(gameTime, Camera, "Normal");
                guiSystem.Draw(gameTime);
            }
            backBuffer.Resolve();

            frameRater.RegisterFrame(gameTime);
        }

        private void CreateDepthStencilBuffer()
        {
            depthStencilBuffer = new ManagedDepthStencilBuffer(
                this, OxConfiguration.DepthStencilBufferSize, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
        }

        private void CreateBackBuffer()
        {
            backBuffer = new ManagedBackBuffer(this, 0);
        }

        private void CreateGeneralServices()
        {
            AddService<KeyboardContext>(keyboardContext = CreateKeyboardContextHook());
            AddService<FrameRater>(frameRater = CreateFrameRaterHook());
            AddService<VertexFactory>(CreateVertexFactoryHook());
        }

        private void CreateGuiServices()
        {
            AddService<QuickSpriteDrawer>(quickSpriteDrawer = CreateQuickSpriteDrawerHook());
            AddService<GuiViewFactory>(CreateGuiViewFactoryHook());
            AddService<GuiSkinGroups>(guiSkinGroups = CreateGuiSkinGroupsHook());
            AddService<GuiSystem>(guiSystem = CreateGuiSystemHook());
        }

        private void CreateSceneServices()
        {
            AddService<SurfaceFactory>(CreateSurfaceFactoryHook());
            AddService<SurfaceDrawer>(CreateSurfaceDrawerHook());
            AddService<SceneSystem>(sceneSystem = CreateSceneSystemHook());
        }

        private void CreatePhysicsServices()
        {
            AddService<PhysicsSystem>(physicsSystem = CreatePhysicsSystemHook());
        }

        private void DestroyPhysicsServices()
        {
            DestroyPhysicsSystem();
        }

        private void DestroySceneServices()
        {
            DestroySceneSystem();
            DestroySurfaceDrawer();
            DestroySurfaceFactory();
        }

        private void DestroyGuiServices()
        {
            DestroyGuiSystem();
            DestroyGuiSkinGroups();
            DestroyGuiViewFactory();
            DestroyQuickSpriteDrawer();
        }

        private void DestroyGeneralServices()
        {
            DestroyVertexFactory();
            DestroyFrameRater();
            DestroyKeyboardContext();
        }

        private void DestroyPhysicsSystem()
        {
            RemoveService<PhysicsSystem>();
        }

        private void DestroySceneSystem()
        {
            RemoveService<SceneSystem>();
            sceneSystem.Dispose();
        }

        private void DestroySurfaceDrawer()
        {
            RemoveService<SurfaceDrawer>();
        }

        private void DestroySurfaceFactory()
        {
            RemoveService<SurfaceFactory>();
        }

        private void DestroyGuiSystem()
        {
            RemoveService<GuiSystem>();
            guiSystem.Dispose();
        }

        private void DestroyGuiSkinGroups()
        {
            RemoveService<GuiSkinGroups>();
            guiSkinGroups.Dispose();
        }

        private void DestroyGuiViewFactory()
        {
            RemoveService<GuiViewFactory>();
        }

        private void DestroyQuickSpriteDrawer()
        {
            RemoveService<QuickSpriteDrawer>();
            quickSpriteDrawer.Dispose();
        }

        private void DestroyVertexFactory()
        {
            RemoveService<VertexFactory>();
        }

        private void DestroyFrameRater()
        {
            RemoveService<FrameRater>();
        }

        private void DestroyKeyboardContext()
        {
            RemoveService<KeyboardContext>();
        }

        private void DestroyBackBuffer()
        {
            backBuffer.Dispose();
        }

        private void DestroyDepthStencilBuffer()
        {
            depthStencilBuffer.Dispose();
        }

        private void PreUpdate(GameTime gameTime)
        {
            UpdateInputState();
            UpdateKeyboardContext(gameTime);
            UpdatePhysicsSystem(gameTime);
        }

        private void PostUpdate(GameTime gameTime)
        {
            UpdateResolutionManager(gameTime);
            UpdateGuiSystem(gameTime);
        }

        private void UpdateKeyboardContext(GameTime gameTime)
        {
            keyboardContext.Update(gameTime);
        }

        private void UpdatePhysicsSystem(GameTime gameTime)
        {
            if (!Paused) physicsSystem.Integrate((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void UpdateResolutionManager(GameTime gameTime)
        {
            ResolutionManager.Update(gameTime);
        }

        private void UpdateGuiSystem(GameTime gameTime)
        {
            guiSystem.Update(gameTime);
        }

        private ManagedDepthStencilBuffer depthStencilBuffer;
        private ManagedBackBuffer backBuffer;
        private QuickSpriteDrawer quickSpriteDrawer;
        private KeyboardContext keyboardContext;
        private GuiSkinGroups guiSkinGroups;
        private PhysicsSystem physicsSystem;
        private SceneSystem sceneSystem;
        private FrameRater frameRater;
        private GuiSystem guiSystem;
    }
}
