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

                var users = new List<string>()
                    {
                         "ysmnkc",
                         "frhtclk",
                         "mhmmt",
                         "user1",
                         "user2",
                         "user3",
                         "cinsapt0",
                         "foreign"
                    };
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
