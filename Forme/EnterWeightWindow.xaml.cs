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
    /// Interaction logic for EnterWeightWindow.xaml
    /// </summary>
    public partial class EnterWeightWindow : Window
    {
        private DateTime inputDate;
        private double inputKg;
        private double InpKgrmKgm;
        private double _height;

        public EnterWeightWindow(double metresTall)
        {
            InitializeComponent();
            _height = metresTall;
        }

        public DateTime rvDate { get { return inputDate; } }
        public double rvKilo { get { return inputKg; } }

        private void datepickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!datepickerDate.SelectedDate.HasValue) { return; }
            inputDate = datepickerDate.SelectedDate.Value;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            textblockResultDate.Text = string.Empty;
            textblockResultKilos.Text = string.Empty;
            textblockResultPounds.Text = string.Empty;
            textblockResultStLb.Text = string.Empty;
        }

        private void DisplayValues()
        {
            textblockResultDate.Text = inputDate.ToLongDateString();
            textblockResultKilos.Text = inputKg.ToString("0.0 Kg");
            textblockResultPounds.Text = BodyStatics.WeightAsPounds(inputKg).ToString("0.0 lb");
            textblockResultStLb.Text = BodyStatics.WeightAsStonesAndPoundsString(inputKg);
            textblockResultBMI.Text = BodyStatics.BmiOf(inputKg, _height).ToString("0.0");
        }

        public void PopulateWith(DateTime d, double k)
        {
            {
                inputDate = d;
                inputKg = k;
                datepickerDate.SelectedDate = inputDate;
                textboxInpKg.Text = inputKg.ToString();
                DisplayValues();
                buttonSave.IsEnabled = false;
            }
        }

        private bool CheckKiloInput()
        {
            bool OkInput = true;
            string KgString = textboxInpKg.Text;
            if (string.IsNullOrWhiteSpace(KgString)) { OkInput = false; }
            if (double.TryParse(KgString, out double v) == false) { OkInput = false; }
            if (OkInput)
            {
                InpKgrmKgm = v;
            }
            return OkInput;
        }

        private void textboxInpKg_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckKiloInput();
            inputKg = InpKgrmKgm;
            DisplayValues();
            buttonSave.IsEnabled = true;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        
    }
}
