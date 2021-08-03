using System.Windows.Forms;

namespace Ox.Editor
{
    public partial class EditorCanvas : UserControl
    {
        public EditorCanvas()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (SinkCmdKey(keyData)) return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static bool SinkCmdKey(Keys keyData)
        {
            return
                keyData == Keys.Up ||
                keyData == Keys.Down ||
                keyData == Keys.Left ||
                keyData == Keys.Right;
        }
    }
}
