using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomControl.TimeLineControl
{
    public class DataPointCollection : IEnumerable
    {
        List<DataPoint> dataPoints;

        public DataPointCollection()
        {
            dataPoints = new List<CustomControl.TimeLineControl.DataPoint>();
        }

        public IEnumerator GetEnumerator()
        {
            return new CustomControl.TimeLineControl.DataPointEnumerator(dataPoints);
        }

        public CustomControl.TimeLineControl.DataPoint Add(int x, int y, int value)
        {
            CustomControl.TimeLineControl.DataPoint dataPoint = new CustomControl.TimeLineControl.DataPoint(x, y, value);
            Add(dataPoint);
            return dataPoint;
        }

        private void Add(CustomControl.TimeLineControl.DataPoint dataPoint)
        {
            dataPoints.Add(dataPoint);
        }

        public void Clear()
        {
            dataPoints.Clear();
        }
    }
}
