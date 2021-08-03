using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Pulled from Microsoft.Xna.Framework.Design via reflection and exposed to all the world.
    /// </summary>
    public abstract class MemberPropertyDescriptor : PropertyDescriptor
    {
        // Fields
        private MemberInfo _member;

        // Methods
        public MemberPropertyDescriptor(MemberInfo member)
            : base(member.Name, (Attribute[])member.GetCustomAttributes(typeof(Attribute), true))
        {
            this._member = member;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            MemberPropertyDescriptor descriptor = obj as MemberPropertyDescriptor;
            return ((descriptor != null) && descriptor._member.Equals(this._member));
        }

        public override int GetHashCode()
        {
            return this._member.GetHashCode();
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        // Properties
        public override Type ComponentType
        {
            get
            {
                return this._member.DeclaringType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// FieldPropertyDescriptor - Pulled from Microsoft.Xna.Framework.Design via reflection and exposed to all the world.
    /// </summary>
    public class FieldPropertyDescriptor : MemberPropertyDescriptor
    {
        // Fields
        private FieldInfo _field;

        // Methods
        public FieldPropertyDescriptor(FieldInfo field)
            : base(field)
        {
            this._field = field;
        }

        public override object GetValue(object component)
        {
            return this._field.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this._field.SetValue(component, value);
            this.OnValueChanged(component, EventArgs.Empty);
        }

        // Properties
        public override Type PropertyType
        {
            get
            {
                return this._field.FieldType;
            }
        }
    }

    /// <summary>
    /// A Box type converter. Modified from reflected BoundingBoxConverter class.
    /// </summary>
    public class BoxConverter : MathTypeConverter
    {
        // Methods
        public BoxConverter()
        {
            Type type = typeof(Box);

            propertyDescriptions = new PropertyDescriptorCollection
            (
                new PropertyDescriptor[]
                {
                    new FieldPropertyDescriptor(type.GetField("Center")),
                    new FieldPropertyDescriptor(type.GetField("Extent"))
                }
            ).Sort(new string[] { "Center", "Extent" });

            supportStringConvert = false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Box))
            {
                Box box = (Box)value;
                ConstructorInfo constructor = typeof(Box).GetConstructor(new Type[] { typeof(Vector3), typeof(Vector3) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { box.Center, box.Extent });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Box((Vector3)propertyValues["Center"], (Vector3)propertyValues["Extent"]);
        }
    }
}
