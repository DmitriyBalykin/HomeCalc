using HomeCalc.ChartsLib.Models;
using HomeCalc.ChartsLib.ViewModels;
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
        AreaPlotViewModel dataContext;
        public AreaPlot()
        {
            InitializeComponent();
            
            dataContext = new AreaPlotViewModel();
            //dataContext.SeriesUpdated += (sender, e) => DrawChart();
            DataContext = dataContext;

            plotArea = FindName("PlotArea") as Canvas;
        }

        #region Properties
        public IEnumerable<IEnumerable<SeriesDoubleBasedElement>> Series
        {
            get { return (IEnumerable<IEnumerable<SeriesDoubleBasedElement>>)GetValue(SeriesProperty); }
            set
            {
                SetValue(SeriesProperty, value);
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
            return baseValue;
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
            var newSeriesValue = ReadLocalValue(SeriesProperty);
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

            dataContext.FooterHeight = 50;
            dataContext.HeaderHeight = 50;
            dataContext.LeftLegendWidth = 20;
            dataContext.RightLegendWidth = 20;

            DrawLegend();
            DrawGrid();
            foreach (var seria in Series)
            {
                DrawSeria(seria);
            }
        }

        private void DrawGrid()
        {
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

            plotArea.Width = maxDate;
            plotArea.Height = maxValue;

            DrawLine(0, 0, 0, maxValue, Brushes.Red);
            DrawLine(maxDate, 0, maxDate, maxValue, Brushes.Red);
            DrawLine(0, maxValue, maxDate, maxValue, Brushes.Red);
            DrawLine(0, 0, maxDate, 0, Brushes.Red);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush brush)
        {
            var line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.Stroke = brush;
            line.StrokeThickness = 2;

            plotArea.Children.Add(line);
        }

        private void DrawLegend()
        {
            //TODO: Implement Legend drawing
        }

        private void DrawSeria(IEnumerable<SeriesDoubleBasedElement> seria)
        {
            //TODO: Implement Seria drawing
        }

        #endregion
        
    }
}
