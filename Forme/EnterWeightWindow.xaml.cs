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
        private DateTime _inpDate;
        private double _InpKg;

        private double InpKgrmSPd;
        private double InpKgrmPds;
        private double InpKgrmKgm;
        private double _height;

        public EnterWeightWindow(double metresTall)
        {
            InitializeComponent();
            _height = metresTall;
        }

        public DateTime rvDate { get { return _inpDate; } }
        public double rvKilo { get { return _InpKg; } }

        private void datepickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!datepickerDate.SelectedDate.HasValue) { return; }
            _inpDate = datepickerDate.SelectedDate.Value;
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
            textblockResultDate.Text = _inpDate.ToLongDateString();
            textblockResultKilos.Text = _InpKg.ToString("0.0 Kg");
            textblockResultPounds.Text = BodyStatics.WeightAsPounds(_InpKg).ToString("0.0 lb");
            textblockResultStLb.Text = BodyStatics.WeightAsStonesAndPoundsString(_InpKg);
            textblockResultBMI.Text = BodyStatics.BmiOf(_InpKg, _height).ToString("0.0");
        }

        public void PopulateWith(DateTime d, double k)
        {
            {
                _inpDate = d;
                _InpKg = k;
                datepickerDate.SelectedDate = _inpDate;
                textboxInpKg.Text = _InpKg.ToString();
                buttonKgOkay.IsEnabled = false;
                buttonLbOkay.IsEnabled = false;
                buttonStPdOkay.IsEnabled = false;
                DisplayValues();
                buttonSave.IsEnabled = false;
            }
        }

        private bool CheckStonePoundInput()
        {
            bool OkInput = true;
            string StString = textboxInputStones.Text;
            if (string.IsNullOrWhiteSpace(StString)) { OkInput = false; }
            if (int.TryParse(StString, out int s) == false) { OkInput = false; }
            string PdString = textboxInputPounds.Text;
            if (string.IsNullOrWhiteSpace(PdString)) { OkInput = false; }
            if (int.TryParse(PdString, out int p) == false) { OkInput = false; }
            if (OkInput)
            {
                if (p > 13) { OkInput = false; }
                double v = p + (s * 14);
                v *= 0.45359237f; // kilograms
                InpKgrmSPd = v;
            }
            return OkInput;
        }

        private bool CheckPoundInput()
        {
            bool OkInput = true;
            string PdString = textboxInputPoundsOnly.Text;
            if (string.IsNullOrWhiteSpace(PdString)) { OkInput = false; }
            if (int.TryParse(PdString, out int p) == false) { OkInput = false; }
            if (OkInput)
            {
                double v = p * 0.45359237f;
                InpKgrmPds = v;
            }
            return OkInput;
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

        private void textboxInputStones_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonStPdOkay.IsEnabled = CheckStonePoundInput();
        }

        private void textboxInputPounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonStPdOkay.IsEnabled = CheckStonePoundInput();
        }

        private void textboxInputPoundsOnly_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonLbOkay.IsEnabled = CheckPoundInput();
        }

        private void textboxInpKg_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonKgOkay.IsEnabled = CheckKiloInput();
        }

        private void buttonStPdOkay_Click(object sender, RoutedEventArgs e)
        {
            _InpKg = InpKgrmSPd;
            DisplayValues();
            buttonSave.IsEnabled = true;
        }

        private void buttonLbOkay_Click(object sender, RoutedEventArgs e)
        {
            _InpKg = InpKgrmPds;
            DisplayValues();
            buttonSave.IsEnabled = true;
        }

        private void buttonKgOkay_Click(object sender, RoutedEventArgs e)
        {
            _InpKg = InpKgrmKgm;
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
