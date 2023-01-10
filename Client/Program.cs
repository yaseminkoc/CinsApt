using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmCardReader fLogin = new frmCardReader();
            Boolean flag = true;

                if (fLogin.ShowDialog() == DialogResult.OK)
                {
                    if (fLogin.Textb() != "")
                    {
                        flag = false;
                        frmClientMain form = new frmClientMain();
                        form.setName(fLogin.Textb());
                        Application.Run(form);
                    }
                    else
                    {
                        fLogin.slblU("Please enter");
                        //MessageBox.Show("Please enter");
                    }
                }
                else
                {
                    Application.Exit();

                }
            
        }
    }
}
