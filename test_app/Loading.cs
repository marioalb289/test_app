using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_app
{
    public partial class Loading : Form
    {
        Form mdi;
        public Loading(Form mdiParent)
        {
            InitializeComponent();
            mdi = mdiParent;
            this.BringToFront();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Loading_Load(object sender, EventArgs e)
        {

        }

        private void Loading_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (mdi != null) mdi.Focus();
                this.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
