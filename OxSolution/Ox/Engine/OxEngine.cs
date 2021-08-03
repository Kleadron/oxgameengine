using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.ServicesNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine
{
    /// <summary>
    /// Raised when a script is added to its engine instance.
    /// </summary>
    public delegate void ScriptAdded(OxEngine sender, BaseComponentScript script);
    /// <summary>
    /// Raised when a script is removed from its engine instance.
    /// </summary>
    public delegate void ScriptRemoved(OxEngine sender, BaseComponentScript script);
    /// <summary>
    /// Raised when a component is added to its engine instance.
    /// </summary>
    public delegate void ComponentAdded(OxEngine sender, OxComponent component);
    /// <summary>
    /// Raised when a component is removed from its engine instance.
    /// </summary>
    public delegate void ComponentRemoved(OxEngine sender, OxComponent component);
    /// <summary>
    /// Raised when a subcomponent is added to its engine instance.
    /// </summary>
    public delegate void SubcomponentAdded(OxEngine sender, BaseSubcomponent subcomponent);
    /// <summary>
    /// Raised when a subcomponent is removed from its engine instance.
    /// </summary>
    public delegate void SubcomponentRemoved(OxEngine sender, BaseSubcomponent subcomponent);
    /// <summary>
    /// Raised when the player in control is changed.
    /// </summary>
    public delegate void PlayerInControlChanged(OxEngine sender, PlayerIndex oldPlayerInControl);

    /// <summary>
    /// The Ox game engine.
    /// </summary>
    public abstract class OxEngine : Disposable2
    {
        /// <summary>
        /// Construct an OxEngine.
        /// </summary>
        /// <param name="game">The XNA game object.</param>
        /// <param name="deviceManager">The graphics device manager</param>
        /// <param name="entryAssembly">The entry assembly (where 'static void main' resides).</param>
        public OxEngine(Game game, GraphicsDeviceManager deviceManager, Assembly entryAssembly)
        {
            OxHelper.ArgumentNullCheck(game, deviceManager, entryAssembly);

            this.game = game;
            this.deviceManager = deviceManager;

            game.IsFixedTimeStep = OxConfiguration.IsFixedTimeStep;
            game.Content.RootDirectory = OxConfiguration.ContentRootDirectory;

            domainManager = new DomainManager(game.Services);
            {
                resolutionManager = new ResolutionManager(deviceManager);
                camera = new FovCamera(this);
                scriptFactory = new ScriptFactory(this, entryAssembly);
                documentLoader.AddLoader(OxConfiguration.GeneralDocumentType, new GeneralDocumentLoader());
            }
            domainManager.CreateDomain(OxConfiguration.GlobalDomainName);

            root = new OxComponent(this, OxConfiguration.GlobalDomainName, false);

            components.NameConflicted += components_NameConflicted;
        }

        /// <summary>
        /// The GraphicsDeviceManager of the game.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get { return deviceManager; } }

        /// <summary>
        /// The object that manages the game's display resolution.
        /// </summary>
        public ResolutionManager ResolutionManager { get { return resolutionManager; } }

        /// <summary>
        /// The GraphicsDevice of the game.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get { return deviceManager.GraphicsDevice; } }

        /// <summary>
        /// The most recent state of the keyboard.
        /// </summary>
        public KeyboardState KeyboardState { get { return keyboardState; } }

        /// <summary>
        /// The state of the controlling player's game pad.
        /// </summary>
        public GamePadState GamePadState { get { return gamePads[(int)playerInControl]; } }

        /// <summary>
        /// The root component of the dependency graph.
        /// </summary>
        public OxComponent Root { get { return root; } }

        /// <summary>
        /// Which player controls the flow of the game.
        /// </summary>
        public PlayerIndex PlayerInControl
        {
            get { return playerInControl; }
            set
            {
                if (playerInControl == value) return;
                PlayerIndex oldPlayerInControl = playerInControl;
                playerInControl = value;
                if (PlayerInControlChanged != null) PlayerInControlChanged(this, oldPlayerInControl);
            }
        }

#if !XBOX360
        /// <summary>
        /// Get the most recent state of the mouse.
        /// </summary>
        public MouseState MouseState { get { return mouseState; } }
#endif

        /// <summary>
        /// The default camera. Additional cameras can be used in an implementation that extends
        /// this interface, but this is the camera that all implementations must use to draw by
        /// default.
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                camera = value;
            }
        }

        /// <summary>
        /// The mouse position relative to the screen.
        /// </summary>
        public Point OSMousePosition
        {
            get
            {
#if !XBOX360
                return new Point(mouseState.X, mouseState.Y);
#else
                return osMousePosition;
#endif
            }
            set
            {
#if !XBOX360
                Mouse.SetPosition(value.X, value.Y);
#else
                osMousePosition = value;
#endif
            }
        }

        /// <summary>
        /// The deprecated XNA game object (see remarks).
        /// </summary>
        /// <remarks>
        /// In Ox, the XNA Game object is deprecated in almost every way. First and foremost, the
        /// XNA Game object no longer represents a "game" in the abstract sense. For example, if
        /// you need a place to put a game-related method called "ChangeLevel()", don't add it to a
        /// subclass of XNA's Game. Instead, attach it to a script object that represents the
        /// game object of your own design. For an example of such an approach, see the GameDemo
        /// project and how it represents its game abstraction with Ox.Scripts.ExampleGame.
        /// 
        /// Additionally, the XNA game object is an obsolete way to store / update / and draw
        /// components. Finally, the XNA game object is an obsolete way to store and retrieve game
        /// services.
        /// 
        /// Unfortunately, some external components do use the XNA Game object in some ways that
        /// are deprecated in Ox. This is generally fine and can be worked around or otherwise
        /// accomodated. But using the XNA Game object in this way is coherent only in the face of
        /// backward-compatibility constraints. Any other usage of the XNA Game object is
        /// incoherent.
        /// </remarks>
        public Game Game { get { return game; } }

        /// <summary>
        /// Should components, subcomponents, and scripts be updated in order?
        /// </summary>
        public bool UpdateInOrder
        {
            get { return updateInOrder; }
            set { updateInOrder = value; }
        }

        /// <summary>
        /// Is the mouse to be centered in the application display every frame?
        /// </summary>
        public bool CenterMouse
        {
            get { return _centerMouse; }
            set
            {
                if (_centerMouse == value) return;
                _centerMouse = value;
                if (CenterMouseChanged != null) CenterMouseChanged(null);
            }
        }

        /// <summary>
        /// Is the game paused?
        /// </summary>
        public bool Paused
        {
            get { return _paused; }
            set
            {
                if (_paused == value) return;
                _paused = value;
                if (PauseStateChanged != null) PauseStateChanged(this);
            }
        }

        /// <summary>
        /// Raised when a script is added.
        /// </summary>
        public event ScriptAdded ScriptAdded;

        /// <summary>
        /// Raised when a script is removed.
        /// </summary>
        public event ScriptRemoved ScriptRemoved;

        /// <summary>
        /// Raised when a component is added.
        /// </summary>
        public event ComponentAdded ComponentAdded;

        /// <summary>
        /// Raised when a component is removed.
        /// </summary>
        public event Action<OxEngine> ComponentNameConflicted;

        /// <summary>
        /// Raised when multiple components are assigned the same name.
        /// </summary>
        public event ComponentRemoved ComponentRemoved;

        /// <summary>
        /// Raised when a subcomponent is added.
        /// </summary>
        public event SubcomponentAdded SubcomponentAdded;

        /// <summary>
        /// Raised when a subcomponent is removed.
        /// </summary>
        public event SubcomponentRemoved SubcomponentRemoved;

        /// <summary>
        /// Raised when the player in control is changed.
        /// </summary>
        public event PlayerInControlChanged PlayerInControlChanged;

        /// <summary>
        /// Raised when multiple peer subcomponents are assigned the same name.
        /// </summary>
        public event Action<OxEngine> SubcomponentNameConflicted;

        /// <summary>
        /// Raised when the CenterMouse property is changed.
        /// </summary>
        public event Action<OxEngine> CenterMouseChanged;

        /// <summary>
        /// Raised when the PauseState property is changed.
        /// </summary>
        public event Action<OxEngine> PauseStateChanged;

        /// <summary>
        /// Update the engine. Must be called once per game cycle.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            UpdateHook(gameTime);
        }

        /// <summary>
        /// Draw what the engine is poised to draw.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(GameTime gameTime)
        {
            DrawHook(gameTime);
        }

        /// <summary>
        /// Get the camera as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the camera.</typeparam>
        public T GetCamera<T>() where T : Camera
        {
            return OxHelper.Cast<T>(camera);
        }

        /// <summary>
        /// Get the most recent state of a game pad.
        /// </summary>
        public void GetGamePadState(PlayerIndex index, out GamePadState state)
        {
            state = gamePads[(int)index];
        }

        /// <summary>
        /// Get the most recent state of a game pad.
        /// </summary>
        public GamePadState GetGamePadState(PlayerIndex index)
        {
            return gamePads[(int)index];
        }

        /// <summary>
        /// Get the most recent state of the keyboard.
        /// </summary>
        public void GetKeyboardState(out KeyboardState state)
        {
            state = keyboardState;
        }

