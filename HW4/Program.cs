using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW4
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

            Task.Run(() => Application.Run(new Form1()));
            Task.Run(() => Application.Run(new Form2()));
            Task.Run(() => Application.Run(new Form3()));

            Application.Run();
        }
    }
}
