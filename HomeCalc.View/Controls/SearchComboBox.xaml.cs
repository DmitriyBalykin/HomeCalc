using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for SearchComboBox.xaml
    /// </summary>
    public partial class SearchComboBox : ComboBox
    {
        private bool selectionChangeLocked = false;
        public SearchComboBox()
        {
            InitializeComponent();

            this.PreviewKeyDown += control_KeyDown;
        }

        public override void OnApplyTemplate()
        {
            var textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            
            if (textBox != null)
            {
                textBox.SelectionChanged += textBox_SelectionChanged;
            }

            base.OnApplyTemplate();
        }

        void control_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Pressed key: {0}", e.Key);
            Console.WriteLine("IsDropDownOpen: {0}", IsDropDownOpen);
            if (IsDropDownOpen && e.Key == Key.Down)
            {
                Console.WriteLine("Selecting idex 0");
                SelectedIndex = 0;
            }
        }

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (selectionChangeLocked)
            {
                return;
            }
            TextBox textBox = (TextBox)sender;
            
            if (IsDropDownOpen && textBox.SelectionLength > 0)
            {
                selectionChangeLocked = true;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
                selectionChangeLocked = false;
            }
        }

    }
}
