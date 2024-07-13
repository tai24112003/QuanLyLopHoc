using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testUdpTcp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FirewallHelper.AddFirewallRule();
<<<<<<< HEAD
            Application.Run(new ClientForm());
            //Application.Run(new Waiting());
=======
            //Application.Run(new ClientForm());
            Application.Run(new Waiting());
>>>>>>> origin/dev
        }
    }
}
