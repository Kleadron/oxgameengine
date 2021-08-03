using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// Raised when an object's heavyweight properties have changed.
    /// </summary>
    public delegate void PropertyChanged(object sender, string propertyName, object oldValue);

    /// <summary>
    /// The base type for a serializable item within an Ox document.
    /// </summary>
    public abstract class ItemToken
    {
        /// <summary>
        /// Create an ItemToken.
        /// </summary>
        public ItemToken()
        {
            Name = DefaultName;
        }

        /// <summary>
        /// The guid of the parent, if any.
        /// </summary>
        [DefaultValue(null), Browsable(false)]
        public Guid? ParentGuid
        {
            get { return _parentGuid; }
            set
            {
                if (_parentGuid == value) return;
                object oldValue = _parentGuid;
                _parentGuid = value;
                RaisePropertyChanged("ParentGuid", oldValue);
            }
        }
        
        /// <summary>
        /// The item's guid.
        /// </summary>
        [Browsable(false)]
        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (_guid == value) return;
                object oldValue = _guid;
                _guid = value;
                RaisePropertyChanged("Guid", oldValue);
            }
        }
        
        /// <summary>
        /// The name of the item's type.
        /// </summary>
        public string ItemType { get { return OxHelper.StripToken(GetType().Name); } }
        
        /// <summary>
        /// The name of the item.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) value = string.Empty; // VALIDATION
                value = FormatName(value); // VALIDATION
                if (_name == value) return;
                object oldValue = _name;
                _name = value;
                RaisePropertyChanged("Name", oldValue);
            }
        }
        
        /// <summary>
        /// Immutable, prettified version of Name.
        /// </summary>
        public string ItemName
        {
            get
            {
                return
                    Name == DefaultName ?
                    OxConfiguration.DefaultItemName :
                    OxHelper.StripGuid(Name);
            }
        }
        
        /// <summary>
        /// Is the item expanded in the editor's tree view?
        /// </summary>
        [DefaultValue(true), Browsable(false)]
        public bool Expanded
        {
            get { return expanded; }
            set { expanded = value; }
        }

        /// <summary>
        /// Raised when the item's heavyweight properties have changed.
        /// </summary>
        public event PropertyChanged PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event.
        /// </summary>
        public void RaisePropertyChanged(string propertyName, object oldValue)
        {
            if (PropertyChanged != null) PropertyChanged(this, propertyName, oldValue);
        }

        /// <summary>
        /// Duplicate the item by copying all public properties that pass PropertyFilter.
        /// </summary>
        public ItemToken Duplicate()
        {
            Type type = GetType();
            ConstructorInfo constructor = type.GetConstructor(OxHelper.EmptyTypes);
            ItemToken result = OxHelper.Cast<ItemToken>(constructor.Invoke(null));
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties.Where(x => DuplicationFilter(x)))
            {
                object propertyValue = property.GetValue(this, null);
                property.SetValue(result, propertyValue, null);
            }
            return result;
        }

        /// <summary>
        /// Impersonate another item.
        /// </summary>
        /// <param name="source">The source of the impersonation.</param>
        public void Impersonate(ItemToken source)
        {
            Type myType = GetType();
            Type sourceType = source.GetType();
            IEnumerable<PropertyInfo> myProperties = myType.GetProperties().Where(x => ImpersonationFilter(x));
            IEnumerable<PropertyInfo> sourceProperties = sourceType.GetProperties();
            foreach (PropertyInfo myProperty in myProperties)
            {
                PropertyInfo sourceProperty = sourceProperties.FirstOrDefault(x =>
                    x.Name == myProperty.Name &&
                    x.PropertyType == myProperty.PropertyType &&
                    x.CanRead &&
                    x.CanWrite);
                if (sourceProperty == null) continue;
                object propertyValue = sourceProperty.GetValue(source, null);
                myProperty.SetValue(this, propertyValue, null);
            }
        }

        /// <summary>
        /// The default name for new ItemTokens.
        /// </summary>
        protected string DefaultName { get { return OxHelper.AffixGuid(ItemType, Guid); } }

        /// <summary>
        /// Format the object's name, if needed.
        /// </summary>
        protected abstract string FormatName(string name);

        /// <summary>
        /// Should the specified public instance property be duplicated?
        /// </summary>
        protected abstract bool DuplicationFilterHook(PropertyInfo property);

        /// <summary>
        /// Should the specified public instance property be impersonated?
        /// </summary>
        protected abstract bool ImpersonationFilterHook(PropertyInfo property);

        private bool DuplicationFilter(PropertyInfo property)
        {
            return
                property.CanRead &&
                property.CanWrite &&
                property.Name != "Guid" &&
                property.Name != "ParentGuid" &&
                property.Name != "Expanded" &&
                DuplicationFilterHook(property);
        }

        private bool ImpersonationFilter(PropertyInfo property)
        {
            return
                property.CanRead &&
                property.CanWrite &&
                property.Name != "Guid" &&
                ImpersonationFilterHook(property);
        }

        private bool expanded = true;
        private Guid? _parentGuid;
        private Guid _guid = Guid.NewGuid();
        private string _name;
    }
}
