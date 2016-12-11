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
    public partial class AreaPlot : UserControl
    {
        private const double ARGUMENT_LABEL_VERTICAL_SHIFT = 10;
        private const double VALUE_LABEL_HORIZONTAL_SHIFT = 0;
        private const double MAX_DESIRED_X_VALUES = 12;
        private const double MAX_DESIRED_Y_VALUES = 10;
        
        public AreaPlot()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register("Series", typeof(IEnumerable<IEnumerable<SeriesDateBasedElement>>), typeof(AreaPlot), new FrameworkPropertyMetadata(OnSeriesPropertyChanged));

        public static readonly DependencyProperty ChartBackgroundProperty = DependencyProperty.Register("ChartBackground", typeof(Brush), typeof(AreaPlot));

        public static IEnumerable<IEnumerable<SeriesDateBasedElement>> GetSeriesProperty(DependencyObject source)
        {
            return source.GetValue(SeriesProperty) as IEnumerable<IEnumerable<SeriesDateBasedElement>>;
        }

        public static void SetSeriesProperty(DependencyObject source, IEnumerable<IEnumerable<SeriesDateBasedElement>> value)
        {
            source.SetValue(SeriesProperty, value);
        }

        public static Brush GetChartBackgroundProperty(DependencyObject source)
        {
            return source.GetValue(SeriesProperty) as Brush;
        }

        public static void SetChartBackgroundProperty(DependencyObject source, Brush value)
        {
            source.SetValue(SeriesProperty, value);
        }

        private static void OnSeriesPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var series = GetSeriesProperty(source);

            var backBrush = GetChartBackgroundProperty(source);

            var plotArea = (source as FrameworkElement).FindName("PlotArea") as Canvas;

            if (plotArea == null)
            {
                return;
            }
            if (series == null || series.Count() == 0)
            {
                return;
            }
            if (series.FirstOrDefault().Count() == 0)
            {
                return;
            }

            DrawChart(plotArea, series, backBrush);
        }

        #endregion

        #region Internal
        private static void DrawChart(Canvas plotArea, IEnumerable<IEnumerable<SeriesDateBasedElement>> series, Brush backgroundColor)
        {
            plotArea.Children.Clear();

            ChartPlotInfo plotInfo = CalculatePlotInfo(series);

            plotArea.Width = plotInfo.Width + plotInfo.LeftMarginWidth + plotInfo.RightMarginWidth;
            plotArea.Height = plotInfo.MaxHeight + plotInfo.FooterHeight + plotInfo.HeaderHeight;

            DrawLegend();
            DrawBackground(plotArea, plotInfo, backgroundColor);
            DrawGrid(plotArea, plotInfo);
            foreach (var seria in series)
            {
                DrawSeria(plotArea, seria, plotInfo, BrushGenerator.GetBrush());
            }
        }

        private static void DrawBackground(Canvas plotArea, ChartPlotInfo plotInfo, Brush background)
        {
            var chartBack = new Rectangle();
            chartBack.Width = plotInfo.MaxWidth;
            chartBack.Height = plotInfo.MaxHeight;
            Canvas.SetLeft(chartBack, plotInfo.LeftMarginWidth);
            Canvas.SetTop(chartBack, plotInfo.HeaderHeight);
            chartBack.Fill = background;
            
            plotArea.Children.Add(chartBack);
        }

        private static void DrawGrid(Canvas plotArea, ChartPlotInfo info)
        {
            var frameBrush = Brushes.SteelBlue;
            var gridBrush = Brushes.LightSteelBlue;

            //Grid
            for (DateTime x = info.MinX; x < info.MaxX; x += info.XStep)
            {
                if (x > info.MinX)
                {
                    DrawLine(plotArea, x, 0, x, info.PhysHeight, info, gridBrush, true);
                }
                var labelModel = new LabelModel(info)
                {
                    Text = string.Format("{0:dd MMM yyyy}", x),
                    XPhys = x,
                    YPix = info.MaxHeight + ARGUMENT_LABEL_VERTICAL_SHIFT + info.HeaderHeight
                };

                DrawLabel(plotArea, labelModel, info);
            }

            for (var y = info.MinY; y <= info.MaxY; y += info.YStep)
            {
                DrawLine(plotArea, 0, y, info.Width, y, info, gridBrush, true);

                var labelModel = new LabelModel(info)
                {
                    Text = string.Format("{0:F1}", y),
                    XPix = VALUE_LABEL_HORIZONTAL_SHIFT,
                    YPhys = y
                };
                DrawLabel(plotArea, labelModel, info);
            }

            //Frame
            //left
            DrawLine(plotArea, 0, 0, 0, info.MaxY, info, frameBrush);
            //right
            DrawLine(plotArea, info.Width, 0, info.Width, info.MaxY, info, frameBrush);
            //bottom
            DrawLine(plotArea, 0, info.MaxY, info.Width, info.MaxY, info, frameBrush);
            //top
            DrawLine(plotArea, 0, 0, info.Width, 0, info, frameBrush);
        }

        private static void DrawLabel(Canvas plotArea, LabelModel labelModel, ChartPlotInfo info, LabelAlign align = LabelAlign.Left)
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

            RenderElement(plotArea, textBlock, labelModel.XPix, labelModel.YPix);
        }

        private static void DrawLine(Canvas plotArea, DateTime x1, double y1, DateTime x2, double y2, ChartPlotInfo info, Brush brush, bool isDashed = false)
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

            RenderElement(plotArea, line);
        }

        private static void DrawLine(Canvas plotArea, double x1, double y1, double x2, double y2, ChartPlotInfo info, Brush brush, bool isDashed = false)
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

            RenderElement(plotArea, line);
        }

        private static void DrawCircle(Canvas plotArea, DateTime dateTime, double p, ChartPlotInfo info, Brush brush)
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

            RenderElement(plotArea, circle);
        }

        private static void DrawBezierSegment(Point p1, Point p2, Point p3, ChartPlotInfo info, Brush brush)
        {
            //var line = new BezierSegment();
            //line.Point1 = GeometricHelper.PointToChart(p1, info);
            //line.Point2 = GeometricHelper.PointToChart(p2, info);
            //line.Point3 = GeometricHelper.PointToChart(p3, info);
            //line.Stroke = brush;
            //line.StrokeThickness = 2;

            //RenderElement(line);
        }
        private static void RenderElement(Canvas plotArea, UIElement element, double? x = null, double? y = null)
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
        private static void DrawLegend()
        {
            //TODO: Implement Legend drawing
        }

        private static void DrawSeria(Canvas plotArea, IEnumerable<SeriesDateBasedElement> seria, ChartPlotInfo info, Brush brush)
        {
            var timeOrderedSeria = seria.OrderBy(x => x.Argument).ToList();
            var prevItem = timeOrderedSeria.First();
            foreach (var item in timeOrderedSeria.Skip(1))
            {
                DrawLine(
                    plotArea,
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
                DrawCircle(plotArea, item.Argument, item.Value, info, brush);
            }
        }

        private static ChartPlotInfo CalculatePlotInfo(IEnumerable<IEnumerable<SeriesDateBasedElement>> series)
        {
            ChartPlotInfo plotInfo = new ChartPlotInfo();

            DateTime minDate = series.FirstOrDefault().Min(element => element.Argument);
            DateTime maxDate = series.FirstOrDefault().Max(element => element.Argument);
            double maxValue = series.FirstOrDefault().Max(element => element.Value);
            foreach (var seria in series.Skip(1))
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
