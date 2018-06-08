using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomControl.TimeLineControl
{
    public class DataPoint
    {
        private int _x;
        private int _y;
        private int _value;
        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int Value
        {
            get { return _value; }
        }

        public DataPoint(int x, int y, int value)
        {
            _x = x;
            _y = y;
            _value = value;
        }
    }
}
