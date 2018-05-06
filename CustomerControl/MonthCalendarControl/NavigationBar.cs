using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomControl.MonthCalendarControl
{
    class NavigationBar : Panel
    {
        public enum BarStyle { ArrowMode, None };
        public const int BAR_HEIGHT = NAVIGATION_HEIGHT + SPLIT_LINE_WIDTH;

        public delegate void LeftArrow();
        public event LeftArrow OnLeftArrow;

        public delegate void RightArrow();
        public event RightArrow OnRightArrow;

        private const int NAVIGATION_HEIGHT = 30;
        public const int SPLIT_LINE_WIDTH = 2;
        private const int ARROW_SIZE = NAVIGATION_HEIGHT;
        private const int ARROW_SPLIT = 4;

        private Color BAR_BACK_COLOR = Color.Honeydew;
        private Color ARROW_CLICKED_COLOR = Color.Blue;
        private Color ARROW_COLOR = Color.Black;

        BarStyle barStyle;

        Panel rightArrow;
        Panel leftArrow;
        Label[] middleLabel;
        int column;

        bool leftArrow_entered;
        bool rightArrow_entered;

        public NavigationBar(BarStyle style, int col)
        {
            column = col;

            this.BackColor = BAR_BACK_COLOR;

            barStyle = style;

            AddArrows();
            AddLabels();

            leftArrow_entered = false;
            rightArrow_entered = false;
        }

        private void AddArrows()
        {
            int locX = this.Location.X;
            int locY = this.Location.Y + SPLIT_LINE_WIDTH;

            if (barStyle == BarStyle.ArrowMode)
            {
                leftArrow = new Panel();
                leftArrow.Location = new Point(locX, locY);
                leftArrow.Size = new Size(ARROW_SIZE, this.Height - SPLIT_LINE_WIDTH);
                leftArrow.BackColor = BAR_BACK_COLOR;
                leftArrow.Paint += new PaintEventHandler(LeftArrow_Paint);
                
                leftArrow.Click += new EventHandler(LeftArrow_Click);
                leftArrow.MouseEnter += new EventHandler(LeftArrow_MouseEnter);
                leftArrow.MouseLeave += new EventHandler(LeftArrow_MouseLeave);
                this.Controls.Add(leftArrow);

                rightArrow = new Panel();
                rightArrow.Location = new Point(locX + this.Width - ARROW_SIZE, locY);
                rightArrow.Size = new Size(ARROW_SIZE, this.Height - SPLIT_LINE_WIDTH);
                rightArrow.BackColor = BAR_BACK_COLOR;
                rightArrow.Paint += new PaintEventHandler(RightArrow_Paint);
            
                rightArrow.Click += new EventHandler(RightArrow_Click);
                rightArrow.MouseEnter += new EventHandler(RightArrow_MouseEnter);
                rightArrow.MouseLeave += new EventHandler(RightArrow_MouseLeave);
                this.Controls.Add(rightArrow);
            }
        }

        private void AddLabels()
        {
            int locX = this.Location.X;
            int locY = this.Location.Y + SPLIT_LINE_WIDTH;
            int arrowWidth = 0;

            if (barStyle == BarStyle.ArrowMode)
                arrowWidth = ARROW_SIZE;

            int labelWidth = (this.Width - 2 * arrowWidth) / column;
            int labelHeight = this.Height - SPLIT_LINE_WIDTH;
            middleLabel = new Label[column];
            for (int i = 0; i < column; i++)
            {
                int x = locX + arrowWidth + i * labelWidth;
                int y = locY;
                middleLabel[i] = new Label();
                middleLabel[i].Text = "";
             
                middleLabel[i].Size =
                    new Size(labelWidth, labelHeight);
                middleLabel[i].Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                middleLabel[i].Location = new Point(x, y);
                middleLabel[i].TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(middleLabel[i]);
            }
        }

        public void UpdateLabelText(int year, int month, int index)
        {
            string text;
            if (barStyle == BarStyle.ArrowMode)
                text = year.ToString() + "年" +
                    month.ToString() + "月";
            else
                text = year.ToString() + "年" +
                     month.ToString() + "月";
            middleLabel[index].Text = text;
        }

        private void LeftArrow_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush rectangleBrush;
            int row_width = e.ClipRectangle.Height / ARROW_SPLIT;
            int col_width = e.ClipRectangle.Width / ARROW_SPLIT;
            
            if (leftArrow_entered)
                rectangleBrush = new SolidBrush(ARROW_CLICKED_COLOR);
            else
                rectangleBrush = new SolidBrush(ARROW_COLOR);

            Point topPoint = new Point(ARROW_SIZE - col_width,  row_width);
            Point bottomPoint = new Point(ARROW_SIZE - col_width, ARROW_SIZE - row_width);
            Point middlePoint = new Point(col_width, row_width * 2);
            Point[] points = { topPoint, bottomPoint, middlePoint };
            e.Graphics.FillPolygon(rectangleBrush, points);

        }

        private void RightArrow_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush rectangleBrush;

            int row_width = e.ClipRectangle.Height / ARROW_SPLIT;
            int col_width = e.ClipRectangle.Width / ARROW_SPLIT;

            if (rightArrow_entered)
                rectangleBrush = new SolidBrush(ARROW_CLICKED_COLOR);
            else
                rectangleBrush = new SolidBrush(ARROW_COLOR);

            Point topPoint = new Point(col_width, row_width);
            Point bottomPoint = new Point(col_width, ARROW_SIZE - row_width);
            Point middlePoint = new Point(ARROW_SIZE - col_width, row_width * 2);
            Point[] points = { topPoint, bottomPoint, middlePoint };
            e.Graphics.FillPolygon(rectangleBrush, points);
        }

        private void LeftArrow_Click(object sender, EventArgs e)
        {
            OnLeftArrow();    
        }

        private void LeftArrow_MouseEnter(object sender, EventArgs e)
        {
            leftArrow_entered = true;
            ((Panel)sender).Invalidate();
        }

        private void LeftArrow_MouseLeave(object sender, EventArgs e)
        {
            leftArrow_entered = false;
            ((Panel)sender).Invalidate();
        }

        private void RightArrow_Click(object sender, EventArgs e)
        {
            OnRightArrow();
        }

        private void RightArrow_MouseEnter(object sender, EventArgs e)
        {
            rightArrow_entered = true;
            ((Panel)sender).Invalidate();
        }
        

        private void RightArrow_MouseLeave(object sender, EventArgs e)
        {
            rightArrow_entered = false;
            ((Panel)sender).Invalidate();
        }

    

        protected override void OnResize(EventArgs e)
        {
            int labelWidth = (this.Width - 2 * ARROW_SIZE) / column;
            int labelHeight = this.Height;
            int locX = this.Location.X;
            int locY = this.Location.Y + SPLIT_LINE_WIDTH;

            if (barStyle == BarStyle.ArrowMode)
            {
                leftArrow.Location = new Point(locX, locY);

                rightArrow.Location = new Point(locX + this.Width - ARROW_SIZE, locY);
            }

            for (int i=0; i< column; i++)
            {
                int x = locX + ARROW_SIZE + i * labelWidth;
                int y = locY;

                middleLabel[i].Size =
                    new Size(labelWidth, labelHeight);
                middleLabel[i].Location = new Point(x, y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //System.Console.WriteLine("{0}, {1}, {2}", this.Location.X, this.Location.Y, e.ClipRectangle.ToString());
            SolidBrush lineBrush = new SolidBrush(Color.Gray);
            Rectangle rect = new Rectangle(0, 0, this.Width, SPLIT_LINE_WIDTH);
            e.Graphics.FillRectangle(lineBrush, rect);


            base.OnPaint(e);
        }
    }
}
