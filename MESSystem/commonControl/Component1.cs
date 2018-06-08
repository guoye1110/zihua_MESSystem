using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace MESSystem.commonControl
{
    public partial class myMenu : MenuStrip
    {
        public myMenu()
        {
            InitializeComponent();
            this.Renderer = new CustomProfessionalRenderer();
        }

        public myMenu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.Renderer = new CustomProfessionalRenderer();
        }
    }

    class CustomProfessionalRenderer : ToolStripProfessionalRenderer
    {
        //默认的绘制背景色的颜色
        private Color menu_color = Color.Gray;      //菜单的背景色
        private Color toolbar_color = Color.Gray;   //工具栏的背景色
        private Color image_color = Color.Gray;     //菜单图片栏的背景色
        private Color separator_color = Color.Gray; //菜单分割条的背景色    

        public CustomProfessionalRenderer()
            : base()
        {
        }

        public CustomProfessionalRenderer(Color mColor, Color iColor, Color sColor)
            : base()
        {
            menu_color = mColor;
            image_color = iColor;
            separator_color = sColor;
        }

        public CustomProfessionalRenderer(Color tColor)
            : base()
        {
            toolbar_color = tColor;
        }


        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            //判断ToolStrip的类型
            ToolStrip tsType = e.ToolStrip;
            Graphics g = e.Graphics;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (tsType is MenuStrip ||
                tsType is ToolStripDropDown)
            {
                //指定填充Menu栏与ToolBar栏的背景色的画刷,使用线性渐变画刷
                LinearGradientBrush lgBursh = new LinearGradientBrush(new Point(0, 0),
                                                                      new Point(0, tsType.Height),
                                                                      Color.FromArgb(255, Color.White),
                                                                      Color.FromArgb(150, menu_color));
                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                int diameter = 10;//直径                
                Rectangle rect = new Rectangle(Point.Empty, tsType.Size);
                Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

                path.AddLine(0, 0, 10, 0);
                // 右上角
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                // 右下角
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                // 左下角
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                path.CloseFigure();

                //设置控件的窗口区域
                tsType.Region = new Region(path);

                //填充窗口区域
                g.FillPath(lgBursh, path);
            }
            else if (tsType is ToolStrip)
            {
                //指定填充Menu栏与ToolBar栏的背景色的画刷,使用线性渐变画刷
                LinearGradientBrush lgBursh = new LinearGradientBrush(new Point(0, 0),
                                                                      new Point(0, tsType.Height),
                                                                      Color.FromArgb(255, Color.White),
                                                                      Color.FromArgb(150, toolbar_color));
                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                int diameter = 10;//直径                
                Rectangle rect = new Rectangle(Point.Empty, tsType.Size);
                Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

                path.AddLine(0, 0, 10, 0);
                // 右上角
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                // 右下角
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                // 左下角
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                path.CloseFigure();

                //设置控件的窗口区域
                tsType.Region = new Region(path);

                //填充窗口区域
                g.FillPath(lgBursh, path);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //判断ToolStrip的类型
            ToolStrip tsType = e.ToolStrip;
            Graphics g = e.Graphics;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (tsType is MenuStrip ||
                tsType is ToolStripDropDown)
            {
                //设置画笔
                Pen LinePen = new Pen(menu_color);
                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                int diameter = 10;//直径                
                Rectangle rect = new Rectangle(Point.Empty, tsType.Size);
                Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

                path.AddLine(0, 0, 10, 0);
                // 右上角
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                // 右下角
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                // 左下角
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                path.CloseFigure();

                //画边框
                g.DrawPath(LinePen, path);
            }
            else if (tsType is ToolStrip)
            {
                //设置画笔
                Pen LinePen = new Pen(toolbar_color);
                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                int diameter = 10;//直径                
                Rectangle rect = new Rectangle(Point.Empty, tsType.Size);
                Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

                path.AddLine(0, 0, 10, 0);
                // 右上角
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                // 右下角
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                // 左下角
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                path.CloseFigure();

                //画边框
                g.DrawPath(LinePen, path);
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = menu_color;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            ToolStripItem item = e.Item;
            ToolStrip tsType = e.ToolStrip;

            g.SmoothingMode = SmoothingMode.HighQuality;

            //渲染顶级项
            if (tsType is MenuStrip)
            {
                if (e.Item.Selected)
                {
                    Pen LinesPen = new Pen(Color.FromArgb(205, 226, 252));
                    Point[] LinePoint = { new Point(0, 2),
                                           new Point(item.Size.Width - 1, 2),
                                           new Point(item.Size.Width - 1, item.Size.Height - 3),
                                           new Point(0, item.Size.Height - 3),
                                           new Point(0, 2)};
                    g.DrawLines(LinesPen, LinePoint);

                    SolidBrush brush = new SolidBrush(Color.FromArgb(197, 228, 253));
                    Rectangle rect = new Rectangle(0, 2, item.Size.Width - 1, item.Size.Height - 5);
                    g.FillRectangle(brush, rect);
                }
                if (item.Pressed)
                {
                    Pen LinesPen = new Pen(Color.FromArgb(197, 228, 253));
                    Point[] LinePoint = { new Point(0, 2),
                                           new Point(item.Size.Width - 1, 2),
                                           new Point(item.Size.Width - 1, item.Size.Height - 3),
                                           new Point(0, item.Size.Height - 3),
                                           new Point(0, 2)};
                    g.DrawLines(LinesPen, LinePoint);
                }
            }
            //渲染下拉项
            else if (tsType is ToolStripDropDown)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                LinearGradientBrush lgbrush = new LinearGradientBrush(new Point(0, 0), new Point(item.Width, 0), Color.FromArgb(200, menu_color), Color.FromArgb(0, Color.White));
                if (item.Selected)
                {
                    GraphicsPath gp = GetRoundedRectPath(new Rectangle(0, 0, item.Width, item.Height), 10);
                    g.FillPath(lgbrush, gp);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }


        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics g = e.Graphics;

            ToolStrip tsType = e.ToolStrip;

            if (tsType is ToolStripDropDown)
            {
                LinearGradientBrush lgbrush = new LinearGradientBrush(new Point(0, 0),
                                                                  new Point(e.Item.Width, 0),
                                                                  separator_color,
                                                                  Color.FromArgb(0, separator_color));
                g.FillRectangle(lgbrush, new Rectangle(33, e.Item.Height / 2, e.Item.Width / 4 * 3, 1));
            }
        }
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            //base.OnRenderImageMargin(e);
            //屏蔽掉左边图片竖条

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle image_rect = e.AffectedBounds;

            //SolidBrush brush = new SolidBrush(image_color);
            LinearGradientBrush lgbrush = new LinearGradientBrush(new Point(0, 0),
                                                                  new Point(image_rect.Width, 0),
                                                                  Color.FromArgb(200, image_color),
                                                                  Color.FromArgb(0, Color.White));
            Rectangle rect = new Rectangle(0, 0, image_rect.Width, image_rect.Height);
            g.FillRectangle(lgbrush, rect);
        }


        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            ToolStripItem item = e.Item;

            if (item.Selected)
            {
                Pen LinesPen = new Pen(Color.FromArgb(205, 226, 252));
                Point[] LinePoint = { new Point(0, 2),
                                           new Point(item.Size.Width - 1, 2),
                                           new Point(item.Size.Width - 1, item.Size.Height - 3),
                                           new Point(0, item.Size.Height - 3),
                                           new Point(0, 2)};
                g.DrawLines(LinesPen, LinePoint);

                SolidBrush brush = new SolidBrush(Color.FromArgb(197, 228, 253));
                Rectangle rect = new Rectangle(0, 2, item.Size.Width - 1, item.Size.Height - 5);
                g.FillRectangle(brush, rect);
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }


        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
