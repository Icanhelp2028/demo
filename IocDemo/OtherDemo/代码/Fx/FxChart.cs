using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fx
{
    /// <summary>图表浏览器</summary>
    [Docking(DockingBehavior.Ask)]
    public class FxChart : Control, IDropChart
    {
        private const int BorderBottom = 18;
        private const int BorderLeft = 2;
        private const int BorderTop = 2;
        private int[] BarCenters = new[] { 1, 2, 3, 6, 9, 12 };
        private int[] BarPaddings = new[] { 2, 4, 6, 6, 6, 6 };
        private int[] BarWidths = new[] { 2, 4, 6, 12, 18, 24 };
        private Font ChartFont = new Font(FontFamily.GenericSerif, 8f);
        private Indicator indicator = new Indicator();
        private int[] Remainders = new[] { 50, 100, 50, 100, 50, 50, 50 };
        private IndicatorInfo info = null;
        private DropChart dropChart = null;

        public FxChart()
        {
            base.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            this.ChartView = new ChartView();
        }

        //LW,S,WW//1，1，3//1，3，5//1，5，11
        public ChartView ChartView { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            ChartProperties properties = this.ChartView.Properties;
            ChartStyle style = this.ChartView.Style;

            Graphics g = e.Graphics;
            g.Clear(style.Background);

            //SizeF strSize = g.MeasureString((1m).ToString("f" + ChartView.Digits), ChartFont);
            SizeF strSize = new SizeF(this.ChartView.SymbolWidth, 14);
            Rectangle borderRect = new Rectangle(
                BorderLeft,
                BorderTop,
                this.Width - BorderLeft - (12 + (int)strSize.Width) - 1,
                this.Height - BorderTop - BorderBottom - 1);

            //画图表边框
            using (Pen penFg = new Pen(style.Foreground))
                g.DrawRectangle(penFg, borderRect);

            int offsetShift = 0;

            if (properties.ChartShift)
                offsetShift = ChartView.ChartShift;

            const float figureWidth = 9.1f;

            //画平移图表图标
            float x = borderRect.Right - figureWidth - 1 - offsetShift;
            float y = borderRect.Top + 1;
            using (Brush brush = new SolidBrush(Color.FromArgb(255, 127, 137, 149)))
            {
                g.FillPolygon(brush, new PointF[]
                {
                    new PointF(x, y),
                    new PointF(x + figureWidth, y),
                    new PointF(x + 4.5f, y + 5.0f)
                });
            }
            List<Bar> bars = this.ChartView.Bars;
            int barPadding = BarPaddings[this.ChartView.Zoom - 1];
            int barWidth = BarWidths[this.ChartView.Zoom - 1];

            int nVisibleWidth = (borderRect.Width - offsetShift - barWidth - barPadding);
            //视图可见柱数
            int nVisibleBarsCounter = nVisibleWidth / (barWidth + barPadding);
            if (nVisibleBarsCounter < 1)
                return;
            int nScrollBarCount = this.ChartView.ScrollBarCount;

            //计算最高最低
            decimal highest = decimal.MinValue, lowest = decimal.MaxValue;
            int nVisibleBarCount = nVisibleBarsCounter + nScrollBarCount;
            if (nVisibleBarCount > bars.Count)
                nVisibleBarCount = bars.Count;
            if (nScrollBarCount >= nVisibleBarCount)
                return;
            if (nVisibleBarCount == 0)
                return;
            for (int i = nScrollBarCount; i < nVisibleBarCount; i++)
            {
                if (bars[i].High > highest)
                    highest = bars[i].High;
                if (bars[i].Low < lowest)
                    lowest = bars[i].Low;
            }
            const int tickPadding = 32, TickTop = 14, tickBottom = 4;
            decimal Point = (decimal)Math.Pow(10, -this.ChartView.Digits);
            //扩展最高最低空间
            int remainder = Remainders[this.ChartView.Digits];
            int Power = (int)Math.Pow(10, this.ChartView.Digits);
            decimal dtmp = highest * Power;
            decimal mMax = (dtmp + remainder - dtmp % remainder) * Point;
            dtmp = lowest * Power;
            decimal mMin = (dtmp - dtmp % remainder) * Point;
            //decimal pMax = decimal.Round(highest + Point * 99, 3);
            //decimal pMin = decimal.Round(lowest - Point * 99, 3);

            float centerH = strSize.Height / 2.0f;
            int tickCount = (borderRect.Height - TickTop - tickBottom) / tickPadding;
            if (tickCount == 0)
                return;

            decimal perTick = decimal.Round((mMax - mMin) / (decimal)tickCount, 4);
            if (perTick == 0m)
                perTick = 0.0001m;
            int tickHeight = borderRect.Bottom - tickBottom;

            info = new IndicatorInfo(this.ChartView, mMin, perTick)
            {
                Properties = properties,
                Style = style,
                BarCenter = BarCenters[ChartView.Zoom - 1],
                BarPadding = barPadding,
                VisibleBarCount = nVisibleBarCount,
                VisibleBarsCounter = nVisibleBarsCounter,
                BarWidth = barWidth,
                GDI = g,
                OffsetShift = offsetShift,
                LeftMargin = BorderLeft,
                RightMargin = borderRect.Right,
                ChartWidth = this.Width,
                ScrollBarCount = nScrollBarCount,
                TickHeight = tickHeight,
                TickPadding = tickPadding,
                ChartFont = ChartFont,
                StrSize = strSize
            };

            //画刻度尺
            string format = "f" + this.ChartView.Digits;
            using (Pen penFg = new Pen(style.Foreground))
            {
                using (Brush brFg = new SolidBrush(style.Foreground))
                {
                    int yTick;
                    decimal mTick;
                    for (int nTick = 0; nTick <= tickCount; nTick++)
                    {
                        yTick = tickHeight - nTick * tickPadding;
                        mTick = (nTick * perTick + mMin);

                        g.DrawLine(penFg, borderRect.Right, yTick, borderRect.Right + 5, yTick);
                        g.DrawString(mTick.ToString(format), ChartFont, brFg, borderRect.Right + 6, yTick - centerH);
                    }
                }
            }
            indicator.Start(info);
            if (this.dropChart == null)
            {
                this.dropChart = new DropChart(this);
                this.MouseMove += this.dropChart.OnMouseMove;
                this.MouseDown += this.dropChart.OnMouseDown;
                this.MouseUp += this.dropChart.OnMouseUp;
            }
        }


        #region IDropChart
        private class DropChart
        {
            private Point FirstHits = Point.Empty;
            private int LastScrollBarCount = 0;
            private IDropChart dropChart;

            public DropChart(IDropChart dropChart)
            {
                this.dropChart = dropChart;
            }

            public void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (FirstHits != Point.Empty)
                {
                    int barCount = this.dropChart.GetBarCount();
                    int visibleBars = this.dropChart.GetVisibleBarsCounter();
                    int barSpace = this.dropChart.GetBarSpace();

                    if (barCount < visibleBars)
                        return;
                    int x = e.Location.X - FirstHits.X;
                    int bar = LastScrollBarCount + (int)(x / barSpace);

                    if (bar + visibleBars >= barCount)
                        bar = barCount - visibleBars + 1;

                    if (bar < 0)
                        bar = 0;
                    this.dropChart.SetScrollBarCount(bar);
                    this.dropChart.Invalidate();
                }
            }

            public void OnMouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (FirstHits == Point.Empty)
                    {
                        LastScrollBarCount = this.dropChart.GetScrollBarCount();
                        FirstHits = e.Location;
                    }
                }
            }

            public void OnMouseUp(object sender, MouseEventArgs e)
            {
                if (FirstHits != Point.Empty)
                {
                    FirstHits = Point.Empty;
                }
            }
        }
        int IDropChart.GetBarCount()
        {
            return this.ChartView.Bars.Count;
        }

        int IDropChart.GetVisibleBarsCounter()
        {
            return this.info.VisibleBarsCounter;
        }

        int IDropChart.GetScrollBarCount()
        {
            return this.ChartView.ScrollBarCount;
        }

        void IDropChart.SetScrollBarCount(int sbc)
        {
            this.ChartView.ScrollBarCount = sbc;
        }

        int IDropChart.GetBarSpace()
        {
            return this.info.BarWidth + this.info.BarPadding;
        }

        void IDropChart.Invalidate()
        {
            this.Invalidate();
        }
        #endregion IDropChart
    }

    public interface IDropChart
    {
        int GetBarCount();

        int GetVisibleBarsCounter();

        int GetScrollBarCount();

        int GetBarSpace();

        void SetScrollBarCount(int sbc);

        void Invalidate();
    }
}