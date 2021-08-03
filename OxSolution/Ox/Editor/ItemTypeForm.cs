using System;
using System.Linq;
using System.Windows.Forms;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using System.Collections.Generic;

namespace Ox.Editor
{
    public partial class ItemTypeForm : Form
    {
        public ItemTypeForm(Type[] itemTypes)
        {
            OxHelper.ArgumentNullCheck(itemTypes);
            this.itemTypes = itemTypes;
            InitializeComponent();
            PopulateItemTypes();
        }

        /// <summary>May be empty string.</summary>
        public string NewType { get { return newType; } }

        private void buttonOK_Click(object sender, EventArgs e) { ActionOK(); }
        
        private void buttonCancel_Click(object sender, EventArgs e) { ActionCancel(); }

        private void ActionOK()
        {
            newType = listBoxType.Text;
            Close();
        }

        private void ActionCancel()
        {
            Close();
        }

        private void PopulateItemTypes()
        {
            listBoxType.Items.Clear();
            itemTypes
                .Select(x => OxHelper.StripToken(x.Name))
                .ForEach(x => listBoxType.Items.Add(x));
            if (itemTypes.Length != 0) listBoxType.SelectedIndex = 0;
        }

        private readonly Type[] itemTypes;
        private string newType = string.Empty;
    }
}
