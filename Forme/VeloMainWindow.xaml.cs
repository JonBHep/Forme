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
    /// Interaction logic for VeloMainWindow.xaml
    /// </summary>
    public partial class VeloMainWindow : Window
    {
        public VeloMainWindow()
        {
            InitializeComponent();
        }

        private VeloHistory _history;
        private const string _distFormat = "0.0 km";
        private readonly List<TextBlock> _signs = new List<TextBlock>();
        private int _plot = 0;
        private double _canvasWidth;
        private readonly double _xInterval = 4;
        private DateTime _minDate;
        private DateTime _maxDate;

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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _history = new VeloHistory("Data.velo");
            RefreshDisplay();
            DisplayVersionInfo();
        }

        private void RefreshDisplay()
        {
            DisplayStatistics();
            DisplayDateFramework();
            DisplayRidesGraph();
        }

        private void DisplayVersionInfo()
        {
            string Vs = AppManager.VersionString();
            string vers = $"Ver {Vs}";
            VersionTextBlock.Text = vers;

            TodayTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(42, 133, 83));
            TodayTextBlock.Text = "Today is " + DateTime.Today.ToString("ddd dd MMM yyyy");
        }

        private void DisplayStatistics()
        {
            int tt = _history.TripCountCycling;
            int td = _history.TotalDaysCycling;
            CountTbkVelo.Text = $"{tt} rides in {td} days";
            double dd = td / (double)tt;
            CountTbkVelo.ToolTip = $"A ride every {dd:0.0} days";
            if (_history.TripCountCycling > 0)
            {
                MaximumTbkVelo.Text = $"Longest ride {_history.MaximumTripKmVelo.ToString(_distFormat)}";
                MeanTbkVelo.Text = $"Mean ride {_history.AverageTripKmVelo.ToString(_distFormat)}";
                KmPerDayTbkVelo.Text = $"Mean {_history.AverageDailyKmVelo.ToString(_distFormat)} / day";

                string mean = _history.Average4WeeklyKmVelo.ToString(_distFormat);
                string roll = (_history.RollingMonth.TripCountVelo == 0) ? "no rides" : (_history.RollingMonth.TripCountVelo == 1) ? $"One ride of {_history.RollingMonth.RiddenKilometres.ToString(_distFormat)}." : $"{_history.RollingMonth.RiddenKilometres.ToString(_distFormat)} ({_history.RollingMonth.TripCountVelo} rides)";
                KmPerMonthTbkVelo.Text = $"Month: mean {mean}, latest {roll}";
                double mix = _history.RollingMonth.RiddenKilometres / _history.Average4WeeklyKmVelo; // compares actual with average
                                                                                                     // convert to a double between 0 and 1 with 0.5 = current matches average
                mix /= 2;
                mix = Math.Min(1, mix); // in case the actual was more than twice the average
                PerMonthFlagVelo.Fill = new SolidColorBrush(VeloHistory.ColourMix(Colors.LightCoral, Colors.DarkSeaGreen, mix));
                PerMonthFlagVelo.ToolTip = mix.ToString();
            }
            tt = _history.TripCountPied;
            td = _history.TotalDaysPied;
            CountTbkPied.Text = $"{tt} walks in {td} days";
            dd = td / (double)tt;
            CountTbkPied.ToolTip = $"A walk every {dd:0.0} days";
            if (_history.TripCountPied > 0)
            {
                MaximumTbkPied.Text = $"Longest walk {_history.MaximumTripKmPied.ToString(_distFormat)}";
                MeanTbkPied.Text = $"Mean walk {_history.AverageTripKmPied.ToString(_distFormat)}";
                KmPerDayTbkPied.Text = $"Mean {_history.AverageDailyKmPied.ToString(_distFormat)} / day";

                string mean = _history.Average4WeeklyKmPied.ToString(_distFormat);
                string roll = (_history.RollingMonth.TripCountPied == 0) ? "no walks" : (_history.RollingMonth.TripCountPied == 1) ? $"One walk of {_history.RollingMonth.WalkedKilometres.ToString(_distFormat)}." : $"{_history.RollingMonth.WalkedKilometres.ToString(_distFormat)} ({_history.RollingMonth.TripCountPied} walks)";
                KmPerMonthTbkPied.Text = $"Month: mean {mean}, latest {roll}";
                double mix = _history.RollingMonth.WalkedKilometres / _history.Average4WeeklyKmPied; // compares actual with average
                                                                                                     // convert to a double between 0 and 1 with 0.5 = current matches average
                mix /= 2;
                mix = Math.Min(1, mix); // in case the actual was more than twice the average
                PerMonthFlagPied.Fill = new SolidColorBrush(VeloHistory.ColourMix(Colors.LightCoral, Colors.DarkSeaGreen, mix));
                PerMonthFlagPied.ToolTip = mix.ToString();
            }
        }

        private double XPosition(DateTime d)
        {
            TimeSpan t = d - _minDate;
            return t.TotalDays * _xInterval;
        }
        private void DisplayRidesGraph()
        {
            double canvasHeight = ChartCanvas.ActualHeight;

            List<Balade> riderBallades = _history.TripList;

            double maxDist = double.MinValue;

            foreach (Balade b in riderBallades)
            {
                maxDist = Math.Max(maxDist, b.RideKm);
            }

            // add rides
            foreach (Balade b in riderBallades)
            {
                double xpos = XPosition(b.RideDate);
                double ypos = canvasHeight - (b.RideKm * canvasHeight / maxDist);
                Brush scb = VeloHistory.BrushEasy;
                if (b.Difficulty == 2) { scb = VeloHistory.BrushIntermediate; }
                if (b.Difficulty == 3) { scb = VeloHistory.BrushHard; }
                if (b.Kind == VeloHistory.TripType.Walk) { scb = VeloHistory.BrushWalk; }
                Line l = new Line() { X1 = xpos, X2 = xpos, Y1 = canvasHeight, Y2 = ypos, StrokeThickness = _xInterval - 1, Stroke = scb };
                ChartCanvas.Children.Add(l);
            }

            // draw a horizontal line at each 10K
            for (double km = 0; km < 1200; km += 10)
            {
                double ypos = canvasHeight - (km * canvasHeight / maxDist);
                if ((ypos > 0) && (ypos < canvasHeight))
                {
                    Line l = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = ypos, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.Ivory, Opacity = 0.3 };
                    ChartCanvas.Children.Add(l);

                    TextBlock tl = new TextBlock() { Text = $" {km} km ", Foreground = Brushes.Silver, Background = ChartCanvas.Background };
                    _signs.Add(tl);
                    Canvas.SetTop(tl, ypos - 8);
                    Canvas.SetLeft(tl, 100);
                    ChartCanvas.Children.Add(tl);
                }
            }

            //// draw a horizontal line at the average distance value for cycling
            double yavg;
            yavg = canvasHeight - (_history.AverageTripKmVelo * canvasHeight / maxDist);
            Line m = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = yavg, Y2 = yavg, StrokeThickness = 1.5, Stroke = Brushes.LightCoral, Opacity = 0.5 };
            ChartCanvas.Children.Add(m);

            TextBlock tml = new TextBlock() { Text = " Mean ride distance ", Foreground = Brushes.LightCoral, Background = ChartCanvas.Background, Margin = new Thickness(100, 0, 0, 0) };
            Canvas.SetTop(tml, yavg - 8);
            Canvas.SetLeft(tml, 100);
            ChartCanvas.Children.Add(tml);
            _signs.Add(tml);

            //// draw a horizontal line at the average distance value for walking
            yavg = canvasHeight - (_history.AverageTripKmPied * canvasHeight / maxDist);
            m = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = yavg, Y2 = yavg, StrokeThickness = 0.75, Stroke = Brushes.LightCoral, Opacity = 0.5 };
            ChartCanvas.Children.Add(m);

            tml = new TextBlock() { Text = " Mean walk distance ", FontWeight = FontWeights.Light, Foreground = Brushes.LightCoral, Background = ChartCanvas.Background, Margin = new Thickness(100, 0, 0, 0) };
            Canvas.SetTop(tml, yavg - 8);
            Canvas.SetLeft(tml, 100);
            ChartCanvas.Children.Add(tml);
            _signs.Add(tml);

            ChartScrollViewer.ScrollToRightEnd();
        }

        private void DisplayCumulativeGraph()
        {
            List<Tuple<double, DateTime>> cumulus = new List<Tuple<double, DateTime>>();
            double cumv = 0;

            double canvasHeight = ChartCanvas.ActualHeight;

            List<Balade> riderBallades = _history.TripList;

            foreach (Balade b in riderBallades)
            {
                cumv += b.RideKm;
                Tuple<double, DateTime> cv = new Tuple<double, DateTime>(cumv, b.RideDate);
                cumulus.Add(cv);
            }

            // add line for cumulative distance
            Point last = new Point(-1, -1);
            foreach (Tuple<double, DateTime> f in cumulus)
            {
                double xpos = XPosition(f.Item2);
                double ypos = canvasHeight - (f.Item1 * canvasHeight / cumv);
                if (last.X >= 0)
                {
                    Line A = new Line() { X1 = last.X, X2 = xpos, Y1 = last.Y, Y2 = last.Y, StrokeThickness = 1.5, Stroke = Brushes.DarkSeaGreen };
                    ChartCanvas.Children.Add(A);
                    Line B = new Line() { X1 = xpos, X2 = xpos, Y1 = last.Y, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.DimGray };
                    ChartCanvas.Children.Add(B);
                }
                last = new Point(xpos, ypos);
            }

            // draw a horizontal line at each 500K
            _signs.Clear();
            for (double km = 0; km < 100000; km += 500)
            {
                double ypos = canvasHeight - (km * canvasHeight / cumv);
                if ((ypos > 0) && (ypos < canvasHeight))
                {
                    Line l = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = ypos, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.Black, Opacity = 0.3 };
                    ChartCanvas.Children.Add(l);

                    TextBlock tl = new TextBlock() { Text = $" {km} km ", Foreground = Brushes.Brown, Background = Brushes.Ivory };
                    _signs.Add(tl);
                    Canvas.SetTop(tl, ypos - 8);
                    Canvas.SetLeft(tl, 100);
                    ChartCanvas.Children.Add(tl);
                }
            }

        }

        private void DisplayDateFramework()
        {
            double canvasHeight = ChartCanvas.ActualHeight;

            List<Balade> riderBallades = _history.TripList;
            ChartCanvas.Children.Clear();
            _minDate = riderBallades[0].RideDate.Date;
            //_maxDate = riderBallades[riderBallades.Count - 1].RideDate.Date;
            _maxDate = DateTime.Today;
            TimeSpan dateInterval = _maxDate - _minDate;
            double dayspan = dateInterval.TotalDays + 1;
            ChartCanvas.Width = _canvasWidth = _xInterval * dayspan;
            DateTime p = _minDate;
            do
            {
                if (p.Day == 1)
                {
                    double xpos = XPosition(p);
                    Line monthstarter = new Line() { X1 = xpos, X2 = xpos, Y1 = 0, Y2 = canvasHeight, Stroke = Brushes.Ivory, StrokeThickness = 0.8, StrokeDashArray = new DoubleCollection { 6, 3 } };
                    ChartCanvas.Children.Add(monthstarter);
                    TextBlock monthstarterblock = new TextBlock() { Text = p.ToString("MMM yyyy"), Foreground = Brushes.Ivory };
                    Canvas.SetLeft(monthstarterblock, xpos + 4);
                    Canvas.SetTop(monthstarterblock, 8);
                    ChartCanvas.Children.Add(monthstarterblock);
                }
                p = p.AddDays(1);
            } while (p < _maxDate);

            ChartScrollViewer.ScrollToRightEnd();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PaintingWindow ww = new PaintingWindow() { Owner = this };
            ww.ShowDialog();
        }

        private void DailyButton_Click(object sender, RoutedEventArgs e)
        {
            DayStatsWindow w = new DayStatsWindow(_history) { Owner = this };
            w.ShowDialog();
            if (w.DataAltered)
            {
                RefreshDisplay();
            }
        }

        private void RatiosButton_Click(object sender, RoutedEventArgs e)
        {
            RatiosWindow w = new RatiosWindow() { Owner = this };
            w.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _history.SaveData();
        }

        private void WeeklyButton_Click(object sender, RoutedEventArgs e)
        {
            PeriodStatsWindow w = new PeriodStatsWindow(_history)
            {
                Owner = this
            };
            w.ShowDialog();
        }

        private void ChartScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            foreach (TextBlock bloc in _signs)
            {
                Canvas.SetLeft(bloc, 100 + e.HorizontalOffset);
            }
        }

        private void PlotButton_Click(object sender, RoutedEventArgs e)
        {
            if (_plot == 0)
            {
                _plot = 1;
                DisplayDateFramework();
                DisplayCumulativeGraph();
                PlotTextBlock.Text = "Cumulative distance";
                PlotButton.Content = "Daily rides";
                PlotButton.ToolTip = "Display individual ride distances";
            }
            else
            {
                _plot = 0;
                DisplayDateFramework();
                DisplayRidesGraph();
                PlotTextBlock.Text = "Daily rides";
                PlotButton.Content = "Cumulative distance";
                PlotButton.ToolTip = "Display cumulative distance ridden";
            }
        }

        private void DistanceButton_Click(object sender, RoutedEventArgs e)
        {
            DistanceWindow w = new DistanceWindow(_history) { Owner = this };
            w.ShowDialog();
        }

    }

}
