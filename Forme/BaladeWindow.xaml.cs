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
    /// Interaction logic for BaladeWindow.xaml
    /// </summary>
    public partial class BaladeWindow : Window
    {
        private Balade _ride;
        private readonly DateTime _originalDate;
        private readonly List<string> _groupList;

        public BaladeWindow(Balade trip, List<string> gpList)
        {
            InitializeComponent();
            _ride = trip;
            _originalDate = _ride.RideDate;
            _groupList = gpList;
        }

        public Balade TripDetails { get { return _ride; } }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            GroupComboBox.Items.Clear();
            foreach (string s in _groupList) { GroupComboBox.Items.Add(s); }
            DifficultyComboBox.Items.Clear();
            for (int d = 1; d < 4; d++) { DifficultyComboBox.Items.Add(Balade.DifficultyCaption(d)); }
            DateInputBox.DateValue = _ride.RideDate;
            DistJInputBox.Text = _ride.RideKm.ToString();
            LocnInputBox.Text = _ride.RideCaption;
            GroupComboBox.Text = _ride.RideGroup;
            DifficultyComboBox.SelectedIndex = _ride.Difficulty - 1;
            switch (_ride.Kind)
            {
                case VeloHistory.TripType.Cycle:
                    {
                        RideRadio.IsChecked = true;
                        break;
                    }
                case VeloHistory.TripType.Walk:
                    {
                        WalkRadio.IsChecked = true;
                        break;
                    }
            }
            DateInputBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt;
            if (DateInputBox.DateValue.HasValue)
            {
                dt = DateInputBox.DateValue.Value.Date;
            }
            else
            {
                MessageBox.Show("You must enter a valid date", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            string dstJ = DistJInputBox.Text;
            bool dstOkJ = double.TryParse(dstJ, out double kilomJ);
            if (!dstOkJ)
            {
                MessageBox.Show("You must enter a valid distance in Km\ne.g. 34.5", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            string locn = LocnInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(locn))
            {
                MessageBox.Show("You must enter a route or location", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            string grup = GroupComboBox.Text;
            int diffy = DifficultyComboBox.SelectedIndex + 1;

            VeloHistory.TripType k = VeloHistory.TripType.Walk;
            if (RideRadio.IsChecked.HasValue && RideRadio.IsChecked.Value)
            {
                k = VeloHistory.TripType.Cycle;
                //if (FullDayCheckBox.IsChecked.HasValue && FullDayCheckBox.IsChecked.Value) { k = History.TripType.CycleFullDay; }
            }
            _ride = new Balade(dat: dt, kmJ: kilomJ, cp: locn, gp: grup, diff: diffy, kind: k);
            DialogResult = true;
        }

        public bool DateAltered { get { return !_originalDate.Equals(_ride.RideDate); } }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (DateInputBox.DateValue.HasValue)
            {
                DateTime d = DateInputBox.DateValue.Value;
                d = d.AddDays(-1);
                DateInputBox.DateValue = d;
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (DateInputBox.DateValue.HasValue)
            {
                DateTime d = DateInputBox.DateValue.Value;
                d = d.AddDays(1);
                DateInputBox.DateValue = d;
            }
        }

    }
}
