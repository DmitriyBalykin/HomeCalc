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
            InitializeComponent();

            var viewModel = new AnalyticsViewModel();

            viewModel.ChartSeries = GetChartSeries();

            this.DataContext = viewModel;
        }

        private IEnumerable<IEnumerable<SeriesDoubleBasedElement>> GetChartSeries()
        {
            var seria = new List<SeriesDoubleBasedElement>();
            seria.Add(new SeriesDoubleBasedElement { Argument = 0, Value = 7 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 1, Value = 17 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 2, Value = 2 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 3, Value = 11 });
            seria.Add(new SeriesDoubleBasedElement { Argument = 4, Value = 3 });

            return new List<IEnumerable<SeriesDoubleBasedElement>> { seria };
        }
    }
}
