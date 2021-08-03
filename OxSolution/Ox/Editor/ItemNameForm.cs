using System;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;

namespace Ox.Editor
{
    public partial class ItemNameForm : Form
    {
        public ItemNameForm(ItemToken item)
        {
            OxHelper.ArgumentNullCheck(item);
            InitializeComponent();
            this.item = item;
            textBoxName.Text = OxHelper.StripGuid(item.Name);
        }

        private void buttonOK_Click(object sender, EventArgs e) { ActionOK(); }

        private void buttonCancel_Click(object sender, EventArgs e) { ActionCancel(); }

        private void ActionOK()
        {
            item.Name = textBoxName.Text;
            Close();
        }

        private void ActionCancel()
        {
            Close();
        }
    
        private readonly ItemToken item;
    }
}
