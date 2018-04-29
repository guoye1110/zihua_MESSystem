using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using MESSystem.common;

namespace MESSystem.OEEManagement
{
    public class OEEWorkshop
    {
        /**************************************************** Constant ****************************************************/

        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private string _name;

        /***************************************************** Property ***************************************************/

        public string Name
        {
            get { return _name; }
        }

        public OEEWorkshop(string name)
        {
            _name = name;
        }
    }
}
