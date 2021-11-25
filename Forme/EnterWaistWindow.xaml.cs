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
using System.Windows.Shapes;

namespace Jbh
{
    /// <summary>
    /// Interaction logic for EnterWaistWindow.xaml
    /// </summary>
    public partial class EnterWaistWindow : Window
    {
        public EnterWaistWindow()
        {
            InitializeComponent();
        }

        private double _dataCentimetres;

        public double rvCentimetres { get { return _dataCentimetres; } }

        private bool InputDataIsOk()
        {
            if (!datepickerDate.SelectedDate.HasValue) { return false; }
            if (string.IsNullOrWhiteSpace(textboxCentimetres.Text)) { return false; }
            if (!double.TryParse(textboxCentimetres.Text, out _dataCentimetres)) { return false; }
            return true;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void textboxCentimetres_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

        private void datepickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void PopulateWith(DateTime paramDate, double paramCentimetres)
        {
            datepickerDate.SelectedDate = paramDate;
            textboxCentimetres.Text = paramCentimetres.ToString();
            buttonSave.IsEnabled = false;
        }
    }
}
