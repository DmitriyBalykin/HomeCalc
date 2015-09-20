using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SearchCombo.xaml
    /// </summary>
    public partial class SearchCombo : UserControl
    {
        public SearchCombo()
        {
            InitializeComponent();
        }

        public string Text { get; set; }
        public ObservableCollection<object> Items { get; set; }
    }
}
