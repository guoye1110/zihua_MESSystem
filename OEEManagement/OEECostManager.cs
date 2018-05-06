using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.OEEManagement
{
    public partial class OEECostManager : Form
    {

        /**************************************************** Constant ****************************************************/

        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private OEEMachineGroup[] machineGroups;
        //private OEEMachine[] machines;
        /***************************************************** Property ***************************************************/

        public OEECostManager(OEEFactory factory)
        {
            machineGroups = factory.MachineGroups;
            InitializeComponent();
        }
    }
}
