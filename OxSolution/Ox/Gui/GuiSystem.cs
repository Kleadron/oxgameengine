using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.ServicesNamespace;
using Ox.Engine.Utility;
using Ox.Gui.Component;
using Ox.Gui.Event;
using Ox.Gui.Input;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui
{
    /// <summary>
    /// Raised when the gui screen is changed.
    /// </summary>
    /// <param name="sender">The object firing the event.</param>
    /// <param name="oldScreen">The previous screen. May be null.</param>
    public delegate void ScreenChanged(GuiSystem sender, BaseGuiComponent oldScreen);

    /// <summary>
    /// The gui system.
    /// </summary>
    public class GuiSystem : Disposable
    {
        /// <summary>
        /// Create a GuiSystem.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GuiSystem(OxEngine engine)
        {
            OxHelper.ArgumentNullCheck(engine);

            this.engine = engine;

            camera = CreateGuiCamera();
            eventForwarder = new EventForwarder(engine, repeatRate);
            engine.AddDocumentLoader(GuiConfiguration.GuiDocumentType, new GuiDocumentLoader());

            mouse = new MouseComponent(engine, domainName, false);
            mouse.Z = 0.5f; // place at top

            frameRateLabel = new Label(engine, domainName, false);
            frameRateLabel.Z = mouse.Z - layerGap; // place beneath mouse
            frameRateLabel.Position = GuiConfiguration.FrameRateLabelPosition;
            frameRateLabel.Scale = GuiConfiguration.FrameRateLabelScale;
            frameRateLabel.Visible = false;
            frameRateLabel.SetTrait("TextScale", GuiConfiguration.FrameRateLabelTextScale);

            messageBox = new MessageBox(engine, domainName, false);
            messageBox.Z = frameRateLabel.ZWorld - layerGap; // place beneath frameRateLabel
            messageBox.Visible = false;

            virtualKeyboard = new VirtualKeyboard(engine, domainName, false);
            virtualKeyboard.Z = messageBox.ZWorld - layerGap; // place beneath messageBox
            virtualKeyboard.Visible = false;
        }

        /// <summary>
        /// The component that represents the game's current gui screen.
        /// May be null.
        /// </summary>
        public BaseGuiComponent Screen
        {
            get { return _screen; }
            set
            {
                if (_screen == value) return;
                BaseGuiComponent oldScreen = _screen;
                HideVirtualKeyboard();
                HideMessageBox();
                if (_screen != null) DeactivateScreen();
                _screen = value;
                if (_screen != null) ActivateScreen();
                if (ScreenChanged != null) ScreenChanged(this, oldScreen);
            }
        }

        /// <summary>
        /// The camera used to draw the gui.
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
        /// The mouse position relative to the game window.
        /// </summary>
        public Vector2 AppMousePosition
        {
            get
            {
                Point resolution = engine.ResolutionManager.Resolution;
                return GuiHelper.OSMouseToAppMouse(engine.OSMousePosition, resolution);
            }
            set
            {
                Point resolution = engine.ResolutionManager.Resolution;
                engine.OSMousePosition = GuiHelper.AppMouseToOSMouse(value, resolution);
            }
        }

        /// <summary>
        /// The delay before starting key-repeat.
        /// </summary>
        public float KeyRepeatFirstDelay
        {
            get { return repeatRate.FirstDelay; }
            set { repeatRate.FirstDelay = value; }
        }

        /// <summary>
        /// The delay between each key-repeat.
        /// </summary>
        public float KeyRepeatDelay
        {
            get { return repeatRate.Delay; }
            set { repeatRate.Delay = value; }
        }

        /// <summary>
        /// Should the mouse snap to a gui component when it gains focus?
        /// </summary>
        public bool MouseSnap
        {
            get { return mouseSnapEnabled; }
            set { mouseSnapEnabled = value; }
        }

        /// <summary>
        /// Are the gui events enabled?
        /// </summary>
        public bool EventsEnabled
        {
            get { return eventForwarder.Enabled; }
            set { eventForwarder.Enabled = value; }
        }

        /// <summary>
        /// Raised when the screen has changed.
        /// </summary>
        public event ScreenChanged ScreenChanged;

        /// <summary>
        /// Update the gui. Must be called once per frame.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            UpdatePicked();
            UpdateEventForwarder(gameTime);
            UpdateMouse(gameTime);
            UpdateScreen(gameTime);
            UpdateFrameRateLabel(gameTime);
        }

        /// <summary>
        /// Draw the gui. Must be called once per frame.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);

            CollectSprites(cachedSprites);
            {
                DrawSprites(gameTime);
            }
            cachedSprites.Clear();
        }

        /// <summary>
        /// Hide the mouse.
        /// </summary>
        public void HideMouse()
        {
            mouse.Visible = false;
        }

        /// <summary>
        /// Show the mouse.
        /// </summary>
        public void ShowMouse()
        {
            mouse.Visible = true;
        }

        /// <summary>
        /// Hide the frame rate label.
        /// </summary>
        public void HideFrameRateLabel()
        {
            frameRateLabel.Visible = false;
        }

        /// <summary>
        /// Show the frame rate label.
        /// </summary>
        public void ShowFrameRateLabel()
        {
            frameRateLabel.Visible = true;
        }

        /// <summary>
        /// Hide the virtual keyboard.
        /// </summary>
        public void HideVirtualKeyboard()
        {
            if (Screen == null || virtualKeyboard.Parent != Screen) return;
            Screen.RemoveChild(virtualKeyboard);
            virtualKeyboard.Visible = false;
            virtualKeyboard.Terminate();
        }

        /// <summary>
        /// Show the virtual keyboard for controlling a text box.
        /// </summary>
        public void ShowVirtualKeyboard(TextBox textBox)
        {
            OxHelper.ArgumentNullCheck(textBox);
            if (Screen == null || virtualKeyboard.Parent == Screen) return;
            Screen.AddChild(virtualKeyboard);
            virtualKeyboard.Visible = true;
            virtualKeyboard.Position = CalculateVirtualKeyboardPlacement(textBox);
            virtualKeyboard.Execute(textBox);
        }

        /// <summary>
        /// Hide the message box.
        /// </summary>
        public void HideMessageBox()
        {
            if (Screen == null || messageBox.Parent != Screen) return;
            Screen.RemoveChild(messageBox);
            messageBox.Visible = false;
            if (messageBoxAnswerAction != null)
            {
                messageBox.Answered -= messageBoxAnswerAction;
                messageBoxAnswerAction = null;
            }
            if (messageBoxReturnComponent != null)
            {
                messageBoxReturnComponent.FocusByOtherInput();
                messageBoxReturnComponent = null;
            }
        }

        /// <summary>
        /// Show the message box.
        /// </summary>
        /// <param name="type">The type of message box to show.</param>
        /// <param name="headerText">The header text of the message box.</param>
        /// <param name="messageText">The message text of the message box.</param>
        /// <param name="answerAction">
        /// A delegate called when the message box has been answered.
        /// May be null.</param>
        /// <param name="returnComponent">
        /// The gui component to gain focus when the message box is terminated.
        /// May be null.</param>
        public void ShowMessageBox(MessageBoxType type, string headerText, string messageText,
            MessageBoxAnswered answerAction, BaseGuiComponent returnComponent)
        {
            OxHelper.ArgumentNullCheck(headerText, messageText);
            if (Screen == null || messageBox.Parent == Screen) return;
            Screen.AddChild(messageBox);
            messageBox.Position = (OxConfiguration.VirtualResolutionFloat - messageBox.Scale) * 0.5f;
            messageBox.Visible = true;
            messageBox.Execute(type, headerText, messageText);
            messageBoxAnswerAction = answerAction;
            if (answerAction != null) messageBox.Answered += answerAction;
            this.messageBoxReturnComponent = returnComponent;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                virtualKeyboard.Dispose();
                messageBox.Dispose();
                frameRateLabel.Dispose();
                mouse.Dispose();
                engine.DestroyDomain(domainName);
                engine.RemoveDocumentLoader(GuiConfiguration.GuiDocumentType);
                camera.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateMouse(GameTime gameTime)
        {
            mouse.Update(gameTime);
        }

        private void UpdateFrameRateLabel(GameTime gameTime)
        {
            FrameRater frameRater = engine.GetService<FrameRater>();
            if (frameRate != frameRater.FrameRate || frameRateString.Length == 0)
            {
                frameRate = frameRater.FrameRate;
                frameRateString.Length = 0;
                frameRateString.Append("FR: ");
                frameRateString.Append((int)frameRate);
                frameRateLabel.Text = frameRateString.ToString();
            }
            frameRateLabel.Update(gameTime);
        }

        private void UpdateScreen(GameTime gameTime)
        {
            if (Screen != null) Screen.Update(gameTime);
        }

        private void UpdatePicked()
        {
            if (Screen == null) return;
            BaseGuiComponent pickedComponent = Screen.PickedComponent;
            if (pickedComponent == previousPickedComponent) return;
            if (previousPickedComponent != null) previousPickedComponent.RaisePickedChangedEvent();
            previousPickedComponent = pickedComponent;
            if (pickedComponent != null) pickedComponent.RaisePickedChangedEvent();
        }

        private void UpdateEventForwarder(GameTime gameTime)
        {
            if (eventForwarder.Enabled) eventForwarder.Update(gameTime);
        }

        private Vector2 CalculateVirtualKeyboardPlacement(TextBox textBox)
        {
            const float verticalMargin = 32;
            Vector2 result = new Vector2((OxConfiguration.VirtualResolution.X - virtualKeyboard.Scale.X) * 0.5f, verticalMargin);
            if (textBox.PositionWorld.Y < OxConfiguration.VirtualResolution.Y * 0.5f)
                result.Y = OxConfiguration.VirtualResolution.Y - virtualKeyboard.Scale.Y - verticalMargin;
            return result;
        }

        private OrthoCamera CreateGuiCamera()
        {
            OrthoCamera result = new OrthoCamera(engine);
            result.Width = OxConfiguration.VirtualScreen.Width;
            result.Height = OxConfiguration.VirtualScreen.Height;
            Vector3 position = new Vector3(OxConfiguration.VirtualScreen.Width * 0.5f, 1, OxConfiguration.VirtualScreen.Height * 0.5f);
            Matrix orientation = Matrix.CreateRotationX(-MathHelper.PiOver2);
            result.SetTransformByLookForward(position, orientation.Up, orientation.Forward);
            return result;
        }

        private void CollectSprites(IList<QuickSprite> result)
        {
            if (Screen != null) CollectScreenSprites(result);
            CollectInterleavedSprites(result);
            CollectFrameRaterSprites(result);
        }

        private void DrawSprites(GameTime gameTime)
        {
            QuickSpriteDrawer drawer = engine.GetService<QuickSpriteDrawer>();
            drawer.DrawSprites(gameTime, camera, cachedSprites);
        }

        private void CollectScreenSprites(IList<QuickSprite> result)
        {
            cachedComponents.Add(Screen);
            Screen.CollectChildren<BaseGuiComponent>(CollectionAlgorithm.Descending, cachedComponents);
            {
                for (int i = 0; i < cachedComponents.Count; ++i)
                {
                    BaseGuiComponent component = cachedComponents[i];
                    if (component.VisibleWorld) component.CollectQuickSprites(result);
                }
            }
            cachedComponents.Clear();
        }

        private void CollectFrameRaterSprites(IList<QuickSprite> result)
        {
            if (frameRateLabel.VisibleWorld) frameRateLabel.CollectQuickSprites(result);
        }

        private void CollectInterleavedSprites(IList<QuickSprite> result)
        {
            engine.CollectComponents(cachedInterleaves);
            {
                for (int i = 0; i < cachedInterleaves.Count; ++i)
                {
                    InterleavedComponent component = cachedInterleaves[i];
                    if (component.Visible) cachedInterleaves[i].CollectQuickSprites(result);
                }
            }
            cachedInterleaves.Clear();
        }

        private void DeactivateScreen()
        {
            if (_screen != null) _screen.Defocus();
        }

        private void ActivateScreen()
        {
            if (Screen == null) return;

            cachedComponents.Add(Screen);
            Screen.CollectChildren(CollectionAlgorithm.Descending, cachedComponents);
            {
                for (int i = 0; i < cachedComponents.Count; ++i)
                {
                    BaseGuiComponent component = cachedComponents[i];
                    if (component.CanFocusByOtherInput)
                    {
                        component.FocusByOtherInput();
                        break;
                    }
                }
            }
            cachedComponents.Clear();
        }
        
        private const string domainName = "GuiSystem";
        private const float layerGap = GuiConfiguration.ZGap * 100;

        private readonly IList<InterleavedComponent> cachedInterleaves = new List<InterleavedComponent>();
        private readonly IList<BaseGuiComponent> cachedComponents = new List<BaseGuiComponent>();
        private readonly IList<QuickSprite> cachedSprites = new List<QuickSprite>();
        private readonly VirtualKeyboard virtualKeyboard;
        private readonly EventForwarder eventForwarder;
        private readonly MouseComponent mouse;
        private readonly StringBuilder frameRateString = new StringBuilder();
        private readonly RepeatRate repeatRate = new RepeatRate();
        private readonly MessageBox messageBox;
        private readonly OxEngine engine;
        private readonly Label frameRateLabel;
        /// <summary>May be null.</summary>
        private BaseGuiComponent previousPickedComponent;
        /// <summary>May be null.</summary>
        private BaseGuiComponent messageBoxReturnComponent;
        /// <summary>May be null.</summary>
        private MessageBoxAnswered messageBoxAnswerAction;
        private Camera camera;
        private float frameRate;
        private bool mouseSnapEnabled;
        /// <summary>May be null.</summary>
        private BaseGuiComponent _screen;
    }
}
