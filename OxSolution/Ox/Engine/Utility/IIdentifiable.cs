using System;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Raised when a name is changed.
    /// </summary>
    public delegate void NameChanged(IIdentifiable sender, string oldName);
    /// <summary>
    /// Raised when a guid is changed.
    /// </summary>
    public delegate void GuidChanged(IIdentifiable sender, Guid oldGuid);

    /// <summary>
    /// Enables simple identification properties for objects.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        Guid Guid { get; set; }
        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The default name. Guaranteed to be unique.
        /// </summary>
        string DefaultName { get; }
        /// <summary>
        /// Raised when the Name property changes.
        /// </summary>
        event NameChanged NameChanged;
        /// <summary>
        /// Raised when the Guid property changes.
        /// </summary>
        event GuidChanged GuidChanged;
    }
}
