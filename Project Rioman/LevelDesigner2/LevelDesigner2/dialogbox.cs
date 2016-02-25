using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class dialogbox : Form
    {
        public dialogbox()
        {
            InitializeComponent();
        }

        private void TxtWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() != char.ConvertFromUtf32(0008))
            {
                int isNumber = 0;
                e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
            }
        }

        public int GetWidth
        {
            get
            {
                return Convert.ToInt32(TxtWidth.Text);
            }
        }

        public int GetHeight
        {
            get
            {
                return Convert.ToInt32(TxtHeight.Text);
            }
        }
    }
}
