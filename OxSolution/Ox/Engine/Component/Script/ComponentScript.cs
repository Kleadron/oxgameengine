using System;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A script that controls a component of type T.
    /// </summary>
    /// <typeparam name="T">The type of component the script controls.</typeparam>
    public class ComponentScript<T> : BaseComponentScript
        where T : OxComponent
    {
        /// <summary>
        /// Create a component script.
        /// </summary>
        /// <param name="engine">See property Engine.</param>
        /// <param name="component">The component controlled by the script.</param>
        public ComponentScript(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            this.component = ScriptComponentToT(component.Value);
        }

        /// <summary>
        /// The scripted component.
        /// </summary>
        protected new T Component { get { return component; } }

        private static T ScriptComponentToT(OxComponent component)
        {
            try
            {
                return OxHelper.Cast<T>(component);
            }
            catch (InvalidCastException)
            {
                throw new ComponentScriptException("Script component is not a(n) " + typeof(T).Name + ".");
            }
        }

        private readonly T component;
    }
}
