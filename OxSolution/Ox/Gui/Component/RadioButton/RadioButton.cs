using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Gui.Event;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A graphical radio button for a user interface.
    /// </summary>
    public class RadioButton : GuiComponent<RadioButtonView>
    {
        /// <summary>
        /// Create a RadioButtonView.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public RadioButton(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain)
        {
            View.Checked = Checked;
            Scale = initialScale;
            GuiAction += this_GuiAction;
            MouseButtonAction += this_MouseButtonAction;
            AbstractAction += this_AbstractAction;
        }

        /// <summary>
        /// Which radio button in the radio button group is checked, if any?
        /// May return null.
        /// </summary>
        public RadioButton CheckedRadioButton { get { return Checked ? this : CheckedPeer; } }

        /// <summary>
        /// The name of the group to which the radio button belongs.
        /// </summary>
        public string RadioButtonGroup
        {
            get { return radioButtonGroup; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                radioButtonGroup = value;
            }
        }

        /// <summary>
        /// Is there a check in the check box?
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                View.Checked = _checked = value;
                if (CheckedChanged != null) CheckedChanged(this);
            }
        }

        /// <summary>
        /// Raised when the check box is checked or unchecked.
        /// </summary>
        public event Action<RadioButton> CheckedChanged;

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new RadioButtonToken();
        }

        /// <inheritdoc />
        protected override bool SinkAbstractEventHook(InputType inputType, AbstractEventType eventType)
        {
            return eventType == AbstractEventType.Affirm;
        }

        /// <summary>May return null.</summary>
        private RadioButton CheckedPeer
        {
            get
            {
                RadioButton result = null;

                CollectPeers(peers);
                {
                    for (int i = 0; i < peers.Count; ++i)
                    {
                        RadioButton peer = peers[i];
                        if (peer.Checked)
                        {
                            result = peer;
                            break;
                        }
                    }
                }
                peers.Clear();

                return result;
            }
        }

        private void this_GuiAction(BaseGuiComponent sender, GuiEventType type)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (type == GuiEventType.Defocused) HandleDefocused();
        }

        private void this_MouseButtonAction(BaseGuiComponent sender, InputType type, MouseButton button, Vector2 mousePosition)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (button == MouseButton.Left) HandleLeftMouse(type);
        }

        private void this_AbstractAction(BaseGuiComponent sender, InputType inputType, AbstractEventType eventType)
        {
            OxHelper.ArgumentNullCheck(sender);
            if (eventType == AbstractEventType.Affirm) HandleAffirmButton(inputType);
        }

        private void HandleDefocused()
        {
            pressed = false;
        }

        private void HandleLeftMouse(InputType type)
        {
            if (type == InputType.ClickDown)
            {
                RaiseAbstractEvent(InputType.ClickDown, AbstractEventType.Affirm);
            }
            else if (type == InputType.ClickUp)
            {
                if (Picked && pressed)
                {
                    RaiseAbstractEvent(InputType.ClickUp, AbstractEventType.Affirm);
                }
                else
                {
                    pressed = false;
                }
            }
        }

        private void HandleAffirmButton(InputType inputType)
        {
            if (inputType == InputType.ClickDown)
            {
                pressed = true;
            }
            else if (inputType == InputType.ClickUp)
            {
                if (pressed)
                {
                    if (!Checked)
                    {
                        RadioButton checkedPeer = CheckedPeer; // OPTIMIZATION: cache property
                        if (checkedPeer != null && checkedPeer != this) checkedPeer.Checked = false;
                    }
					
					Checked = !Checked;
                    pressed = false;
                }
            }
        }

        private IList<RadioButton> CollectPeers(IList<RadioButton> result)
        {
            BaseGuiComponent parent = Parent as BaseGuiComponent;
            if (parent != null) parent.CollectChildren(CollectionAlgorithm.Shallow, IsPeer, result);
            return result;
        }

        private bool IsPeer(RadioButton radioButton)
        {
            return radioButton != this && radioButton.RadioButtonGroup == radioButtonGroup;
        }

        private static readonly Vector2 initialScale = new Vector2(32);

        private readonly IList<RadioButton> peers = new List<RadioButton>();
        private string radioButtonGroup = string.Empty;
        private bool pressed;
        private bool _checked;
    }
}
