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

        private const double ARGUMENT_LABEL_VERTICAL_SHIFT = 10;
        private const double VALUE_LABEL_HORIZONTAL_SHIFT = 0;
        private const double MAX_DESIRED_X_VALUES = 12;
        private const double MAX_DESIRED_Y_VALUES = 10;
        
        public AreaPlot()
        {
            InitializeComponent();

            plotArea = FindName("PlotArea") as Canvas;
        }

        #region Properties

        private IEnumerable<IEnumerable<SeriesDateBasedElement>> series;
        public IEnumerable<IEnumerable<SeriesDateBasedElement>> Series
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
            typeof(IEnumerable<IEnumerable<SeriesDateBasedElement>>),
            typeof(AreaPlot),
            new FrameworkPropertyMetadata(
                default(IEnumerable<IEnumerable<SeriesDateBasedElement>>),
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
                Series = baseValue as IEnumerable<IEnumerable<SeriesDateBasedElement>>;
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
                series = localValue as IEnumerable<IEnumerable<SeriesDateBasedElement>>;
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
            if (Series.FirstOrDefault().Count() == 0)
            {
                return;
            }
            plotArea.Children.Clear();

            ChartPlotInfo plotInfo = CalculatePlotInfo();

            plotArea.Width = plotInfo.Width + plotInfo.LeftMarginWidth + plotInfo.RightMarginWidth;
            plotArea.Height = plotInfo.MaxHeight + plotInfo.FooterHeight + plotInfo.HeaderHeight;

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
            Canvas.SetLeft(chartBack, plotInfo.LeftMarginWidth);
            Canvas.SetTop(chartBack, plotInfo.HeaderHeight);
            chartBack.Fill = ChartBackground;
            
            plotArea.Children.Add(chartBack);
        }

        private void DrawGrid(ChartPlotInfo info)
        {
            var frameBrush = Brushes.SteelBlue;
            var gridBrush = Brushes.LightSteelBlue;

            //Grid
            for (DateTime x = info.MinX; x < info.MaxX; x += info.XStep)
            {
                if (x > info.MinX)
                {
                    DrawLine(x, 0, x, info.PhysHeight, info, gridBrush, true);
                }
                var labelModel = new LabelModel(info)
                {
                    Text = string.Format("{0:dd MMM yyyy}", x),
                    XPhys = x,
                    YPix = info.MaxHeight + ARGUMENT_LABEL_VERTICAL_SHIFT + info.HeaderHeight
                };
                
                DrawLabel(labelModel, info);
            }

            for (var y = info.MinY; y <= info.MaxY; y += info.YStep)
            {
                DrawLine(0, y, info.Width, y, info, gridBrush, true);

                var labelModel = new LabelModel(info)
                {
                    Text = string.Format("{0:F1}", y),
                    XPix = VALUE_LABEL_HORIZONTAL_SHIFT,
                    YPhys = y
                };
                DrawLabel(labelModel, info);
            }

            //Frame
            DrawLine(0, 0, 0, info.MaxY, info, frameBrush);
            DrawLine(info.Width, 0, info.Width, info.MaxY, info, frameBrush);
            DrawLine(0, info.MaxY, info.Width, info.MaxY, info, frameBrush);
            DrawLine(0, 0, info.Width, 0, info, frameBrush);
        }

        private void DrawLabel(LabelModel labelModel, ChartPlotInfo info, LabelAlign align = LabelAlign.Left)
        {
            switch (align)
            {
                case LabelAlign.Left:
                    break;
                case LabelAlign.Center:
                    labelModel.XPix -= labelModel.Text.Length * 4;
                    break;
                case LabelAlign.Right:
                    break;
                default:
                    break;
            }
            
            var textBlock = new TextBlock();
            textBlock.Text = labelModel.Text;

            RenderElement(textBlock, labelModel.XPix, labelModel.YPix);
        }

        private void DrawLine(DateTime x1, double y1, DateTime x2, double y2, ChartPlotInfo info, Brush brush, bool isDashed = false)
        {
            var line = new Line();
            line.X1 = GeometricHelper.DateToChart(x1, info);
            line.X2 = GeometricHelper.DateToChart(x2, info);
            line.Y1 = GeometricHelper.YCoordToChart(y1, info);
            line.Y2 = GeometricHelper.YCoordToChart(y2, info);
            line.Stroke = brush;
            line.StrokeThickness = 2;
            if (isDashed)
            {
                DoubleCollection dashes = new DoubleCollection(2);
                dashes.Add(5);
                dashes.Add(5);
                line.StrokeThickness = 1;
                line.StrokeDashArray = dashes;
            }

            RenderElement(line);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, ChartPlotInfo info, Brush brush, bool isDashed = false)
        {
            var line = new Line();
            line.X1 = GeometricHelper.XCoordToChart(x1, info);
            line.X2 = GeometricHelper.XCoordToChart(x2, info);
            line.Y1 = GeometricHelper.YCoordToChart(y1, info);
            line.Y2 = GeometricHelper.YCoordToChart(y2, info);
            line.Stroke = brush;
            line.StrokeThickness = 2;
            if (isDashed)
            {
                line.StrokeThickness = 1;
                line.StrokeDashOffset = 5;
            }

            RenderElement(line);
        }

        private void DrawCircle(DateTime dateTime, double p, ChartPlotInfo info, Brush brush)
        {
            var circle = new Ellipse();

            circle.ToolTip = string.Format("Дата: {0:yyyy MMM dd}, значення: {1}", dateTime, p);

            circle.Stroke = brush;
            circle.Fill = Brushes.White;

            circle.Width = 10;
            circle.Height = 10;
            circle.StrokeThickness = 2;
            var x = GeometricHelper.DateToChart(dateTime, info) - circle.Width/2;
            var y = GeometricHelper.YCoordToChart(p, info) - circle.Height/2;
            Canvas.SetTop(circle, y);
            Canvas.SetLeft(circle, x);

            RenderElement(circle);
        }

        private void DrawBezierSegment(Point p1, Point p2, Point p3, ChartPlotInfo info, Brush brush)
        {
            //var line = new BezierSegment();
            //line.Point1 = GeometricHelper.PointToChart(p1, info);
            //line.Point2 = GeometricHelper.PointToChart(p2, info);
            //line.Point3 = GeometricHelper.PointToChart(p3, info);
            //line.Stroke = brush;
            //line.StrokeThickness = 2;

            //RenderElement(line);
        }
        private void RenderElement(UIElement element, double? x = null, double? y = null)
        {
            if (x != null)
            {
                Canvas.SetLeft(element, x ?? 0);
            }
            if (y != null)
            {
                Canvas.SetTop(element, y ?? 0);
            }

            plotArea.Children.Add(element);
        }
        private void DrawLegend()
        {
            //TODO: Implement Legend drawing
        }

        private void DrawSeria(IEnumerable<SeriesDateBasedElement> seria, ChartPlotInfo info, Brush brush)
        {
            var timeOrderedSeria = seria.OrderBy(x => x.Argument).ToList();
            var prevItem = timeOrderedSeria.First();
            foreach (var item in timeOrderedSeria.Skip(1))
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
            foreach (var item in timeOrderedSeria)
            {
                DrawCircle(item.Argument, item.Value, info, brush);
            }
        }

        private ChartPlotInfo CalculatePlotInfo()
        {
            ChartPlotInfo plotInfo = new ChartPlotInfo();

            DateTime minDate = Series.FirstOrDefault().Min(element => element.Argument);
            DateTime maxDate = Series.FirstOrDefault().Max(element => element.Argument);
            double maxValue = Series.FirstOrDefault().Max(element => element.Value);
            foreach (var seria in Series.Skip(1))
            {
                var minX = seria.Min(el => el.Argument);
                if (minX < minDate)
                {
                    minDate = minX;
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
            plotInfo.MinY = 0;

            plotInfo.XStep = new TimeSpan((int)Math.Ceiling(plotInfo.DaysWidth / MAX_DESIRED_X_VALUES), 0, 0, 0);

            var valuesPower = Math.Log10(plotInfo.MaxY * 10);
            var valuesPowerRounded = Math.Ceiling(valuesPower);
            plotInfo.YStep = 100 / Math.Pow(10, valuesPowerRounded) / 2;
            plotInfo.MaxY = Math.Ceiling(plotInfo.MaxY / plotInfo.YStep) * plotInfo.YStep;


            plotInfo.FooterHeight = 50;
            plotInfo.HeaderHeight = 50;
            plotInfo.LeftMarginWidth = 40;
            plotInfo.RightMarginWidth = 20;

            return plotInfo;
        }

        #endregion
        
    }

    enum LabelAlign
    {
        Left = 0,
        Center = 1,
        Right = 2
    }
}
