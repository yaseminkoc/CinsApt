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
            if (fLogin.ShowDialog() == DialogResult.OK)
                {
                string[] users = System.IO.File.ReadAllLines(@".\Users.txt");
                bool isHere = false;

                if (fLogin.Textb() != "")
                {
                    foreach (string user in users)
                    {
                        if (user == fLogin.Textb())
                        {
                            isHere = true;
                        }
                    }
                    if (isHere == false)
                    {
                        MessageBox.Show("You are not in our apartment :) So you cannot login");
                    }
                    else
                    {
                        frmClientMain form = new frmClientMain();
                        form.setName(fLogin.Textb());
                        Application.Run(form);
                    }

                }
                else
                {
                    fLogin.slblU("Please enter");
                }
               
                }
            
        }
    }
}
