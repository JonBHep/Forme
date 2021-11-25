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
    /// Interaction logic for ListBloodPressureWindow.xaml
    /// </summary>
    public partial class ListBloodPressureWindow : Window
    {
        public ListBloodPressureWindow(PersonProfile core)
        {
            InitializeComponent();
            _coreData = core;
            _customDataItems = new System.Collections.ObjectModel.ObservableCollection<CustomData>();
            listviewData.ItemsSource = _customDataItems;
            RefreshReadings();
        }

        private PersonProfile _coreData;

        private struct CustomData
        {
            public string Date { get; set; }
            public string Diastolic { get; set; }
            public string Systolic { get; set; }
            public string Pulse { get; set; }
        }

        private System.Collections.ObjectModel.ObservableCollection<CustomData> _customDataItems;

        private void RefreshReadings()
        {
            _customDataItems.Clear();
            foreach (PersonProfile.BloodPressure p in _coreData.BloodPressureReadings)
            {
                CustomData lvd = new CustomData();
                lvd.Date = p.BprDate.ToShortDateString();
                lvd.Diastolic = p.BprDiastolic.ToString();
                lvd.Pulse = p.BprPulse.ToString();
                lvd.Systolic = p.BprSystolic.ToString();
                _customDataItems.Add(lvd);
            }
            listviewData.SelectedIndex = listviewData.Items.Count - 1;
            listviewData.ScrollIntoView(listviewData.SelectedItem);
        }

        private void ListviewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listviewData.SelectedItem == null)
            { buttonDelete.IsEnabled = false; buttonEdit.IsEnabled = false; }
            else
            { buttonDelete.IsEnabled = true; buttonEdit.IsEnabled = true; }
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            EnterBloodPressureWindow w = new EnterBloodPressureWindow();
            w.PopulateWith(DateTime.Today, 0, 0, 0);
            if (w.ShowDialog() == true)
            {
                _coreData.AddBloodPressureReading(w.datepickerDate.SelectedDate.Value, w.rvDiastolic, w.rvSystolic, w.rvPulse);
                RefreshReadings();
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.BloodPressure r = _coreData.BloodPressureReadings[y];
            EnterBloodPressureWindow w = new EnterBloodPressureWindow();
            w.PopulateWith(r.BprDate, r.BprDiastolic, r.BprSystolic, r.BprPulse);
            if (w.ShowDialog() == true)
            {
                _coreData.EditBloodPressureReading(y, w.datepickerDate.SelectedDate.Value, w.rvDiastolic, w.rvSystolic, w.rvPulse);
                RefreshReadings();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.BloodPressure r = _coreData.BloodPressureReadings[y];
            if (MessageBox.Show("Blood pressure reading\n\nDate: " + r.BprDate.ToLongDateString() + "\n" + r.BprSystolic.ToString() + "/" + r.BprDiastolic.ToString() + " and " + r.BprPulse.ToString(), "Delete reading?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }
            _coreData.BloodPressureReadings.RemoveAt(y);
            RefreshReadings();
        }

    }
}
