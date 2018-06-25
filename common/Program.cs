using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESSystem.mainUI;

namespace MESSystem.common
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

            if (gVariable.workshopReport == gVariable.WORKSHOP_REPORT_NONE || gVariable.workshopReport == gVariable.WORKSHOP_REPORT_FUNCTION)
                Application.Run(new firstScreen());
            else //if(gVariable.workshopReport == gVariable.WORKSHOP_REPORT_BULLETIN)
                Application.Run(new workshopReport());
        }
    }
}
