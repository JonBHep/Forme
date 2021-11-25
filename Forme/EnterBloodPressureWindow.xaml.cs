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
    /// Interaction logic for EnterBloodPressureWindow.xaml
    /// </summary>
    public partial class EnterBloodPressureWindow : Window
    {
        private int _dataDia;
        private int _dataPulse;
        private int _dataSys;

        public int rvDiastolic { get { return _dataDia; } }
        public int rvPulse { get { return _dataPulse; } }
        public int rvSystolic { get { return _dataSys; } }

        public EnterBloodPressureWindow()
        {
            InitializeComponent();
        }

        private void textboxSystolic_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

        private bool InputDataIsOk()
        {
            if (!datepickerDate.SelectedDate.HasValue) { return false; }
            if (string.IsNullOrWhiteSpace(textboxDiastolic.Text)) { return false; }
            if (string.IsNullOrWhiteSpace(textboxPulse.Text)) { return false; }
            if (string.IsNullOrWhiteSpace(textboxSystolic.Text)) { return false; }

            if (!int.TryParse(textboxDiastolic.Text, out _dataDia)) { return false; }
            if (!int.TryParse(textboxPulse.Text, out _dataPulse)) { return false; }
            if (!int.TryParse(textboxSystolic.Text, out _dataSys)) { return false; }

            return true;
        }

        private void textboxDiastolic_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

        private void textboxPulse_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

        public void PopulateWith(DateTime bpDate, int bpDiastolic, int bpPulse, int bpSystolic)
        {
            datepickerDate.SelectedDate = bpDate;
            textboxDiastolic.Text = bpDiastolic.ToString();
            textboxPulse.Text = bpPulse.ToString();
            textboxSystolic.Text = bpSystolic.ToString();
            buttonSave.IsEnabled = false;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void datepickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonSave.IsEnabled = InputDataIsOk();
        }

    }
}
