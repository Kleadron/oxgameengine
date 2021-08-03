using System.ComponentModel;
using System.Reflection;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// A serizalizable token that represents a group within a grouped document.
    /// </summary>
    public class GroupToken : ItemToken
    {
        /// <inheritdoc />
        [Browsable(false)]
        public new string ItemType { get { return ItemName; } }
        
        /// <inheritdoc />
        protected override string FormatName(string name)
        {
            return OxHelper.AffixGuid(OxHelper.StripGuid(name), Guid);
        }

        /// <inheritdoc />
        protected override bool DuplicationFilterHook(PropertyInfo property) { return true; }

        /// <inheritdoc />
        protected override bool ImpersonationFilterHook(PropertyInfo property) { return true; }
    }
}
