using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Upon deserialization, can construct an instance of a type.
    /// </summary>
    public struct ConstructionToken
    {
        /// <summary>
        /// Create a ConstructionToken.
        /// </summary>
        /// <param name="fileName">See property FileName.</param>
        /// <param name="typeName">See property TypeName.</param>
        public ConstructionToken(string fileName, string typeName)
        {
            this.FileName = fileName;
            this.TypeName = typeName;
        }

        /// <summary>
        /// Get the constructed type.
        /// </summary>
        public Type GetConstructedType()
        {
            Assembly assembly = Assembly.LoadFrom(FileName);
            return assembly.GetType(TypeName, true);
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="parameterValues">The constructor's parameter values. May be null.</param>
        public T Construct<T>(object[] parameterValues) where T : class
        {
            Type[] parameterTypes = GetParameterTypes(parameterValues);
            ConstructorInfo constructor = GetConstructedType().GetConstructor(parameterTypes);
            object instance = constructor.Invoke(parameterValues);
            return OxHelper.Cast<T>(instance);
        }
        
        /// <summary>
        /// Construct an instance.
        /// </summary>
        public T Construct<T>() where T : class
        {
            return Construct<T>(null);
        }

        /// <summary>
        /// The name of the assembly file that contains the constructed type.
        /// May be null (unfortunately).
        /// </summary>
        public string FileName;
        /// <summary>
        /// The name of the constructed type.
        /// May be null (unfortunately).
        /// </summary>
        public string TypeName;

        private static Type[] GetParameterTypes(object[] parameterValues)
        {
            return
                parameterValues != null ?
                parameterValues.Select(x => x.GetType()).ToArray() :
                OxHelper.EmptyTypes;
        }
    }

    /// <summary>
    /// Implements a list of ConstructionToken objects.
    /// </summary>
    public class ConstructionTokens : List<ConstructionToken> { }

    /// <summary>
    /// Implements a read-only dictionary of ConstructionToken objects.
    /// </summary>
    public class ConstructionDictionary
    {
        /// <summary>
        /// Get the ConstructionToken object with the matching class name.
        /// </summary>
        public ConstructionToken this[string key]
        {
            get { return dictionary[key]; }
        }

        /// <summary>
        /// Create a ConstructionDictionary.
        /// </summary>
        public ConstructionDictionary(ConstructionTokens constructionInfoList)
        {
            PopulateDictionary(constructionInfoList);
            PopulateConstructedTypes(constructionInfoList);
        }

        /// <summary>
        /// Get all the types that this dictionary can construct.
        /// </summary>
        public Type[] GetConstructedTypes()
        {
            return constructedTypes.ToArray();
        }

        private void PopulateDictionary(ConstructionTokens constructionInfoList)
        {
            for (int i = 0; i < constructionInfoList.Count; ++i)
            {
                ConstructionToken constructionInfo = constructionInfoList[i];
                dictionary.Add(constructionInfo.GetConstructedType().Name, constructionInfo);
            }
        }

        private void PopulateConstructedTypes(ConstructionTokens constructionInfoList)
        {
            for (int i = 0; i < constructionInfoList.Count; ++i)
                constructedTypes.Add(constructionInfoList[i].GetConstructedType());
        }

        private readonly Dictionary<string, ConstructionToken> dictionary = new Dictionary<string,ConstructionToken>();
        private readonly List<Type> constructedTypes = new List<Type>();
    }
}
