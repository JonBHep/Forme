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
    /// Interaction logic for FitnessMainWindow.xaml
    /// </summary>
    public partial class FitnessMainWindow : Window
    {
        public FitnessMainWindow()
        {
            InitializeComponent();
        }

        private PersonProfile _jData;
        private double _labelFontSize = 16;

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _jData = new PersonProfile();
            SetLabelFontSize();
            RefreshLabels();
        }

        private void SetLabelFontSize()
        {
            HeightJTextBlock.FontSize = _labelFontSize;
            J_BloodPressureValueTextBlock.FontSize = _labelFontSize;
            J_WaistValueTextBlock.FontSize = _labelFontSize;
            J_WeightValueTextBlock.FontSize = _labelFontSize;
        }

        private void RefreshLabels()
        {
            string J_FtInches = BodyStatics.HeightAsFeetAndInchesString(_jData.HeightInMetres);
            HeightJTextBlock.Text = $"{_jData.HeightInMetres} m = {J_FtInches}";

            if (_jData.BloodPressureReadings.Count > 0)
            {
                J_BloodPressureDateTextBlock.Text = _jData.LastBloodPressureReadingDate.ToLongDateString();
                BloodPressureAgoTextBlock.Text = AgoString(_jData.LastBloodPressureReadingDate);
                J_BloodPressureValueTextBlock.Text = _jData.LastBloodPressureReadingValue;
            }
            if (_jData.WaistReadings.Count > 0)
            {
                J_WaistDateTextBlock.Text = _jData.LastWaistReadingDate.ToLongDateString();
                WaistAgoTextBlock.Text = AgoString(_jData.LastWaistReadingDate);
                J_WaistValueTextBlock.Text = _jData.LastWaistReadingValue;
            }
            if (_jData.WeightReadings.Count > 0)
            {
                J_WeightDateTextBlock.Text = _jData.LastWeightReadingDate.ToLongDateString();
                WeightAgoTextBlock.Text = AgoString(_jData.LastWeightReadingDate);
                J_WeightValueTextBlock.Text = $"{ _jData.LastWeightReadingImperialString} = {_jData.LastWeightReadingKg} kg = BMI {_jData.LastWeightReadingBmi.ToString("0.0")}";
            }

        }
        private void PlotJButton_Click(object sender, RoutedEventArgs e)
        {
            FitnessPlotterWindow w = new FitnessPlotterWindow(_jData) { Owner = this };
            w.ShowDialog();
            return;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _jData.SaveAllData();
        }

        private void ButtonListWeightJ_Click(object sender, RoutedEventArgs e)
        {
            ListWeightWindow w = new ListWeightWindow(_jData) { Owner = this };
            bool? q = w.ShowDialog();
            if (q.HasValue && q.Value) { RefreshLabels(); }
        }

        private void ButtonListWaistJ_Click(object sender, RoutedEventArgs e)
        {
            ListWaistWindow w = new ListWaistWindow(_jData) { Owner = this };
            bool? q = w.ShowDialog();
            if (q.HasValue && q.Value) { RefreshLabels(); }
        }

        private void ButtonListBloodJ_Click(object sender, RoutedEventArgs e)
        {
            ListBloodPressureWindow w = new ListBloodPressureWindow(_jData) { Owner = this };
            bool? q = w.ShowDialog();
            if (q.HasValue && q.Value) { RefreshLabels(); }
        }

        private void HeightJButton_Click(object sender, RoutedEventArgs e)
        {
            Jbh.InputBox box = new Jbh.InputBox("My height", "Height in metres", _jData.HeightInMetres.ToString()) { Owner = this };
            bool? q = box.ShowDialog();
            if (q.HasValue && q.Value)
            {
                string ht = box.ResponseText;
                if (double.TryParse(ht, out double result)) { _jData.HeightInMetres = result; RefreshLabels(); } else { MessageBox.Show("An invalid value was entered", Jbh.AppManager.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk); }
            }
        }

        private string AgoString(DateTime d)
        {
            TimeSpan t = DateTime.Today - d;
            if (t.TotalDays < 1) { return "Up to date"; }
            string q = (t.TotalDays > 1) ? $"{t.TotalDays} days ago" : $"1 day ago";
            return q;
        }

    }
}
