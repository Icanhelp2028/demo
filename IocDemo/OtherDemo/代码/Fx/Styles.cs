using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Fx
{
    //图表样式已经改变
    public delegate void ChartStyleChangedEventHandler(ChartStyle style);

    public enum Chartsticks
    {
        BarChart,
        Candlesticks,
        LineChart
    }

    public interface IIndicator
    {
        void Start(IndicatorInfo info);
    }

    /// <summary>柱信息</summary>
    [Serializable]
    public class Bar
    {
        /// <summary>收盘价</summary>
        public decimal Close;

        /// <summary>最高价</summary>
        public decimal High;

        /// <summary>最低价</summary>
        public decimal Low;

        /// <summary>开盘价</summary>
        public decimal Open;

        /// <summary>开盘时间</summary>
        public DateTime Time;

        /// <summary>成交量</summary>
        public decimal Volume;
    }

    //图表设置
    public class ChartProperties
    {
        public ChartProperties()
        {
            this.ChartShift = true;
            this.AutoOffset = true;
            this.ShowOHLC = true;
            this.ShowPeriodSeparators = false;
            this.ShowGrid = false;
            this.ShowVolumes = false;
            this.Chartsticks = Chartsticks.Candlesticks;
        }

        /// <summary>自动滚动图表</summary>
        [DefaultValue(true)]
        public bool AutoOffset { get; set; }

        /// <summary>平移图表</summary>
        [DefaultValue(true)]
        public bool ChartShift { get; set; }

        [DefaultValue(typeof(Chartsticks), "Candlesticks")]
        public Chartsticks Chartsticks { get; set; }

        /// <summary>显示网格</summary>
        [DefaultValue(false)]
        public bool ShowGrid { get; set; }

        /// <summary>显示开盘最高最低收盘价</summary>
        [DefaultValue(true)]
        public bool ShowOHLC { get; set; }

        /// <summary>显示时段间隔</summary>
        [DefaultValue(false)]
        public bool ShowPeriodSeparators { get; set; }

        /// <summary>显示成交量</summary>
        [DefaultValue(false)]
        public bool ShowVolumes { get; set; }
    }

    //图表样式
    public class ChartStyle
    {
        public ChartStyleChangedEventHandler StyleChange;

        public ChartStyle()
        {
            this.Background = Color.Black;
            this.Foreground = Color.White;
            this.Grid = Color.LightSlateGray;
            this.Barup = Color.White;
            this.Bardown = Color.Red;
            this.Bullcandle = Color.White;
            this.Bearcandle = Color.Red;
            this.Linegraph = Color.Lime;
            this.Volumes = Color.LimeGreen;
        }

        /// <summary>前景</summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color Background { get; set; }

        /// <summary>阴柱</summary>
        [DefaultValue(typeof(Color), "Red")]
        public Color Bardown { get; set; }

        /// <summary>阳柱</summary>
        [DefaultValue(typeof(Color), "White")]
        public Color Barup { get; set; }

        /// <summary>阴烛</summary>
        [DefaultValue(typeof(Color), "Red")]
        public Color Bearcandle { get; set; }

        /// <summary>阴柱</summary>
        [DefaultValue(typeof(Color), "White")]
        public Color Bullcandle { get; set; }

        /// <summary>背景</summary>
        [DefaultValue(typeof(Color), "White")]
        public Color Foreground { get; set; }

        /// <summary>网格</summary>
        [DefaultValue(typeof(Color), "LightSlateGray")]
        public Color Grid { get; set; }

        /// <summary>折线图</summary>
        [DefaultValue(typeof(Color), "Lime")]
        public Color Linegraph { get; set; }

        /// <summary>成交量</summary>
        [DefaultValue(typeof(Color), "LimeGreen")]
        public Color Volumes { get; set; }

        public void OnStyleChange()
        {
            if (StyleChange != null)
                StyleChange(this);
        }
    }

    /// <summary>图表</summary>
    public class ChartView
    {
        private int m_nChartShift = 130;

        public ChartView()
        {
            this.Zoom = 1;
            this.ScrollBarCount = 0;
            this.Digits = 5;
            this.SymbolWidth = 40;
            this.Style = new ChartStyle();
            this.Properties = new ChartProperties();
            this.Bars = new List<Bar>();
        }

        /// <summary>柱数据</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<Bar> Bars { get; set; }

        /// <summary>平移距离</summary>
        [Browsable(false)]
        [DefaultValue(130)]
        public int ChartShift
        {
            get
            {
                return m_nChartShift;
            }
            set
            {
                if (value < 130)
                    m_nChartShift = 130;
                else
                    m_nChartShift = value;
            }
        }
        //Width=39.26909, Height=13.14583
        [Browsable(false)]
        [DefaultValue(40)]
        public int SymbolWidth { get; set; }

        [Browsable(false)]
        [DefaultValue(5)]
        public int Digits { get; set; }

        public ChartProperties Properties { get; set; }

        /// <summary>平移距离 柱数为单位</summary>
        [Browsable(false)]
        [DefaultValue(0)]
        public int ScrollBarCount { get; set; }

        public ChartStyle Style { get; set; }

        /// <summary>图表缩放大小</summary>
        [Browsable(false)]
        [DefaultValue(1)]
        public int Zoom { get; set; }
    }

    public class Indicator : IIndicator
    {
        public void Start(IndicatorInfo info)
        {
            if (info.VisibleBarCount < 1)
                return;
            float high, low, close, open;
            Graphics gdi = info.GDI;
            Color fg = info.Style.Foreground;
            string format = "f" + info.ChartView.Digits;

            using (Brush brushfgFont = new SolidBrush(fg))
            using (Brush brushInvfgFont = new SolidBrush(Color.FromArgb(255, 255 - fg.R, 255 - fg.G, 255 - fg.B)))
            using (Pen penGrid = new Pen(info.Style.Grid))
            using (Pen penBarup = new Pen(info.Style.Barup))
            using (Brush brushBarup = new SolidBrush(info.Style.Bullcandle))
            using (Pen penBardown = new Pen(info.Style.Bardown))
            using (Pen penLine = new Pen(info.Style.Linegraph))
            using (Brush brushBardown = new SolidBrush(info.Style.Bearcandle))
            {
                int dx = info.RightMargin - info.OffsetShift - 1;
                close = info.GetYPos(info.Bars[0].Close);

                gdi.DrawLine(penGrid,
                 new PointF(info.LeftMargin + 1, close),
                 new PointF(info.RightMargin + 1, close));

                float height = info.StrSize.Height;
                gdi.FillRectangle(brushfgFont,
                    (float)(info.RightMargin + 2),
                    close - height / 2,
                    (float)info.ChartWidth - info.RightMargin - 3,
                    height);
                gdi.DrawString(info.Bars[0].Close.ToString(format),
                    info.ChartFont,
                    brushInvfgFont, info.RightMargin + 6, close - height / 2 - 0.5f);

                for (int i = info.ScrollBarCount; i < info.VisibleBarCount; i++)
                {
                    dx -= info.BarWidth + info.BarPadding;
                    high = info.GetYPos(info.Bars[i].High);
                    low = info.GetYPos(info.Bars[i].Low);
                    close = info.GetYPos(info.Bars[i].Close);
                    open = info.GetYPos(info.Bars[i].Open);
                    switch (info.Properties.Chartsticks)
                    {
                        case Chartsticks.Candlesticks:
                            if (info.Bars[i].Close > info.Bars[i].Open)
                            {
                                gdi.DrawLine(penBarup,
                                    new PointF(dx - info.BarCenter, high),
                                    new PointF(dx - info.BarCenter, low));
                                gdi.FillRectangle(brushBarup,
                                    (float)(dx - info.BarWidth),
                                    close,
                                    (float)info.BarWidth,
                                    open - close);
                                gdi.DrawRectangle(penBarup,
                                    (float)(dx - info.BarWidth),
                                    close,
                                    (float)info.BarWidth,
                                    open - close);
                            }
                            else if (info.Bars[i].Close < info.Bars[i].Open)
                            {
                                gdi.DrawLine(penBardown,
                                    new PointF(dx - info.BarCenter, high),
                                    new PointF(dx - info.BarCenter, low));
                                gdi.FillRectangle(brushBardown,
                                    (float)(dx - info.BarWidth),
                                    open,
                                    (float)info.BarWidth,
                                    close - open);
                                gdi.DrawRectangle(penBardown,
                                    (float)(dx - info.BarWidth),
                                    open,
                                    (float)info.BarWidth,
                                    close - open);
                            }
                            else
                            {
                                gdi.DrawLine(penLine,
                                    new PointF(dx - info.BarCenter, high),
                                    new PointF(dx - info.BarCenter, low));
                                gdi.DrawLine(penLine,
                                    new PointF(dx - info.BarWidth, open),
                                    new PointF(dx, open));
                                gdi.DrawRectangle(penLine,
                                    (float)(dx - info.BarWidth),
                                    close,
                                    (float)info.BarWidth,
                                    open - close);
                            }
                            break;

                        case Chartsticks.LineChart:
                            break;
                    }
                }
            }
        }
    }

    public class IndicatorInfo
    {
        public int BarCenter = 0;
        public int VisibleBarCount = 0;
        public int BarPadding = 0;
        public IList<Bar> Bars = null;
        public int BarWidth = 0;
        public ChartView ChartView;
        public Graphics GDI;
        public int OffsetShift = 0;
        public ChartProperties Properties;
        public int LeftMargin = 0, RightMargin = 0;
        public int ScrollBarCount = 0;
        public int VisibleBarsCounter;
        public ChartStyle Style;
        public decimal TickHeight = 0;
        public decimal TickPadding = 0;
        private decimal Min = 0;
        private decimal PerTick = 0;
        public int ChartWidth = 0;
        public Font ChartFont;
        public SizeF StrSize;
        public IndicatorInfo(ChartView chartView, decimal min, decimal perTick)
        {
            this.ChartView = chartView;
            this.Bars = chartView.Bars;
            this.Min = min;
            this.PerTick = perTick;
        }

        //坐标转换
        //decimal tick = (1.37400m - pMin) / PerTick;
        //float ytick = (float)(TickHeight - tick * TickPadding);
        public float GetYPos(decimal price)
        {
            if (price == Min)
            {
                return (float)TickHeight;
            }
            else
            {
                decimal tick = (price - Min) / PerTick;
                float ytick = (float)(TickHeight - tick * TickPadding);
                return ytick;
            }
        }
    }
}