namespace System.ComponentModel
{
    /// <summary>
    /// A null-class definition for TypeConverterAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute : Attribute
    {
        public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();
        public TypeConverterAttribute() { }
        public TypeConverterAttribute(string typeName) { }
        public TypeConverterAttribute(Type type) { }
        public string ConverterTypeName { get { return string.Empty; } }
    }
}
