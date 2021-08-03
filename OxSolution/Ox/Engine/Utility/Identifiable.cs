using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// A vanilla implementation of IIdentifiable.
    /// </summary>
    public class Identifiable : IIdentifiable
    {
        /// <summary>
        /// Create an Identifiable object.
        /// </summary>
        /// <param name="typeName">The object's most specific type name.</param>
        public Identifiable(string typeName)
        {
            this.typeName = typeName;
            Name = DefaultName;
        }

        /// <inheritdoc />
        public Guid Guid
        {
            get { return guid; }
            set
            {
                if (guid == value) return;
                Guid oldGuid = guid;
                guid = value;
                if (GuidChanged != null) GuidChanged(this, oldGuid);
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_name == value) return;
                string oldName = _name;
                _name = value;
                if (NameChanged != null) NameChanged(this, oldName);
            }
        }

        /// <inheritdoc />
        public string DefaultName { get { return OxHelper.AffixGuid(typeName, Guid); } }

        /// <inheritdoc />
        public event GuidChanged GuidChanged;

        /// <inheritdoc />
        public event NameChanged NameChanged;

        private Guid guid = Guid.NewGuid();
        private readonly string typeName;
        private string _name;
    }
}
