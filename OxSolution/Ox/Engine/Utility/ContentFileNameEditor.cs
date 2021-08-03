using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Ox.Engine.Utility
{
    /// <summary>
    /// Edits content file names.
    /// </summary>
    public class ContentFileNameEditor : FileNameEditor
    {
        /// <inheritdoc />
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string fileName = OxHelper.Cast<string>(base.EditValue(context, provider, null));
            string rootPath = OxConfiguration.ContentRootDirectoryAbsolute;
            int rootPathLength = rootPath.Length + 1;
            if (fileName == null || rootPathLength >= fileName.Length) return OxHelper.Cast<string>(value);
            fileName = fileName.Substring(rootPathLength);
            return fileName.Substring(0, fileName.Length - ".xnb".Length);
        }

        /// <inheritdoc />
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory = OxConfiguration.ContentRootDirectoryAbsolute;
        }
    }
}