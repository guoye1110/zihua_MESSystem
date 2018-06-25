using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomControl.TimeLineControl
{
    public class DataPointEnumerator : IEnumerator

    {
        List<DataPoint> dataPoints;
        int current = -1;

        public DataPointEnumerator(List<DataPoint> points)
        {
            dataPoints = points;
        }

        public object Current
        {
            get { return CurrentDataPoint(); }
        }

        object CurrentDataPoint()
        {
            if (current < 0 || current > dataPoints.Count)
                return null;
            else
                return dataPoints[current];
        }

        public bool MoveNext()
        {
            current++;
            if (current < dataPoints.Count && dataPoints[current] != null)
                return true;
            return false;
        }

        public void Reset()
        {
            current = 0;
        }
    }
}
