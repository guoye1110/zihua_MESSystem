using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.OEEManagement
{
    class OEETimeLine
    {
        /**************************************************** Constant ****************************************************/

        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private List<OEETimeBlock> _timeBlocks = null;

        /***************************************************** Property ***************************************************/
        public string[] Labels
        {
            get;
            set;
        }

        public List<OEETimeBlock> TimeBlocks
        {
            get { return _timeBlocks; }
        }

        public void Add(OEETimeBlock timeBlock)
        {
            _timeBlocks.Add(timeBlock);
        }
    }
}
