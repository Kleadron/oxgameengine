using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Storage;
using Ox.Engine.Component;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// A script factory.
    /// </summary>
    public class ScriptFactory
    {
        /// <summary>
        /// Create a ScriptFactory.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="entryAssembly">The entry assembly (where 'static void main' resides).</param>
        public ScriptFactory(OxEngine engine, Assembly entryAssembly)
        {
            OxHelper.ArgumentNullCheck(engine, entryAssembly);
            this.engine = engine;
            this.entryAssembly = entryAssembly;
            scriptAssembly = Assembly.LoadFrom(OxConfiguration.ScriptsFileName);
        }

        /// <summary>
        /// Create a script with the specified script class.
        /// </summary>
        /// <param name="scriptClass">The script's class type.</param>
        /// <param name="component">The component the script is bound to.</param>
        /// <exception cref="ComponentScriptException" />
        public BaseComponentScript CreateScript(string scriptClass, Transfer<OxComponent> component)
        {
            OxHelper.ArgumentNullCheck(scriptClass, component.Value);
            Type type = GetType(scriptClass);
            ConstructorInfo constructor = type.GetConstructor(constructorParameterTypes);
            object[] constructorParameters = new object[] { engine, component };
            try
            {
                return OxHelper.Cast<BaseComponentScript>(constructor.Invoke(constructorParameters));
            }
            catch (Exception e)
            {
                if (e.InnerException != null) FailMessage(scriptClass, e.InnerException);
                throw;
            }
        }

        private Type GetType(string scriptClass)
        {
            Type result = scriptAssembly.GetType(scriptClass);
            if (result != null) return result;
            result = entryAssembly.GetType(scriptClass);
            if (result == null) throw new ComponentScriptException(ScriptExceptionMessage(scriptClass));
            return result;
        }

        private static string ScriptExceptionMessage(string scriptClass)
        {
            return "Cannot find script type \"" + scriptClass + "\".";
        }

        private static void FailMessage(string scriptClass, Exception e)
        {
            System.Diagnostics.Trace.Fail(FailSummary(scriptClass), FailDetail(e));
        }

        private static string FailSummary(string scriptClass)
        {
            return "Exception thrown in constructor of " + scriptClass + ".";
        }

        private static string FailDetail(Exception e)
        {
            return
                "Exception message: " + e.Message + "\n" +
                "Exception type: " + e.GetType() + "\n" +
                "Exception stack trace: " + "\n" + e.StackTrace;
        }

        private static readonly Type[] constructorParameterTypes = new Type[] {
            typeof(OxEngine), typeof(Transfer<OxComponent>) };

        private readonly OxEngine engine;
        // HACK: getting the entry assembly via parameter is an awful approach. It would be MUCH
        // better if Assembly.GetEntryAssembly() was used, but it's not yet available on the Xbox.
        private readonly Assembly entryAssembly;
        private readonly Assembly scriptAssembly;
    }
}
