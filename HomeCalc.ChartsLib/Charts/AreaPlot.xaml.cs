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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AreaPlot<T> : UserControl where T : SeriesElement
    {
        Canvas plotArea;
        public AreaPlot()
        {
            InitializeComponent();

            DataContext = new AreaPlotViewModel();

            plotArea = FindName("PlotArea") as Canvas;
        }
        #region Properties
        public IEnumerable<IEnumerable<T>> Series
        {
            get { return (IEnumerable<IEnumerable<T>>)GetValue(SeriesProperty); }
            set {
                SetValue(SeriesProperty, value);
                DrawSeries();
            }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(IEnumerable<IEnumerable<T>>), typeof(IEnumerable<IEnumerable<T>>), new PropertyMetadata(0));

        #endregion

        #region Internal
        private void DrawSeries()
        {
            if (plotArea == null)
            {
                return;
            }
            if (Series == null || Series.Count() == 0)
            {
                return;
            }
            foreach (var seria in Series)
            {
                DrawSeria(seria);
            }
        }

        private void DrawSeria(IEnumerable<T> seria)
        {
            throw new NotImplementedException();
        }

        #endregion
        
    }
}
