using HomeCalc.ChartsLib.Models;
using HomeCalc.ChartsLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeCalc.ChartsLib.Charts
{
    /// <summary>
    /// Interaction logic for AreaPlot.xaml
    /// </summary>
    public partial class AreaPlot : UserControl
    {
        Canvas plotArea;
        
        public AreaPlot()
        {
            InitializeComponent();

            plotArea = FindName("PlotArea") as Canvas;
        }

        #region Properties

        private IEnumerable<IEnumerable<SeriesDoubleBasedElement>> series;
        public IEnumerable<IEnumerable<SeriesDoubleBasedElement>> Series
        {
            get { return series; }
            set
            {
                //SetValue(SeriesProperty, value);
                series = value;
                DrawChart();
            }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
            "Series",
            typeof(IEnumerable<IEnumerable<SeriesDoubleBasedElement>>),
            typeof(AreaPlot),
            new FrameworkPropertyMetadata(
                default(IEnumerable<IEnumerable<SeriesDoubleBasedElement>>),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSeriesChangedCallback),
                new CoerceValueCallback(OnSeriesCoerceCallback))
                );

        private static object OnSeriesCoerceCallback(DependencyObject d, object baseValue)
        {
            if (d is AreaPlot)
            {
                return (d as AreaPlot).OnCoerceSeriesProperty(baseValue);
            }
            else
            {
                return baseValue;
            }
        }

        private object OnCoerceSeriesProperty(object baseValue)
        {
            if (baseValue != null)
            {
                Series = baseValue as IEnumerable<IEnumerable<SeriesDoubleBasedElement>>;
            }
            return series;
        }

        private static void OnSeriesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AreaPlot)
            {
                (d as AreaPlot).OnSeriesPropertyChanged(e);
            }
        }

        private void OnSeriesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var localValue = ReadLocalValue(SeriesProperty);
            if (localValue != null)
            {
                series = localValue as IEnumerable<IEnumerable<SeriesDoubleBasedElement>>;
            }
        }

        private Brush chartBackground;
        public Brush ChartBackground
        {
            get { return chartBackground; }
            set
            {
                chartBackground = value;
            }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartBackgroundProperty =
            DependencyProperty.Register(
            "ChartBackground",
            typeof(Brush),
            typeof(AreaPlot),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnChartBackgroundChangedCallback),
                new CoerceValueCallback(OnChartBackgroundCoerceCallback))
                );

        private static object OnChartBackgroundCoerceCallback(DependencyObject d, object baseValue)
        {
            if (d is AreaPlot)
            {
                return (d as AreaPlot).OnCoerceChartBackgroundProperty(baseValue);
            }
            else
            {
                return baseValue;
            }
        }

        private object OnCoerceChartBackgroundProperty(object baseValue)
        {
            if (baseValue != null)
            {
                ChartBackground = baseValue as Brush;
            }
            return series;
        }

        private static void OnChartBackgroundChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AreaPlot)
            {
                (d as AreaPlot).OnChartBackgroundPropertyChanged(e);
            }
        }

        private void OnChartBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var localValue = ReadLocalValue(ChartBackgroundProperty);
            if (localValue != null)
            {
                chartBackground = localValue as Brush;
            }
        }

        #endregion

        #region Internal
        private void DrawChart()
        {
            if (plotArea == null)
            {
                return;
            }
            if (Series == null || Series.Count() == 0)
            {
                return;
            }

            ChartPlotInfo plotInfo = CalculatePlotInfo();

            plotArea.Width = plotInfo.Width + plotInfo.LeftLegendWidth + plotInfo.RightLegendWidth;
            plotArea.Height = plotInfo.Height + plotInfo.FooterHeight + plotInfo.HeaderHeight;

            DrawLegend();
            DrawBackground(plotInfo);
            DrawGrid(plotInfo);
            foreach (var seria in Series)
            {
                DrawSeria(seria, plotInfo, BrushGenerator.GetBrush());
            }
        }

        private void DrawBackground(ChartPlotInfo plotInfo)
        {
            var chartBack = new Rectangle();
            chartBack.Width = plotInfo.MaxWidth;
            chartBack.Height = plotInfo.MaxHeight;
            Canvas.SetLeft(chartBack, plotInfo.LeftLegendWidth);
            Canvas.SetTop(chartBack, plotInfo.HeaderHeight);
            chartBack.Fill = ChartBackground;
            
            plotArea.Children.Add(chartBack);
        }

        private void DrawGrid(ChartPlotInfo info)
        {
            DrawLine(0, 0, 0, info.Height, info, Brushes.Red);
            DrawLine(info.Width, 0, info.Width, info.Height, info, Brushes.Red);
            DrawLine(0, info.Height, info.Width, info.Height, info, Brushes.Red);
            DrawLine(0, 0, info.Width, 0, info, Brushes.Red);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, ChartPlotInfo info, Brush brush)
        {
            var line = new Line();
            line.X1 = GeometricHelper.XCoordToChart(x1, info);
            line.X2 = GeometricHelper.XCoordToChart(x2, info);
            line.Y1 = GeometricHelper.YCoordToChart(y1, info);
            line.Y2 = GeometricHelper.YCoordToChart(y2, info);
            line.Stroke = brush;
            line.StrokeThickness = 2;

            plotArea.Children.Add(line);
        }

        private void DrawBezierSegment(Point p1, Point p2, Point p3, ChartPlotInfo info, Brush brush)
        {
            var line = new BezierSegment();
            line.Point1 = GeometricHelper.PointToChart(p1, info);
            line.Point2 = GeometricHelper.PointToChart(p2, info);
            line.Point3 = GeometricHelper.PointToChart(p3, info);
            //line.Stroke = brush;
            //line.StrokeThickness = 2;

            //plotArea.Children.Add(line);
        }

        private void DrawLegend()
        {
            //TODO: Implement Legend drawing
        }

        private void DrawSeria(IEnumerable<SeriesDoubleBasedElement> seria, ChartPlotInfo info, Brush brush)
        {
            var prevItem = seria.First();
            foreach(var item in seria.Skip(1))
            {
                DrawLine(
                    prevItem.Argument,
                    prevItem.Value,
                    item.Argument,
                    item.Value,
                    info,
                    brush);
                prevItem = item;
            }
        }

        private ChartPlotInfo CalculatePlotInfo()
        {
            ChartPlotInfo plotInfo = new ChartPlotInfo();

            double minDate = Series.FirstOrDefault().Min(element => element.Argument);
            double minValue = Series.FirstOrDefault().Min(element => element.Value);
            double maxDate = Series.FirstOrDefault().Max(element => element.Argument);
            double maxValue = Series.FirstOrDefault().Max(element => element.Value);
            foreach (var seria in Series.Skip(1))
            {
                var minX = seria.Min(el => el.Argument);
                if (minX < minDate)
                {
                    minDate = minX;
                }
                var minY = seria.Min(el => el.Value);
                if (minY < minValue)
                {
                    minValue = minY;
                }
                var maxX = seria.Max(el => el.Argument);
                if (maxX > maxDate)
                {
                    maxDate = maxX;
                }
                var maxY = seria.Max(el => el.Value);
                if (maxY > maxValue)
                {
                    maxValue = maxY;
                }
            }

            plotInfo.MaxX = maxDate;
            plotInfo.MaxY = maxValue;
            plotInfo.MinX = minDate;
            plotInfo.MinY = minValue;

            plotInfo.FooterHeight = 50;
            plotInfo.HeaderHeight = 50;
            plotInfo.LeftLegendWidth = 20;
            plotInfo.RightLegendWidth = 20;

            return plotInfo;
        }
        #endregion
        
    }
}
