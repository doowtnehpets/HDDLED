using System.Windows.Forms;

namespace HDDLED
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Dispose();
        }

        private void AboutLabel_MouseClick(object sender, MouseEventArgs e)
        {
            this.Dispose();
        }
    }
}
