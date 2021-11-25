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
    /// Interaction logic for PeriodStatsWindow.xaml
    /// </summary>
    public partial class PeriodStatsWindow : Window
    {
        private VeloHistory _source;
        private List<MonthlyStats> _months = new List<MonthlyStats>();
        private List<EpochStats> _groups = new List<EpochStats>();
        private bool _prepared = false;

        private double _canvasHeight = 0;

        private List<Point> _distanceTracePied = new List<Point>();
        private List<Point> _tripTracePied = new List<Point>();
        private List<Point> _rollingDistanceTracePied = new List<Point>();
        private List<Point> _distanceTraceVelo = new List<Point>();
        private List<Point> _tripTraceVelo = new List<Point>();
        private List<Point> _rollingDistanceTraceVelo = new List<Point>();


        public PeriodStatsWindow(VeloHistory sourceData)
        {
            InitializeComponent();
            _source = sourceData;
        }

        private void CalculateMonthRatiosAndPlots()
        {
            double distanceRatioPied;
            double tripRatioPied;
            double distanceRatioVelo;
            double tripRatioVelo;
            double xRatio;

            double maxDistPied = 0;
            double maxDistVelo = 0;
            double maxTripsPied = 0;
            double maxTripsVelo = 0;

            foreach (MonthlyStats ms in _months)
            {
                if (ms.WalkedKilometres > maxDistPied) { maxDistPied = ms.WalkedKilometres; }
                if (ms.RiddenKilometres > maxDistVelo) { maxDistVelo = ms.RiddenKilometres; }
                if (ms.TripCountPied > maxTripsPied) { maxTripsPied = ms.TripCountPied; }
                if (ms.TripCountVelo > maxTripsVelo) { maxTripsVelo = ms.TripCountVelo; }
            }

            double canvasWidth = ChartCanvas.ActualWidth;
            _canvasHeight = ChartCanvas.ActualHeight;

            distanceRatioPied = _canvasHeight / maxDistPied;
            distanceRatioVelo = _canvasHeight / maxDistVelo;
            tripRatioPied = _canvasHeight / maxTripsPied;
            tripRatioVelo = _canvasHeight / maxTripsVelo;
            xRatio = canvasWidth / _months.Count;

            double xpos = xRatio / 2;
            foreach (MonthlyStats ms in _months)
            {
                _distanceTracePied.Add(new Point(xpos, _canvasHeight - (ms.WalkedKilometres * distanceRatioPied)));
                _distanceTraceVelo.Add(new Point(xpos, _canvasHeight - (ms.RiddenKilometres * distanceRatioVelo)));
                _rollingDistanceTracePied.Add(new Point(xpos, _canvasHeight - _source.RollingYearMonthlyMeanKm(ms.YearNumber, ms.MonthNumber, VeloHistory.TripType.Walk) * distanceRatioPied));
                _rollingDistanceTraceVelo.Add(new Point(xpos, _canvasHeight - _source.RollingYearMonthlyMeanKm(ms.YearNumber, ms.MonthNumber, VeloHistory.TripType.Cycle) * distanceRatioVelo));
                _tripTracePied.Add(new Point(xpos, _canvasHeight - (ms.TripCountPied * tripRatioPied)));
                _tripTraceVelo.Add(new Point(xpos, _canvasHeight - (ms.TripCountVelo * tripRatioVelo)));
                xpos += xRatio;
            }
            _prepared = true;
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
            DetailBorder.Visibility = Visibility.Hidden;
            DetailBorder.Width = winX * 0.8;
            DetailBorder.Height = winY * 0.8;
            DetailBorder.HorizontalAlignment = HorizontalAlignment.Center;
            DetailBorder.VerticalAlignment = VerticalAlignment.Center;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            MonthsListBox.Items.Clear();
            GroupsListBox.Items.Clear();

            RecalculatePeriodStats();
            CalculateMonthRatiosAndPlots();

            foreach (MonthlyStats ms in _months)
            {
                ListBoxItem lbitm = MonthListedItem(ms, new DateTime(ms.YearNumber, ms.MonthNumber, 1).ToString("MMM yyyy"));
                MonthsListBox.Items.Add(lbitm);
            }
            RollingMonthStats rms = _source.RollingMonth;
            ListBoxItem lbi = MonthListedItem(rms, "Rolling month");
            MonthsListBox.Items.Add(lbi);

            RecalculateEpochStats();
            foreach (EpochStats gs in _groups)
            {
                ListBoxItem lbitm = GroupListedItem(gs, gs.GroupCaption);
                GroupsListBox.Items.Add(lbitm);
            }
            DrawDistance(VeloHistory.TripType.Cycle);
        }

        private ListBoxItem MonthListedItem(GroupedStats stats, string rubric)
        {
            ListBoxItem lbi = new ListBoxItem()
            {
                Tag = stats
            };
            TextBlock tbDays = new TextBlock() { Text = rubric, Margin = new Thickness(2), Width = 100 };

            TextBlock tbRidesVelo = new TextBlock() { Text = stats.TripCountVelo.ToString(), Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 60 };

            TextBlock tbRidesPied = new TextBlock() { Text = stats.TripCountPied.ToString(), Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 60 };

            double k = stats.WalkedKilometres;
            double m = VeloHistory.MilesFromKm(k);
            TextBlock tbKmPied = new TextBlock() { Text = $"{k.ToString("0.0")} km = {m.ToString("0.0")} m", Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 160 };
            if (k == 0) { tbKmPied.Visibility = tbRidesPied.Visibility = Visibility.Hidden; }

            TextBlock tbRollingKmPied = new TextBlock() { Width = 160, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.CornflowerBlue, TextAlignment = TextAlignment.Center };
            if (stats is MonthlyStats)
            {
                MonthlyStats Q = (MonthlyStats)stats;
                k = _source.RollingYearTotalKm(Q.YearNumber, Q.MonthNumber, VeloHistory.TripType.Walk);
                tbRollingKmPied.Text = $"{k.ToString("0")} km";
            }

            k = stats.RiddenKilometres;
            m = VeloHistory.MilesFromKm(k);
            TextBlock tbKmVelo = new TextBlock() { Text = $"{k.ToString("0.0")} km = {m.ToString("0.0")} m", Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 150 };
            if (k == 0) { tbKmVelo.Visibility = tbRidesVelo.Visibility = Visibility.Hidden; }

            TextBlock tbRollingKmVelo = new TextBlock() { Width = 160, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.SeaGreen, TextAlignment = TextAlignment.Center };
            if (stats is MonthlyStats)
            {
                MonthlyStats Q = (MonthlyStats)stats;
                k = _source.RollingYearMonthlyMeanKm(Q.YearNumber, Q.MonthNumber, VeloHistory.TripType.Cycle);
                tbRollingKmVelo.Text = $"{k.ToString("0")} km";
            }

            tbRidesPied.Foreground = tbKmPied.Foreground = Brushes.CornflowerBlue;
            tbRidesVelo.Foreground = tbKmVelo.Foreground = Brushes.SeaGreen;

            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };

            sp.Children.Add(tbDays);

            sp.Children.Add(tbRidesVelo);

            sp.Children.Add(tbKmVelo);

            sp.Children.Add(tbRollingKmVelo);

            sp.Children.Add(tbRidesPied);

            sp.Children.Add(tbKmPied);

            sp.Children.Add(tbRollingKmPied);

            lbi.Content = sp;

            return lbi;
        }

        private ListBoxItem GroupListedItem(EpochStats stats, string rubric)
        {
            ListBoxItem lbi = new ListBoxItem()
            {
                Tag = stats
            };
            TextBlock tbDays = new TextBlock() { Text = rubric, Margin = new Thickness(2), Width = 300 };

            TextBlock tbRidesJ = new TextBlock() { Text = (stats.TripCountPied + stats.TripCountVelo).ToString(), Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 60 };

            TextBlock tbMeanDistance = new TextBlock() { Margin = new Thickness(300, 0, 0, 0), Foreground = Brushes.Black, Text = "Distance per trip" };
            TextBlock tbMeanDistanceJ = new TextBlock() { Margin = new Thickness(12, 0, 0, 0), Foreground = Brushes.MediumBlue };

            double k = 0;
            if (stats.TripKind == VeloHistory.TripType.Cycle)
            {
                tbMeanDistanceJ.Text = $"Average {stats.PerTripKmMeanVelo} (max {stats.PerTripKmMaxVelo}, min {stats.PerTripKmMinVelo})";
                k = stats.RiddenKilometres;
            }
            else if (stats.MixedTripKinds)
            {
                MessageBox.Show("A group contains both cycle rides and walks", Jbh.AppManager.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                tbMeanDistanceJ.Text = $"Average {stats.PerTripKmMeanPied} (max {stats.PerTripKmMaxPied}, min {stats.PerTripKmMinPied})";
                k = stats.WalkedKilometres;
            }
            double m = VeloHistory.MilesFromKm(k);
            TextBlock tbKmJ = new TextBlock() { Text = $"{k.ToString("0.0")} km = {m.ToString("0.0")} m", Margin = new Thickness(2), TextAlignment = TextAlignment.Center, Width = 150 };
            tbRidesJ.Foreground = tbKmJ.Foreground = Brushes.MediumBlue;

            StackPanel spVert = new StackPanel();
            StackPanel spFirstLine = new StackPanel() { Orientation = Orientation.Horizontal };
            StackPanel spSecondLine = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 20) };

            spFirstLine.Children.Add(tbDays);

            spFirstLine.Children.Add(tbRidesJ);

            spFirstLine.Children.Add(tbKmJ);

            spSecondLine.Children.Add(tbMeanDistance);
            spSecondLine.Children.Add(tbMeanDistanceJ);

            spVert.Children.Add(spFirstLine);
            spVert.Children.Add(spSecondLine);
            lbi.Content = spVert;

            return lbi;
        }



        private void RecalculatePeriodStats()
        {
            _months = new List<MonthlyStats>();

            DateTime monthKey = _source.HistoryFirstDate;
            while (monthKey < DateTime.Today)
            {
                MonthlyStats ms = new MonthlyStats(monthKey);
                int z = 0;
                foreach (Balade b in _source.TripList) { ms.ConsiderRide(b, z); z++; }
                _months.Add(ms);
                monthKey = monthKey.AddMonths(1);
            }
        }

        private void RecalculateEpochStats()
        {
            List<string> groupcaptions = new List<string>();
            foreach (Balade b in _source.TripList)
            {
                if (!groupcaptions.Contains(b.RideGroup)) { groupcaptions.Add(b.RideGroup); }
            }

            List<Epoch> epochList = new List<Epoch>();
            foreach (string s in groupcaptions)
            {
                Epoch ep = new Epoch(s);
                epochList.Add(ep);
            }

            // get first and last date for each Epoch
            foreach (Epoch era in epochList)
            {
                foreach (Balade b in _source.TripList)
                {
                    if (era.Caption.Equals(b.RideGroup))
                    {
                        if (b.RideDate < era.FirstDate) { era.FirstDate = b.RideDate; }
                        if (b.RideDate > era.LastDate) { era.LastDate = b.RideDate; }
                    }
                }
            }
            epochList.Sort(); // based on first date in each epoch

            foreach (Epoch era in epochList)
            {
                EpochStats es = new EpochStats(era);
                int z = 0;
                foreach (Balade b in _source.TripList) { es.ConsiderRide(b, z); z++; }
                _groups.Add(es);
            }
        }

        private void Box_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox quelle = (ListBox)sender;
            ListBoxItem lbi = (ListBoxItem)quelle.SelectedItem;
            GroupedStats stats = (GroupedStats)lbi.Tag;

            DetailListBox.Items.Clear();
            foreach (int s in stats.RideIdentifiers)
            {
                Brush b;
                Balade bal = _source.TripList[s];
                b = Brushes.MediumBlue;
                TextBlock DateBlock = new TextBlock() { Text = bal.RideDate.ToShortDateString(), Width = 80 };
                string jdist = (bal.RideKm > 0) ? bal.RideKm.ToString("0.00 km") : "-";
                TextBlock DistJBlock = new TextBlock() { Text = jdist, Width = 66, TextAlignment = TextAlignment.Right, Margin = new Thickness(4, 0, 0, 0), Foreground = Brushes.MediumBlue };
                TextBlock WhatBlock = new TextBlock() { Text = bal.RideCaption, Foreground = Brushes.Gray, Margin = new Thickness(8, 0, 0, 0) };
                StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal };
                stack.Children.Add(DateBlock);
                stack.Children.Add(DistJBlock);
                stack.Children.Add(WhatBlock);
                ListBoxItem lbitm = new ListBoxItem() { Content = stack, IsHitTestVisible = false };
                DetailListBox.Items.Add(lbitm);
            }
            DetailBorder.Visibility = Visibility.Visible;
            ExplainTextBlock.Text = "Double-click the detailed list to hide it";
        }

        private void DetailListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DetailBorder.Visibility = Visibility.Hidden;
            ExplainTextBlock.Text = "Double-click an item to show the detailed list";
        }


        private void DrawDistance(VeloHistory.TripType kind)
        {
            if (!_prepared) { return; }
            ChartCanvas.Children.Clear();
            RollingTextBlock.Visibility = RollingBar.Visibility = Visibility.Visible;
            if (kind == VeloHistory.TripType.Cycle)
            {
                foreach (Point p in _distanceTraceVelo)
                {
                    Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.SeaGreen, X1 = p.X, X2 = p.X, Y1 = _canvasHeight, Y2 = p.Y };
                    ChartCanvas.Children.Add(l);
                }
                Point last = new Point(-11267, -11267);
                foreach (Point p in _rollingDistanceTraceVelo)
                {
                    if (last.X != -11267)
                    {
                        Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.Magenta, X1 = last.X, X2 = p.X, Y1 = last.Y, Y2 = p.Y };
                        ChartCanvas.Children.Add(l);
                    }
                    last = p;
                }
            }
            else
            {
                foreach (Point p in _distanceTracePied)
                {
                    Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.CornflowerBlue, X1 = p.X, X2 = p.X, Y1 = _canvasHeight, Y2 = p.Y };
                    ChartCanvas.Children.Add(l);
                }
                Point last = new Point(-11267, -11267);
                foreach (Point p in _rollingDistanceTracePied)
                {
                    if (last.X != -11267)
                    {
                        Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.Magenta, X1 = last.X, X2 = p.X, Y1 = last.Y, Y2 = p.Y };
                        ChartCanvas.Children.Add(l);
                    }
                    last = p;
                }
            }
        }

        private void DrawTripCount(VeloHistory.TripType kind)
        {
            if (!_prepared) { return; } // not fully loaded
            ChartCanvas.Children.Clear();
            RollingTextBlock.Visibility = RollingBar.Visibility = Visibility.Hidden;
            if (kind == VeloHistory.TripType.Cycle)
            {
                foreach (Point p in _tripTraceVelo)
                {
                    Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.SeaGreen, X1 = p.X, X2 = p.X, Y1 = _canvasHeight, Y2 = p.Y };
                    ChartCanvas.Children.Add(l);
                }
            }
            else
            {
                foreach (Point p in _tripTracePied)
                {
                    Line l = new Line() { StrokeThickness = 3, Stroke = Brushes.CornflowerBlue, X1 = p.X, X2 = p.X, Y1 = _canvasHeight, Y2 = p.Y };
                    ChartCanvas.Children.Add(l);
                }
            }
        }

        private void KindRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!_prepared) { return; }
            if ((VeloRadio.IsChecked.HasValue) && (VeloRadio.IsChecked.Value))
            {
                if ((DiRadio.IsChecked.HasValue) && (DiRadio.IsChecked.Value))
                {
                    DrawDistance(VeloHistory.TripType.Cycle);
                }
                else
                {
                    DrawTripCount(VeloHistory.TripType.Cycle);
                }
            }
            else
            {
                if ((DiRadio.IsChecked.HasValue) && (DiRadio.IsChecked.Value))
                {
                    DrawDistance(VeloHistory.TripType.Walk);
                }
                else
                {
                    DrawTripCount(VeloHistory.TripType.Walk);
                }
            }
        }
    }
}
