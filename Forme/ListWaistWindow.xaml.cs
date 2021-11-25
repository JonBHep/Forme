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
    /// Interaction logic for ListWaistWindow.xaml
    /// </summary>
    public partial class ListWaistWindow : Window
    {
        public ListWaistWindow(PersonProfile core)
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
            public string Centimetres { get; set; }
            public string Inches { get; set; }
        }

        private System.Collections.ObjectModel.ObservableCollection<CustomData> _customDataItems;

        private void RefreshReadings()
        {
            _customDataItems.Clear();
            foreach (PersonProfile.Waist p in _coreData.WaistReadings)
            {
                CustomData lvd = new CustomData();
                lvd.Date = p.wstDate.ToShortDateString();
                lvd.Centimetres = p.wstCentimetres.ToString();
                lvd.Inches = (p.wstCentimetres / 2.54).ToString("#.#");
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
            EnterWaistWindow w = new EnterWaistWindow();
            w.PopulateWith(DateTime.Today, 0);
            if (w.ShowDialog() == true)
            {
                _coreData.AddWaistReading(w.datepickerDate.SelectedDate.Value, w.rvCentimetres);
                RefreshReadings();
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.Waist r = _coreData.WaistReadings[y];
            EnterWaistWindow w = new EnterWaistWindow();
            w.PopulateWith(r.wstDate, r.wstCentimetres);
            if (w.ShowDialog() == true)
            {
                _coreData.EditWaistReading(y, w.datepickerDate.SelectedDate.Value, w.rvCentimetres);
                RefreshReadings();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int y = listviewData.SelectedIndex;
            PersonProfile.Waist r = _coreData.WaistReadings[y];
            if (MessageBox.Show("Waist measurement\n\nDate: " + r.wstDate.ToLongDateString() + "\n" + r.wstCentimetres.ToString() + " cm", "Delete measurement?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) { return; }
            _coreData.WaistReadings.RemoveAt(y);
            RefreshReadings();
        }


    }
}
