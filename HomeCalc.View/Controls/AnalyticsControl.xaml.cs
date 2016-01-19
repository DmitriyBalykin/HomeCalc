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

            //viewModel.ChartSeries = GetChartSeries();
        }

        private IEnumerable<IEnumerable<SeriesDateBasedElement>> GetChartSeries()
        {
            var seria = new List<SeriesDateBasedElement>();

            var now = DateTime.Now;

            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-36), Value = 7 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-34), Value = 170 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-33), Value = 221 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-31), Value = 114 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-30), Value = 326 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-29), Value = 71 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-27), Value = 179 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-25), Value = 112 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-22), Value = 171 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-20), Value = 321 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-17), Value = 762 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-15), Value = 217 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-12), Value = 2 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-10), Value = 211 });
            seria.Add(new SeriesDateBasedElement { Argument = now.AddDays(-5), Value = 611 });
            seria.Add(new SeriesDateBasedElement { Argument = now, Value = 32 });

            return new List<IEnumerable<SeriesDateBasedElement>> { seria };
        }
    }
}
