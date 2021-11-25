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
    /// Interaction logic for DayStatsWindow.xaml
    /// </summary>
    public partial class DayStatsWindow : Window
    {

        private readonly VeloHistory _history;
        bool _altered;

        public DayStatsWindow(VeloHistory history)
        {
            InitializeComponent();
            _history = history;
            _altered = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double scrX = SystemParameters.PrimaryScreenWidth;
            double scrY = SystemParameters.PrimaryScreenHeight;
            double winX = scrX * .95;
            double winY = scrY * .9;
            double Xm = (scrX - winX) / 2;
            double Ym = (scrY - winY) / 4;
            Width = winX;
            Height = winY;
            Left = Xm;
            Top = Ym;
        }

        private void DisplayOutings()
        {
            TripsListBox.Items.Clear();

            if (_history.TripList.Count < 1) { return; }

            bool ShownDemarcation = false;
            DateTime Demarcation = new DateTime(2018, 4, 10);
            DateTime startDate = _history.HistoryFirstDate;
            DateTime endDate = DateTime.Today.AddDays(1);

            while (startDate < endDate)
            {
                if ((!ShownDemarcation) && (startDate >= Demarcation))
                {
                    Rectangle demarc = new Rectangle() { Height = 8, Width = 100, Fill = Brushes.Yellow };
                    TripsListBox.Items.Add(new ListBoxItem() { Content = demarc, IsHitTestVisible = false });
                    ShownDemarcation = true;
                }
                if (_history.TripExistsForDate(startDate))
                {
                    Balade b = _history.TripOnDate(startDate);
                    Brush pinceau = Brushes.DarkSeaGreen;
                    Brush pinceau2 = Brushes.MediumSeaGreen;
                    if (b.Kind == VeloHistory.TripType.Walk) { pinceau = Brushes.DarkMagenta; pinceau2 = Brushes.Magenta; }
                    TextBlock DayBlk = new TextBlock() { Text = b.RideDate.ToString("ddd dd MMM yyyy"), Width = 108, Foreground = pinceau };

                    TextBlock DstBlkJ = new TextBlock() { Text = b.RideKmStringJbh, Width = 60, TextAlignment = TextAlignment.Right, Foreground = Brushes.MediumBlue };

                    string distanceRank = string.Empty;
                    if (b.RideKm > 0)
                    {
                        if (b.Kind == VeloHistory.TripType.Cycle)
                        {
                            int distrank = _history.DistanceRanking(b.RideKm, b.Kind);
                            distanceRank = VeloHistory.Ordinal(distrank);
                            if (distrank < 7) { RecordTop5Trip(distrank, b); }
                        }
                    }

                    TextBlock DstRnkBlkJ = new TextBlock() { Text = distanceRank, Width = 76, Foreground = Brushes.MediumBlue, TextAlignment = TextAlignment.Center };

                    TextBlock DiffBlk = new TextBlock() { Text = Balade.DifficultyCaption(b.Difficulty), Width = 108, Foreground = pinceau };
                    TextBlock EpocBlk = new TextBlock() { Text = b.RideGroup, Foreground = pinceau2 };
                    TextBlock RoutBlk = new TextBlock() { Text = b.RideCaption, Margin = new Thickness(10, 0, 0, 0), Foreground = pinceau };
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    sp.Children.Add(DayBlk);

                    sp.Children.Add(DstBlkJ);
                    sp.Children.Add(DstRnkBlkJ);

                    sp.Children.Add(DiffBlk);
                    sp.Children.Add(EpocBlk);
                    sp.Children.Add(RoutBlk);

                    ListBoxItem itm = new ListBoxItem() { Content = sp, Tag = startDate };
                    TripsListBox.Items.Add(itm);
                }
                startDate = startDate.AddDays(1);
            }
            TripsListBox.ScrollIntoView(TripsListBox.Items[TripsListBox.Items.Count - 1]);
        }
                
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Title = "Velo daily statistics";
            DisplayInformation();
        }

        private void DisplayInformation()
        {
            TripCountJTbk.Text = _history.TripCountV.ToString();
            TripCountJTbkW.Text = _history.TripCountW.ToString();

            double v = _history.TotalDistanceKmV;
            TotKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TotMlJTbk.Text = v.ToString("0.0") + " m";

            v = _history.TotalDistanceKmW;
            TotKmJTbkW.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TotMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.MaximumTripKmVelo;
            TripMaxKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripMaxMlJTbk.Text = v.ToString("0.0") + " m";

            v = _history.MaximumTripKmPied;
            TripMaxKmJTbkW.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripMaxMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.AverageTripKmVelo;
            TripAveKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripAveMlJTbk.Text = v.ToString("0.0") + " m";

            v = _history.AverageTripKmPied;
            TripAveKmJTbkW.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripAveMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.AverageDailyKmVelo;
            DayAveKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            DayAveMlJTbk.Text = v.ToString("0.0") + " m";

            v = _history.AverageDailyKmPied;
            DayAveKmJTbkW.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            DayAveMlJTbkW.Text = v.ToString("0.0") + " m";

            DisplayOutings();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Balade b = new Balade(DateTime.Today, 0, string.Empty, string.Empty, 2, VeloHistory.TripType.Cycle);
            BaladeWindow bw = new BaladeWindow(b, _history.GroupList) { Owner = this };
            bool? q = bw.ShowDialog();
            if (q.HasValue && (q.Value == false)) { return; }
            Balade s = bw.TripDetails;

            DateTime dt = s.RideDate;
            if (_history.TripExistsForDate(dt))
            {
                MessageBox.Show("Information has already been recorded for this date", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            _history.AddTrip(s);
            DisplayInformation();
            _altered = true;
        }

        private void EditDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripsListBox.SelectedItem is ListBoxItem i)
            {
                if (i.Tag is DateTime d)
                {
                    Balade ba = _history.TripOnDate(d);

                    string targetSpec = ba.Specification;
                    Balade b = new Balade(targetSpec);

                    BaladeWindow bw = new BaladeWindow(b, _history.GroupList) { Owner = this };
                    bool? q = bw.ShowDialog();
                    if (q.HasValue && (q.Value == false)) { return; }
                    Balade s = bw.TripDetails;

                    if (bw.DateAltered)
                    {
                        DateTime dt = s.RideDate;
                        if (_history.TripExistsForDate(dt))
                        {
                            MessageBox.Show("Information has already been recorded for this date", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            return;
                        }
                    }
                    _history.RemoveTripOnDate(d);
                    _history.AddTrip(s);
                    DisplayInformation();
                    _altered = true;
                }
            }
        }

        private void DeleteDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripsListBox.SelectedItem is ListBoxItem i)
            {
                if (i.Tag is DateTime d)
                {
                    string targetDate = d.ToShortDateString();
                    if (MessageBox.Show("Delete ride for " + targetDate, "Delete entry", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel) { return; }
                    _history.RemoveTripOnDate(d);
                    DisplayInformation();
                    _altered = true;
                }
            }
        }

        private void TripsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditDateButton.IsEnabled = DeleteDateButton.IsEnabled = (TripsListBox.SelectedIndex >= 0);
        }

        public bool DataAltered { get { return _altered; } }

        private void RecordTop5Trip(int rank, Balade bde)
        {
            switch (rank)
            {
                case 1:
                    {
                        Top5Date1TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist1TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route1TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 2:
                    {
                        Top5Date2TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist2TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route2TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 3:
                    {
                        Top5Date3TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist3TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route3TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 4:
                    {
                        Top5Date4TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist4TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route4TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 5:
                    {
                        Top5Date5TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist5TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route5TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 6:
                    {
                        Top5Date6TextBlock.Text = bde.RideDate.ToString("ddd dd MMM yyyy");
                        Top5Dist6TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route6TextBlock.Text = bde.RideCaption;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
