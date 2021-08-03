using System;
using System.Reflection;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// An doable / undoable set_property operation.
    /// </summary>
    public class SetOperation : IOperation
    {
        /// <summary>
        /// Create a SetOperation.
        /// </summary>
        public SetOperation(string name, Guid group, object target, string propertyName, object oldValue)
        {
            OxHelper.ArgumentNullCheck(name, target, propertyName);

            this.name = name;
            this.target = target;
            this.oldValue = oldValue;
            this.group = group;

            propertyInfo = target.GetType().GetProperty(propertyName);
            newValue = propertyInfo.GetValue(target, null);
        }

        /// <inheritdoc />
        public Guid Handle { get { return handle; } }

        /// <inheritdoc />
        public Guid Group { get { return group; } }

        /// <inheritdoc />
        public string Name { get { return name; } }

        /// <inheritdoc />
        public bool Done { get { return done; } }

        /// <inheritdoc />
        public void Redo()
        {
            if (done) return;
            done = true;
            propertyInfo.SetValue(target, newValue, null);
        }

        /// <inheritdoc />
        public void Undo()
        {
            if (!done) return;
            done = false;
            propertyInfo.SetValue(target, oldValue, null);
        }

        private readonly PropertyInfo propertyInfo;
        private readonly Guid handle = Guid.NewGuid();
        private readonly Guid group;
        private readonly object oldValue;
        private readonly object newValue;
        private readonly object target;
        private readonly string name;
        private bool done = true;
    }
}
