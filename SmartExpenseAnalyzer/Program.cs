using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartExpenseAnalyzer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable modern visual styles (rounded buttons, themed controls, etc.)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Launch the main application window
            Application.Run(new SmartExpenseAnalyzer.UI.MainForm());
        }

    }
}