#if !XBOX360
        /// <summary>
        /// Get the most recent state of the mouse.
        /// </summary>
        public void GetMouseState(out MouseState state)
        {
            state = mouseState;
        }
#endif

        /// <summary>
        /// Create a domain with the specified name.
        /// </summary>
        public void CreateDomain(string domainName)
        {
            if (domainName == OxConfiguration.GlobalDomainName) throw new ArgumentException("Cannot create the global domain.");
            domainManager.CreateDomain(domainName);
        }

        /// <summary>
        /// Destroy a domain and all the objects it owns.
        /// </summary>
        public void DestroyDomain(string domainName)
        {
            if (domainName == OxConfiguration.GlobalDomainName) throw new ArgumentException("Cannot destroy the global domain.");
            domainManager.DestroyDomain(domainName);
        }

        /// <summary>
        /// Does a domain exist?
        /// </summary>
        public bool ContainsDomain(string domainName)
        {
            return domainManager.ContainsDomain(domainName);
        }

        /// <summary>
        /// Load content from the specified domain.
        /// </summary>
        public T Load<T>(string fileName, string domainName)
        {
            return domainManager.Load<T>(fileName, domainName);
        }

        /// <summary>
        /// Add a service of type T.
        /// </summary>
        public void AddService<T>(T provider) where T : class
        {
            services.Add(provider);
        }

        /// <summary>
        /// Remove a service of type T.
        /// </summary>
        public bool RemoveService<T>() where T : class
        {
            return services.Remove<T>();
        }

        /// <summary>
        /// Query if a service of type T is present.
        /// </summary>
        public bool ContainsService<T>() where T : class
        {
            return services.Contains<T>();
        }

        /// <summary>
        /// Get a service.
        /// </summary>
        public T GetService<T>() where T : class
        {
            return services.Get<T>();
        }

        /// <summary>
        /// Create a script with the specified script class.
        /// </summary>
        /// <param name="scriptClass">The script's class type.</param>
        /// <param name="component">The component the script is bound to.</param>
        /// <exception cref="ComponentScriptException" />
        public BaseComponentScript CreateScript(string scriptClass, Transfer<OxComponent> component)
        {
            return scriptFactory.CreateScript(scriptClass, component);
        }

        /// <summary>
        /// Add a script to the engine. Its name should be unique among all scripts.
        /// </summary>
        public void AddScript(BaseComponentScript script)
        {
            OxHelper.ArgumentNullCheck(script);
            domainManager[script.DomainName].AddScript(script);
            scripts.Add(script);
            if (ScriptAdded != null) ScriptAdded(this, script);
        }

        /// <summary>
        /// Remove a script from the engine.
        /// </summary>
        public bool RemoveScript(BaseComponentScript script)
        {
            OxHelper.ArgumentNullCheck(script);
            string domainName = script.DomainName;
            if (domainManager.ContainsDomain(domainName))
            {
                Domain domain = domainManager[domainName];
                domain.RemoveScript(script);
            }

            bool result = scripts.Remove(script);
            if (result && ScriptRemoved != null) ScriptRemoved(this, script);
            return result;
        }

        /// <summary>
        /// Does the script of type T with the specified name exist?
        /// </summary>        
        public bool ContainsScript<T>(string scriptName) where T : class
        {
            return scripts.Contains<T>(scriptName);
        }

        /// <summary>
        /// Get a script of type T with the specified name.
        /// </summary>
        public T GetScript<T>(string scriptName) where T : class
        {
            return scripts.Get<T>(scriptName);
        }

        /// <summary>
        /// Collect all scripts of type T.
        /// </summary>
        public IList<T> CollectScripts<T>(IList<T> result) where T : class
        {
            return scripts.Collect<T>(result);
        }

        /// <summary>
        /// Collect all scripts of type T that satisfy a predicate.
        /// </summary>
        public IList<T> CollectScripts<T>(Func<T, bool> predicate, IList<T> result) where T : class
        {
            return scripts.Collect<T>(predicate, result);
        }

        /// <summary>
        /// Add a component to its assigned domain. Its name should be unique among all components.
        /// </summary>
        public void AddComponent(OxComponent component)
        {
            OxHelper.ArgumentNullCheck(component);
            domainManager[component.DomainName].AddComponent(component);
            component.SubcomponentNameConflicted += component_SubcomponentNameConflicted;
            components.Add(component);
            if (ComponentAdded != null) ComponentAdded(this, component);
        }

        /// <summary>
        /// Remove a component.
        /// </summary>
        public bool RemoveComponent(OxComponent component)
        {
            OxHelper.ArgumentNullCheck(component);
            string domainName = component.DomainName;
            if (domainManager.ContainsDomain(domainName))
            {
                Domain domain = domainManager[domainName];
                domain.RemoveComponent(component);
            }

            bool result = components.Remove(component);
            if (result)
            {
                component.SubcomponentNameConflicted -= component_SubcomponentNameConflicted;
                if (ComponentRemoved != null) ComponentRemoved(this, component);
            }
            return result;
        }

        /// <summary>
        /// Does the component of type T with the specified name exist?
        /// </summary>
        public bool ContainsComponent<T>(string name) where T : class
        {
            return components.Contains<T>(name);
        }

        /// <summary>
        /// Get the component of type T with the specified name.
        /// </summary>
        public T GetComponent<T>(string name) where T : class
        {
            return components.Get<T>(name);
        }

        /// <summary>
        /// Collect all components of type T.
        /// </summary>
        public IList<T> CollectComponents<T>(IList<T> result) where T : class
        {
            return components.Collect<T>(result);
        }

        /// <summary>
        /// Collect all components of type T that satisfy a predicate.
        /// </summary>
        public IList<T> CollectComponents<T>(Func<T, bool> predicate, IList<T> result) where T : class
        {
            return components.Collect<T>(predicate, result);
        }

        /// <summary>
        /// Add a subcomponent. Subcomponent name must be unique.
        /// </summary>
        public void AddSubcomponent(BaseSubcomponent subcomponent)
        {
            OxHelper.ArgumentNullCheck(subcomponent);
            subcomponents.Add(subcomponent);
            if (SubcomponentAdded != null) SubcomponentAdded(this, subcomponent);
        }

        /// <summary>
        /// Remove a subcomponent.
        /// </summary>
        public bool RemoveSubcomponent(BaseSubcomponent subcomponent)
        {
            OxHelper.ArgumentNullCheck(subcomponent);
            bool result = subcomponents.Remove(subcomponent);
            if (result && SubcomponentRemoved != null) SubcomponentRemoved(this, subcomponent);
            return result;
        }

        /// <summary>
        /// Collect all subcomponents of type T.
        /// </summary>
        public IList<T> CollectSubcomponents<T>(IList<T> result) where T : class
        {
            return subcomponents.Collect<T>(result);
        }

        /// <summary>
        /// Collect all subcomponents of type T that satisfy a predicate.
        /// </summary>
        public IList<T> CollectSubcomponents<T>(Func<T, bool> predicate, IList<T> result) where T : class
        {
            return subcomponents.Collect<T>(predicate, result);
        }

        /// <summary>
        /// Add a loader to load the specified type of document.
        /// </summary>
        public void AddDocumentLoader(string documentType, DocumentLoader loader)
        {
            documentLoader.AddLoader(documentType, loader);
        }

        /// <summary>
        /// Remove a document loader.
        /// </summary>
        public bool RemoveDocumentLoader(string documentType)
        {
            return documentLoader.RemoveLoader(documentType);
        }

        /// <summary>
        /// Load a document.
        /// </summary>
        /// <param name="fileName">The name of the document file.</param>
        /// <param name="documentType">
        /// The type of document, for example. "General", "Scene", "Gui". Case-sensitive.</param>
        /// <param name="domainName">The domain that will own the document items.</param>
        /// <exception cref="LoadDocumentException" />
        public void LoadDocument(string fileName, string documentType, string domainName)
        {
            documentLoader.LoadDocument(this, fileName, documentType, domainName);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                root.Dispose();

                domainManager.DestroyDomain(OxConfiguration.GlobalDomainName);
                {
                    documentLoader.RemoveLoader(OxConfiguration.GeneralDocumentType);
                    camera.Dispose();
                    DestroyServicesHook();
                }
                domainManager.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handle destruction of engine services.
        /// </summary>
        protected abstract void DestroyServicesHook();

        /// <summary>
        /// Handle updating.
        /// </summary>
        protected abstract void UpdateHook(GameTime gameTime);

        /// <summary>
        /// Handle drawing.
        /// </summary>
        protected abstract void DrawHook(GameTime gameTime);

        /// <summary>
        /// Update the game state (first the scripts, then the components, then the subcomponents).
        /// </summary>
        protected void UpdateGameState(GameTime gameTime)
        {
            UpdateScripts(gameTime);
            UpdateComponents(gameTime);
            UpdateSubcomponents(gameTime);
        }

        /// <summary>
        /// Update the various input states.
        /// </summary>
        protected void UpdateInputState()
        {
            UpdateGamePadStates();
            UpdateKeyboardState();
            UpdateMouseState();
            UpdateMouseCentering();
        }

        private void UpdateScripts(GameTime gameTime)
        {
            scripts.Collect(enabledPredicate, cachedUpdateables);
            {
                if (updateInOrder) cachedUpdateables.Sort(updateOrderComparer);
                for (int i = 0; i < cachedUpdateables.Count; ++i) cachedUpdateables[i].Update(gameTime);
            }
            cachedUpdateables.Clear();
        }

        private void UpdateComponents(GameTime gameTime)
        {
            root.CollectChildren(CollectionAlgorithm.ShallowDescending, componentEnabledPredicate, cachedUpdateableComponents);
            {
                if (updateInOrder) cachedUpdateableComponents.Sort(componentUpdateOrderComparer);
                for (int i = 0; i < cachedUpdateableComponents.Count; ++i) cachedUpdateableComponents[i].Update(gameTime);
            }
            cachedUpdateableComponents.Clear();
        }

        private void UpdateSubcomponents(GameTime gameTime)
        {
            subcomponents.Collect(enabledPredicate, cachedUpdateables);
            {
                if (updateInOrder) cachedUpdateables.Sort(updateOrderComparer);
                for (int i = 0; i < cachedUpdateables.Count; ++i) cachedUpdateables[i].Update(gameTime);
            }
            cachedUpdateables.Clear();
        }

        private void UpdateGamePadStates()
        {
            for (int i = 0; i < OxConfiguration.PlayerMax; ++i)
                gamePads[i] = GamePad.GetState((PlayerIndex)i);
        }

        private void UpdateKeyboardState()
        {
            keyboardState = Keyboard.GetState();
        }

        private void UpdateMouseState()
        {
#if !XBOX360
            mouseState = Mouse.GetState();
#endif
        }

        private void UpdateMouseCentering()
        {
            if (!CenterMouse) return;
            GraphicsDevice device = deviceManager.GraphicsDevice;
            Point viewportCenter = new Point(
                device.Viewport.X + device.Viewport.Width / 2,
                device.Viewport.Y + device.Viewport.Height / 2);
            OSMousePosition = viewportCenter;
        }

        private void components_NameConflicted(QueriableIdentifiables<OxComponent> sender)
        {
            if (ComponentNameConflicted != null) ComponentNameConflicted(this);
        }

        private void component_SubcomponentNameConflicted(OxComponent component)
        {
            if (SubcomponentNameConflicted != null) SubcomponentNameConflicted(this);
        }

        private static readonly ComponentUpdateOrderComparer<UpdateableComponent> componentUpdateOrderComparer = new ComponentUpdateOrderComparer<UpdateableComponent>();
        private static readonly UpdateOrderComparer<IOxUpdateable> updateOrderComparer = new UpdateOrderComparer<IOxUpdateable>();
        private static readonly Func<UpdateableComponent, bool> componentEnabledPredicate = x => x.EnabledWorld;
        private static readonly Func<IOxUpdateable, bool> enabledPredicate = x => x.Enabled;

        private readonly QueriableIdentifiables<OxComponent> components = new QueriableIdentifiables<OxComponent>(true);
        private readonly IQueriableCollection<BaseSubcomponent> subcomponents = new FastQueriableCollection<BaseSubcomponent>();
        private readonly QueriableIdentifiables<BaseComponentScript> scripts = new QueriableIdentifiables<BaseComponentScript>(true);
        private readonly List<UpdateableComponent> cachedUpdateableComponents = new List<UpdateableComponent>();
        private readonly List<IOxUpdateable> cachedUpdateables = new List<IOxUpdateable>();
        private readonly GamePadState[] gamePads = new GamePadState[(int)OxConfiguration.PlayerMax];
        private readonly VariableDocumentLoader documentLoader = new VariableDocumentLoader();
        private readonly GraphicsDeviceManager deviceManager;
        private readonly ResolutionManager resolutionManager;
        private readonly DomainManager domainManager;
        private readonly ScriptFactory scriptFactory;
        private readonly OxComponent root;
        private readonly Services services = new Services();
        private readonly Game game;
        private KeyboardState keyboardState;
        private PlayerIndex playerInControl;
#if !XBOX360
        private MouseState mouseState;
#else
        private Point osMousePosition;
#endif
        private Camera camera;
        private bool updateInOrder;
        private bool _centerMouse;
        private bool _paused;
    }
}
