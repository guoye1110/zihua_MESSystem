using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.OEEManagement
{
    class OEETimeLineChart
    {
        /**************************************************** Constant ****************************************************/

        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private Padding _margin;
        private List<OEETimeLine> _timeLines = null;

        /***************************************************** Property ***************************************************/

        public Padding Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }
        public List<OEETimeLine> TimeLines
        {
            get { return _timeLines; }
        }

        public void Add(OEETimeLine timeLine)
        {

        }
    }
}
