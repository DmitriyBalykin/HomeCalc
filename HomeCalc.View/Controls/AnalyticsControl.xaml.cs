using HomeCalc.ChartsLib.Models;
using HomeCalc.Presentation.ViewModels;
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

namespace HomeCalc.View.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class AnalyticsControl : UserControl
    {
        public AnalyticsControl()
        {
            var viewModel = new AnalyticsViewModel();

            this.DataContext = viewModel;

            InitializeComponent();

            viewModel.ChartSeries = GetChartSeries();
        }

        private IEnumerable<IEnumerable<SeriesDoubleBasedElement>> GetChartSeries()
        {
            var seria = new List<SeriesDoubleBasedElement>();
            seria.Add(new SeriesDoubleBasedElement { Argument = 0, Value = 7 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 17, Value = 170 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 52, Value = 221 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 117, Value = 114 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 159, Value = 326 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 210, Value = 71 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 230, Value = 179 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 399, Value = 112 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 422, Value = 171 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 476, Value = 321 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 589, Value = 762 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 631, Value = 217 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 792, Value = 2 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 850, Value = 211 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 910, Value = 611 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 1000, Value = 32 });

            return new List<IEnumerable<SeriesDoubleBasedElement>> { seria };
        }
    }
}
