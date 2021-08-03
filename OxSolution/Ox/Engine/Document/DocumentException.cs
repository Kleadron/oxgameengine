using System;
using System.Runtime.Serialization;

namespace Ox.Engine.DocumentNamespace
{
    /// <summary>
    /// An exception that can occur when working with a document.
    /// </summary>
    [Serializable]
    public class DocumentException : Exception
    {
        public DocumentException() { }
        public DocumentException(string message) : base(message) { }
        public DocumentException(string message, Exception innerException) : base(message, innerException) { }
#if !XBOX360
        protected DocumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

    /// <summary>
    /// An exception that can occur when loading a document.
    /// </summary>
    [Serializable]
    public class LoadDocumentException : DocumentException
    {
        public LoadDocumentException() { }
        public LoadDocumentException(string message) : base(message) { }
        public LoadDocumentException(string message, Exception innerException) : base(message, innerException) { }
#if !XBOX360
        protected LoadDocumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }

    /// <summary>
    /// An exception that can occur when saving a document.
    /// </summary>
    [Serializable]
    public class SaveDocumentException : DocumentException
    {
        public SaveDocumentException() { }
        public SaveDocumentException(string message) : base(message) { }
        public SaveDocumentException(string message, Exception innerException) : base(message, innerException) { }
#if !XBOX360
        protected SaveDocumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
