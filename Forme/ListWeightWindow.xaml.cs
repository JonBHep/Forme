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
    /// Interaction logic for ListWeightWindow.xaml
    /// </summary>
    public partial class ListWeightWindow : Window
    {
        public ListWeightWindow(PersonProfile core)
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
            public string Weight { get; set; }
            public string Pounds { get; set; }
            public string Kilograms { get; set; }
            public string Bmi { get; set; }
        }

        private System.Collections.ObjectModel.ObservableCollection<CustomData> _customDataItems;



        private void RefreshReadings()
        {
            _customDataItems.Clear();
            foreach (PersonProfile.Weight p in _coreData.WeightReadings)
            {
                CustomData lvd = new CustomData();
                lvd.Date = p.wgtDate.ToShortDateString();
                lvd.Weight = BodyStatics.WeightAsStonesAndPoundsString(p.wgtKilograms);
                lvd.Pounds = BodyStatics.WeightAsPounds(p.wgtKilograms).ToString("0.0");
                lvd.Kilograms = p.wgtKilograms.ToString("0.00");
                lvd.Bmi = BodyStatics.BmiOf(p.wgtKilograms, _coreData.HeightInMetres).ToString("0.0");
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

        private void ButtonAddClick(object sender, RoutedEventArgs e)
        {
            EnterWeightWindow w = new EnterWeightWindow(_coreData.HeightInMetres) { Owner = this };
            w.PopulateWith(DateTime.Today, 0);
            if (w.ShowDialog() == true)
            {
                _coreData.AddWeightReading(w.rvDate, w.rvKilo);
                RefreshReadings();
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.Weight r = _coreData.WeightReadings[y];
            EnterWeightWindow w = new EnterWeightWindow(_coreData.HeightInMetres) { Owner = this };
            w.PopulateWith(r.wgtDate, r.wgtKilograms);
            if (w.ShowDialog() == true)
            {
                _coreData.EditWeightReading(y, w.rvDate, w.rvKilo);
                RefreshReadings();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.Weight r = _coreData.WeightReadings[y];
            if (MessageBox.Show("Weight reading\n\nDate: " + r.wgtDate.ToLongDateString() + "\n" + r.wgtKilograms.ToString() + " Kg", "Delete reading?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }
            _coreData.WeightReadings.RemoveAt(y);
            RefreshReadings();
        }
    }
}
