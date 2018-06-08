using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CustomControl.TimeLineControl
{
    public class Series
    {
        private Color _color;
        private string _lengend;
        private DataPointCollection _points;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string Lengend
        {
            get { return _lengend; }
            set { _lengend = value; }
        }

        public DataPointCollection Points
        {
            get { return _points; }
        }

        public Series()
        {
            _points = new DataPointCollection();
        }
    }
}
