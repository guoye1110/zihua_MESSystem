using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomControl.TimeLineControl
{
    public partial class TimeLine : Control
    {
        private const int paddingX = 5;
        private const int paddingY = 15;
        private const int BAR_HEIGHT = 20;
        private const int padding_labelY = 5;
        private const int padding_labelX = 5;
        private const int padding_lengendX = 15;
        private const int padding_lengendY = 3;

        List<Series> _series;
        int _bars;
        string[] _labelsX;
        string[] _labelsY;
        Font _lengedFont;
        Font _labelFont;

        int screenWidth;
        int screenHeight;

        double ratioX;
        double ratioY;

        int blockBarWidth;
        int blockBarHeight;

        int span;
        int scale;
        int labelscale;

        double scaleX;
        double scaleY;

        float labelY_Max_Width;

        int locX = 0;
        int locY = padding_lengendY;

        public List<Series> Series
        {
            get { return _series; }
        }

        public int Bars
        {
            set { _bars = value; }
        }

        public string[] LabelsX
        {
            set { _labelsX = value; }
        }

        public string[] LabelsY
        {
            set
            {
                _labelsY = value;
                Graphics g = CreateGraphics();
                foreach (string label in _labelsY)
                {
                    float width = g.MeasureString(label, _labelFont).Width;
                    if (width > labelY_Max_Width)
                        labelY_Max_Width = width;
                }
                g.Dispose();
            }
        }

        public Font LengendFont
        {
            set { _lengedFont = value; }
        }

        public Font LabelFont
        {
            set { _labelFont = value; }
        }

        public TimeLine(int timeSpan, int timeScale, int labelInterval)
        {
            InitializeComponent();
            _series = new List<Series>();

            screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            span = timeSpan;
            scale = timeScale;
            labelscale = labelInterval / timeScale;
            labelY_Max_Width = 0;
            this.Location = new Point(0, 0);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            locX = 0;
            locY = padding_lengendY;

            DrawLengend(pe);
            DrawLabels(pe);
            foreach (Series series in _series)
            {
                foreach (DataPoint dataPoint in series.Points)
                    DrawBlocks(dataPoint, series.Color, pe);
            }
           
            base.OnPaint(pe);
        }

        private void DrawLengend(PaintEventArgs pe)
        {
          
            double total_lengendWidth = 0;
            double max_lengendWidth = 0;
            double lengendHeight = 0;

           
            Graphics g = pe.Graphics;

            for (int index = 0; index < _series.Count; index++)
            {
                double lengendWidth = g.MeasureString(_series[index].Lengend, _lengedFont).Width;
                if (lengendWidth > max_lengendWidth)
                    max_lengendWidth = lengendWidth;
            }

            lengendHeight = g.MeasureString(_series[0].Lengend, _lengedFont).Height;
            lengendHeight += padding_lengendY * 2;
            max_lengendWidth += padding_lengendX * 2;
            total_lengendWidth = max_lengendWidth * _series.Count;

            double lengend_locX = locX + (this.Width - total_lengendWidth) / 2;
            double lengend_locY = locY;

            for (int index = 0; index < _series.Count; index++)
            {
                using (SolidBrush rectBrush = new SolidBrush(_series[index].Color),
                    fontBrush = new SolidBrush(Color.Black))
                {
                    Rectangle rect = new Rectangle((int)Math.Ceiling(lengend_locX), (int)lengend_locY, (int)Math.Ceiling(max_lengendWidth), (int)lengendHeight);
                    g.FillRectangle(rectBrush, rect);
                    g.DrawString(_series[index].Lengend, _lengedFont, fontBrush, (int)(lengend_locX + padding_lengendX), (int)(lengend_locY + padding_lengendY));
                    lengend_locX += max_lengendWidth;
                }
            }

            locY += (int)(lengendHeight + paddingY);

            ratioX = (double)this.Width / (double)screenWidth;
            ratioY = (double)this.Height / (double)screenHeight;
            blockBarWidth = this.Width - paddingX * 2 * (int)Math.Ceiling(ratioX) - (int)Math.Ceiling(labelY_Max_Width);
            blockBarHeight = this.Height - paddingY * 2 * (int)Math.Ceiling(ratioY) - (int)(lengendHeight + paddingY);
            scaleX = (double)blockBarWidth / (span / scale);
            scaleY = ((double)blockBarHeight - (_bars * BAR_HEIGHT)) / (_bars - 1);
        }

        private void DrawBlocks(DataPoint dataPoint, Color color, PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            SolidBrush brush = new SolidBrush(color);
           // double x = this.Location.X;
           // double y = this.Location.Y;

            double x = locX;
            double y = locY;
            x += dataPoint.X * scaleX + paddingX * ratioX + (int)Math.Ceiling(labelY_Max_Width);
            y += (dataPoint.Y * scaleY) + paddingY * ratioY;
            Rectangle rect = new Rectangle((int)Math.Ceiling(x), (int)Math.Ceiling(y), (int)Math.Ceiling(dataPoint.Value * scaleX), BAR_HEIGHT);
            g.FillRectangle(brush, rect);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ratioX = (double)this.Width / (double)screenWidth;
            ratioY = (double)this.Height / (double)screenHeight;
            blockBarWidth = this.Width - paddingX * 2 * (int)Math.Ceiling(ratioX) - (int)Math.Ceiling(labelY_Max_Width);
            blockBarHeight = this.Height - paddingY * 2 * (int)Math.Ceiling(ratioY);
            scaleX = (double)blockBarWidth / (span / scale);
            scaleY = ((double)blockBarHeight - (_bars * BAR_HEIGHT)) / (_bars - 1);
            this.Invalidate();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
          //  double x = this.Location.X;
         //   double y = this.Location.Y;
         //   System.Console.WriteLine("OnLocationChanged locx{0}, locY{1}", x, y);
            this.Invalidate();
           
        }
        private void DrawLabels(PaintEventArgs pe)
        {
            double offsetX;
            double offsetY;
            double labelWidth;
            double labelHeight;
            SolidBrush labelBrush = new SolidBrush(Color.Black);


            Graphics g = pe.Graphics;

         //   int locX = this.Location.X;
          //  int locY = this.Location.Y;

            foreach (string label in _labelsY)
            {
                float width = g.MeasureString(label, _labelFont).Width;
                if (width > labelY_Max_Width)
                    labelY_Max_Width = width;
            }

            labelHeight = g.MeasureString(_labelsY[0], _labelFont).Height;
            offsetX = locX + paddingX * ratioX;
            offsetY = locY + paddingY * ratioY + (BAR_HEIGHT - labelHeight) / 2;
            for (int i = 0; i < _bars; i++)
            {
                g.DrawString(_labelsY[i], _labelFont, labelBrush, (float)offsetX, (float)(offsetY));
                offsetY += scaleY;
            }

            offsetY = locY + BAR_HEIGHT + paddingY * ratioY;
            
            for (int i = 0; i < _bars; i++)
            {
                offsetX = locX + paddingX * ratioX + labelY_Max_Width;
                
                foreach (string label in _labelsX)
                {
                    labelWidth = g.MeasureString(label, _labelFont).Width;
                    offsetX -= labelWidth / 2;
                    if (offsetX + labelWidth / 2  > blockBarWidth)
                    {
                        if (offsetX > blockBarWidth)
                            offsetX -= (offsetX - blockBarWidth) - labelWidth;
                        else
                            offsetX -= (labelWidth - (blockBarWidth - offsetX));
                    }
                        
                    g.DrawString(label, _labelFont, labelBrush, (float)offsetX, (float)(offsetY + padding_labelY));
                    offsetX += ((labelscale * scaleX) + labelWidth / 2);
                }
                offsetY += scaleY;
            }

        }
    }
}
