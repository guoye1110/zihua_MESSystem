using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomControl.MonthCalendarControl
{
    class Lengend : Panel
    {
        public enum LayoutStyle { horizontal, vertical};

        private const int RECT_WIDTH = 10;
        private const int PADDING = 5;
        private const int INTERVAL_X = 20;
        private const int INTERVAL_Y = 5;

        private Dictionary<Color, string> _labels;
        private LayoutStyle _style;

        public Dictionary<Color, string> Labels
        {
            get { return _labels; }
        }

        public LayoutStyle Style
        {
            set { _style = value; }
            get { return _style; }
        }
  
        public Lengend()
        {
            _labels = new Dictionary<Color, string>();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int locX = 0;
            int locY = 10;
            int width = 0;
            int height = 0;

            Graphics g = e.Graphics;
            using (SolidBrush fontBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Microsoft Sans Serif", 15, FontStyle.Regular))
            {
                foreach(KeyValuePair<Color,string> pair in _labels)
                {
                    int offsetX, offsetY;
                    int text_width = (int)Math.Ceiling(g.MeasureString(pair.Value, font).Width);
                    int text_height = (int)Math.Ceiling(g.MeasureString(pair.Value, font).Height);
                    if (_style == LayoutStyle.horizontal)
                    {
                        locX += width;
                        offsetX = locX + PADDING + RECT_WIDTH;
                        offsetY = locY;
                        width = RECT_WIDTH + PADDING + text_width + INTERVAL_X;
                    }
                    else
                    {
                        locY += height;
                        offsetX = locX;
                        offsetY = locY + PADDING;
                        height = RECT_WIDTH + text_height + INTERVAL_Y;
                    }

                    using (SolidBrush shapeBrush = new SolidBrush(pair.Key))
                    {
                        Rectangle rect = new Rectangle(locX, locY, RECT_WIDTH, RECT_WIDTH);
                        g.FillEllipse(shapeBrush, rect);
                    }    
                    g.DrawString(pair.Value, font, fontBrush, offsetX, offsetY);
                }
      
            };
            base.OnPaint(e);
        }
    }
}
