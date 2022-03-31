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
    /// Interaction logic for GymWindow.xaml
    /// </summary>
    public partial class GymWindow : Window
    {
        private GymHistory _history;
        private readonly DateTime _antedate = new DateTime(2019, 12, 15);
        private readonly double daywidth = 5;
        public GymWindow()
        {
            InitializeComponent();
        }

        private Brush ActivityBrush(GymHistory.GymType g)
        {
            switch (g)
            {
                case GymHistory.GymType.AquaAerobics: { return Brushes.RoyalBlue; }
                case GymHistory.GymType.GymTraining: { return Brushes.SaddleBrown; }
                case GymHistory.GymType.Other: { return Brushes.SeaGreen; }
                default: { return Brushes.Black; }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double scrX = SystemParameters.PrimaryScreenWidth;
            double scrY = SystemParameters.PrimaryScreenHeight;
            double winX = scrX * .98;
            double winY = scrY * .9;
            double Xm = (scrX - winX) / 2;
            double Ym = (scrY - winY) / 4;
            Width = winX;
            Height = winY;
            Left = Xm;
            Top = Ym;

            _history = new GymHistory("Data.gym");
            RefreshList();
            AddButton.IsEnabled = true;
           AddStackPanel.Visibility = Visibility.Hidden;
            CloseButton.IsEnabled = true;
        }

        private void RefreshList()
        {
            DeleteButton.IsEnabled = false;
            _history.GymList.Sort();
            int b = 0;
            foreach (GymVisit g in _history.GymList)
            {
                g.Index = b; b++;
            }
            // Display list of visits
            GymVisitsListBox.Items.Clear();
            YesterWeekListBox.Items.Clear();
            TodayWeekListBox.Items.Clear();
            DateTime Yesterday = DateTime.Today.AddDays(-1);
            DateTime WeekAgoToday = DateTime.Today.AddDays(-6);
            DateTime WeekAgoYesterday = DateTime.Today.AddDays(-7);

            List<GymVisit> SevenDaysToYesterday = new List<GymVisit>();
            List<GymVisit> SevenDaysToToday = new List<GymVisit>();

            // List all dates and create lists for week to yesterday and week to today
            foreach (GymVisit g in _history.GymList)
            {
                GymVisitsListBox.Items.Add(ListEntry(g));

                if (g.When >= WeekAgoToday)
                {
                    SevenDaysToToday.Add(g);
                }
                if ((g.When >= WeekAgoYesterday) && (g.When <= Yesterday))
                {
                    SevenDaysToYesterday.Add(g);
                }
            }

            // List the most recent week

            YesterWeekTextBlock.Text = "Week to yesterday";
            YesterWeekTextBlock.Inlines.Add(Ticky(SevenDaysToYesterday.Count));
            DateTime daymark = WeekAgoYesterday;
            while (daymark.Date <= Yesterday.Date)
            {
                string mark = GymVisit.StringFromDate(daymark);
                // Don't forget to allow for more than one visit on a given day e.g. gym in the morning, aqua in the evening
                string stuff = string.Empty;
                foreach (GymVisit g in SevenDaysToYesterday)
                {
                    if (g.WhenCode == mark)
                    {
                        if (g.Activity== GymHistory.GymType.AquaAerobics) { stuff += "A"; }
                        if (g.Activity == GymHistory.GymType.GymTraining) { stuff += "G"; }
                        if (g.Activity == GymHistory.GymType.Other) { stuff += "O"; }
                    }
                }
                YesterWeekListBox.Items.Add(ListEntry(daymark, stuff));
                daymark = daymark.AddDays(1);
            }
            TodayWeekTextBlock.Text = "Week to today";
            TodayWeekTextBlock.Inlines.Add(Ticky(SevenDaysToToday.Count));
            daymark = WeekAgoToday;
            while (daymark.Date <= DateTime.Today.Date)
            {
                string mark = GymVisit.StringFromDate(daymark);
                string stuff = string.Empty;
                foreach (GymVisit g in SevenDaysToToday)
                {
                    if (g.WhenCode == mark)
                    {
                        if (g.Activity == GymHistory.GymType.AquaAerobics) { stuff += "A"; }
                        if (g.Activity == GymHistory.GymType.GymTraining) { stuff += "G"; }
                        if (g.Activity == GymHistory.GymType.Other) { stuff += "O"; }
                    }
                }
                TodayWeekListBox.Items.Add(ListEntry(daymark, stuff));
                daymark = daymark.AddDays(1);
            }

            // Display all-time, rolling month and rolling week stats
            WeekPanel.Children.Clear();
            MnthPanel.Children.Clear();
            TotlPanel.Children.Clear();

            int wv = PeriodVisitsUpTo(DateTime.Today, 7, out int wa, out int wo);
            TextBlock bloc = new TextBlock() { Text = "Rolling week sessions", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260 };
            WeekPanel.Children.Add(bloc);
            
            bloc = new TextBlock() { Text = $"{wv} activities in 7 days", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            WeekPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{wv -( wa+wo)} gym", Foreground =ActivityBrush(GymHistory.GymType.GymTraining), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            WeekPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{wa} aqua", Foreground =ActivityBrush(GymHistory.GymType.AquaAerobics), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            WeekPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{wo} other", Foreground = ActivityBrush( GymHistory.GymType.Other), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            WeekPanel.Children.Add(bloc);

            double elapsed = (DateTime.Today - _antedate).TotalDays;
            double twentyeight = Math.Min(28, elapsed);

            int mv = PeriodVisitsUpTo(DateTime.Today, 28, out int ma, out int mo);
            bloc = new TextBlock() { Text = "Rolling 4-weeks average sessions per week", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260 };
            MnthPanel.Children.Add(bloc);
            string pc = PercentageString(mv, twentyeight);
            bloc = new TextBlock() { Text = $"{mv} activities in {twentyeight} days", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            MnthPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{mv - (ma+mo)} gym", Foreground =ActivityBrush(GymHistory.GymType.GymTraining), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            MnthPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{ma} aqua", Foreground = ActivityBrush(GymHistory.GymType.AquaAerobics), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            MnthPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{mo} other", Foreground =ActivityBrush(GymHistory.GymType.Other), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            MnthPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = pc, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            MnthPanel.Children.Add(bloc);

            int v = AllVisitsUpTo(DateTime.Today, out int a, out int o);
            bloc = new TextBlock() { Text = "All time average sessions per week", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260 };
            TotlPanel.Children.Add(bloc);
            pc = PercentageString(v, elapsed);
            bloc = new TextBlock() { Text = $"{v} activities in {elapsed} days", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            TotlPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{v - (a+o)} gym", Foreground = ActivityBrush(GymHistory.GymType.GymTraining), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            TotlPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{a} aqua", Foreground = ActivityBrush(GymHistory.GymType.AquaAerobics), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            TotlPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = $"{o} other", Foreground = ActivityBrush(GymHistory.GymType.Other), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            TotlPanel.Children.Add(bloc);
            bloc = new TextBlock() { Text = pc, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
            TotlPanel.Children.Add(bloc);

            // Plot sessions and graphs
            PlotSessions();
            PlotGraph();
        }

        private int AllVisitsUpTo(DateTime d, out int a, out int o)
        {
            int v=a =o= 0;
            DateTime lendemain = d.AddDays(1).Date;
            foreach (GymVisit g in _history.GymList)
            {
                if (g.When < lendemain)
                {
                    v++;
                    if (g.Activity == GymHistory.GymType.AquaAerobics) { a++; }
                    if (g.Activity == GymHistory.GymType.Other) { o++; }
                }
            }
            return v;
        }

        private int PeriodVisitsUpTo(DateTime d, int days, out int a, out int o)
        {
            int v = a = o = 0;
            DateTime lendemain = d.AddDays(1).Date;
            DateTime baseline = d.AddDays(-days).Date;
            foreach (GymVisit g in _history.GymList)
            {
                if (g.When < lendemain)
                {
                    if (g.When > baseline)
                    {
                        v++;
                        if (g.Activity == GymHistory.GymType.AquaAerobics) { a++; }
                        if (g.Activity == GymHistory.GymType.Other) { o++; }
                    }
                }
            }
            return v;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _history.SaveData();
            DialogResult = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            CloseButton.IsEnabled = false;
            AddStackPanel.Visibility = Visibility.Visible;
            GymDatePicker.DisplayDateStart = new DateTime(2019, 12, 1);
            GymDatePicker.DisplayDateEnd = DateTime.Today;
            GymDatePicker.SelectedDate = DateTime.Today;
            OtherRadio.IsChecked = true;
        }

        private void AddCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = true;
            DeleteButton.IsEnabled = false;
            CloseButton.IsEnabled = true;
            AddStackPanel.Visibility = Visibility.Hidden;
        }

        private void AddConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            GymVisit gv = new GymVisit();
            if (GymRadio.IsChecked.Value == true) 
            { gv.Activity = GymHistory.GymType.GymTraining; } 
            else if (AquaRadio.IsChecked.Value==true)
            { gv.Activity = GymHistory.GymType.AquaAerobics; }
            else if (OtherRadio.IsChecked.Value == true)
            { gv.Activity = GymHistory.GymType.Other; }
            if (GymDatePicker.SelectedDate.HasValue)
            {
                gv.When = GymDatePicker.SelectedDate.Value; 
                if (gv.When > DateTime.Today)
                {
                    MessageBox.Show("The selected date is in the future", Jbh.AppManager.AppName, MessageBoxButton.OK, MessageBoxImage.Warning); return;
                }
            }
            else
            {
                MessageBox.Show("Please select a date", Jbh.AppManager.AppName, MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }
            _history.GymList.Add(gv);
            RefreshList();
            AddButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            AddStackPanel.Visibility = Visibility.Hidden;
        }

        private void GymVisitsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool q = GymVisitsListBox.SelectedIndex >= 0;
            DeleteButton.IsEnabled = q;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = GymVisitsListBox.SelectedItem as ListBoxItem;
            int j = (int)item.Tag;
            _history.GymList.RemoveAt(j);
            RefreshList();
        }

        private void PlotSessions()
        {
            GraphCanvasSess.Children.Clear();
            DateTime moment = new DateTime(2019, 12, 16);
            int daysago = (int)Math.Floor((DateTime.Today - moment).TotalDays);
            //double fourLeft = daywidth * (daysago - 25);
            //double weekLeft = daywidth * (daysago - 4);
            double fourLeft = daywidth * (daysago - 27);
            double weekLeft = daywidth * (daysago - 6);
            Rectangle fourrect = new Rectangle() { Width = daywidth * 28, Height = GraphCanvasSess.ActualHeight, Fill = Brushes.BurlyWood, Opacity=0.4 };
            Rectangle weekrect = new Rectangle() { Width = daywidth * 7, Height = GraphCanvasSess.ActualHeight, Fill = Brushes.BurlyWood , Opacity=0.7};
            Canvas.SetLeft(fourrect, fourLeft);
            Canvas.SetLeft(weekrect, weekLeft);
            GraphCanvasSess.Children.Add(fourrect);
            GraphCanvasSess.Children.Add(weekrect);
            TextBlock explain4TB = new TextBlock() { Text = "28 days", FontSize = 10, Foreground = Brushes.SaddleBrown };
            Canvas.SetLeft(explain4TB, fourLeft);
            Canvas.SetTop(explain4TB, 30);
            GraphCanvasSess.Children.Add(explain4TB);
            TextBlock explain1TB = new TextBlock() { Text = "7 days", FontSize = 10, Foreground = Brushes.SaddleBrown };
            Canvas.SetLeft(explain1TB, weekLeft);
            Canvas.SetTop(explain1TB, 30);
            GraphCanvasSess.Children.Add(explain1TB);
            //double xpos = daywidth;
            double xpos = 0;
            while (moment <= DateTime.Today)
            {
                xpos += daywidth;
                string code = GymVisit.StringFromDate(moment);
                List<GymVisit> daySessions = new List<GymVisit>();

                foreach (GymVisit gv in _history.GymList)
                {
                    if (gv.WhenCode == code) { daySessions.Add(gv); }
                }

                switch (daySessions.Count)
                {
                    case 0:
                        {
                            Rectangle carre = new Rectangle() { Height = 24, Width = daywidth - 0.5, Fill = Brushes.White, Stroke = Brushes.Tan, StrokeThickness = 0.3 };
                            Canvas.SetLeft(carre, xpos);
                            Canvas.SetTop(carre, 4);
                            GraphCanvasSess.Children.Add(carre);
                            break;
                        }
                    case 1:
                        {
                            double ypos = 10;
                            GymVisit gv = daySessions[0];

                            Brush pinceau = ActivityBrush(gv.Activity);
                            Rectangle carre = new Rectangle() { Height = 12, Width = daywidth - 0.5, Fill = pinceau };
                            Canvas.SetLeft(carre, xpos);
                            Canvas.SetTop(carre, ypos);
                            GraphCanvasSess.Children.Add(carre);

                            break;
                        }
                    default:
                        {
                            double ypos = 4;
                            double visits = daySessions.Count;
                            double sessionHeight = (24 - (2 * (visits - 1))) / visits;
                            foreach (GymVisit gv in daySessions)
                            {
                                Brush pinceau = ActivityBrush(gv.Activity);
                                Rectangle carre = new Rectangle() { Height = sessionHeight, Width = daywidth - 0.5, Fill = pinceau };
                                Canvas.SetLeft(carre, xpos);
                                Canvas.SetTop(carre, ypos);
                                GraphCanvasSess.Children.Add(carre);
                                ypos += (sessionHeight + 2);
                            }
                            break;
                        }
                }
                moment = moment.AddDays(1);
            }
        }

        private void PlotGraph()
        {
            double gcw = GraphCanvasWeek.ActualWidth;
            double gch = GraphCanvasWeek.ActualHeight;

            double yHeight = gch / 7.1;

            GraphCanvasWeek.Children.Clear();
            GraphCanvasMnth.Children.Clear();
            GraphCanvasTotl.Children.Clear();

            // Draw horizontal interval lines
            for (int f = 1; f < 8; f++)
            {
                double yp = gch - (f * yHeight);
                if (f == 5)
                {
                    Line w = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5, StrokeDashArray = new DoubleCollection { 12, 6 } };
                    GraphCanvasWeek.Children.Add(w);
                    Line m = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5, StrokeDashArray = new DoubleCollection { 12, 6 } };
                    GraphCanvasMnth.Children.Add(m);
                    Line t = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5, StrokeDashArray = new DoubleCollection { 12, 6 } };
                    GraphCanvasTotl.Children.Add(t);
                }
                else
                {
                    Line w = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5 };
                    GraphCanvasWeek.Children.Add(w);
                    Line m = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5 };
                    GraphCanvasMnth.Children.Add(m);
                    Line t = new Line() { X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5 };
                    GraphCanvasTotl.Children.Add(t);
                }
            }

            DateTime moment = new DateTime(2019, 12, 16);

            // Get values
            List<Tuple<DateTime, double>> weekliesG = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> weekliesGA = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> weekliesGAO = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> mnthliesG = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> mnthliesGA = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> mnthliesGAO = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> totlliesG = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> totlliesGA = new List<Tuple<DateTime, double>>();
            List<Tuple<DateTime, double>> totlliesGAO = new List<Tuple<DateTime, double>>();

            while (moment <= DateTime.Today)
            {
                int wv = PeriodVisitsUpTo(moment, 7, out int wa, out int wo);
                int mv = PeriodVisitsUpTo(moment, 28, out int ma, out int mo);
                int tv = AllVisitsUpTo(moment, out int ta, out int to);

                double elapsed = (moment - _antedate).TotalDays;
                double twentyeight = Math.Min(28, elapsed);
                double mnthweeks = 4 * (twentyeight / 28);
                double totlweeks = elapsed / 7;
                Tuple<DateTime, double> wG = new Tuple<DateTime, double>(moment, wv - (wa+wo));
                Tuple<DateTime, double> wGA = new Tuple<DateTime, double>(moment, wv - wo);
                Tuple<DateTime, double> wGAO = new Tuple<DateTime, double>(moment, wv);
                Tuple<DateTime, double> mG = new Tuple<DateTime, double>(moment, (mv - (ma+mo)) / mnthweeks);
                Tuple<DateTime, double> mGA = new Tuple<DateTime, double>(moment, (mv - mo) / mnthweeks);
                Tuple<DateTime, double> mGAO = new Tuple<DateTime, double>(moment, mv / mnthweeks);
                Tuple<DateTime, double> tG = new Tuple<DateTime, double>(moment, (tv - (ta+to)) / totlweeks);
                Tuple<DateTime, double> tGA = new Tuple<DateTime, double>(moment, (tv - to) / totlweeks);
                Tuple<DateTime, double> tGAO = new Tuple<DateTime, double>(moment, tv / totlweeks);
                weekliesG.Add(wG);
                weekliesGA.Add(wGA);
                weekliesGAO.Add(wGAO);
                mnthliesG.Add(mG);
                mnthliesGA.Add(mGA);
                mnthliesGAO.Add(mGAO);
                totlliesG.Add(tG);
                totlliesGA.Add(tGA);
                totlliesGAO.Add(tGAO);
                moment = moment.AddDays(1);
            }
            //// Plot graph
            int duration = weekliesGAO.Count;
            double cwidth = Math.Max(gcw, duration * daywidth);
            GraphCanvasSess.Width = cwidth;
            GraphCanvasWeek.Width = cwidth;
            GraphCanvasMnth.Width = cwidth;
            GraphCanvasTotl.Width = cwidth;
            SessScroller.ScrollToRightEnd();
            WeekScroller.ScrollToRightEnd();
            MnthScroller.ScrollToRightEnd();
            TotlScroller.ScrollToRightEnd();
            
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.Other), weekliesGAO);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.AquaAerobics), weekliesGA);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.GymTraining), weekliesG);

            PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.Other), mnthliesGAO);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.AquaAerobics), mnthliesGA);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.GymTraining), mnthliesG);

            PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.Other), totlliesGAO);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.AquaAerobics), totlliesGA);
            PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.GymTraining), totlliesG);
        }

        private void PlotPerformance(double DayWidth, double GraphHeight, double IncrementHeight, Canvas Slate, Brush pinceau, List<Tuple<DateTime, double>> data)
        {
            double xpos = 0;
            int counter = data.Count;
            foreach (Tuple<DateTime, double> tup in data)
            {
                xpos += DayWidth;
                double ypos = GraphHeight - (IncrementHeight * tup.Item2);
                Point next = new Point() { X = xpos, Y = ypos };
                Rectangle rex = new Rectangle() { Fill = pinceau, Width = DayWidth, Height = IncrementHeight * tup.Item2, Opacity=0.7 };
                Canvas.SetLeft(rex, xpos);
                Canvas.SetTop(rex, ypos);
                Slate.Children.Add(rex);
                counter--;
            }
        }

        private ListBoxItem ListEntry(GymVisit gv)
        {
            FontFamily ff = new FontFamily("Lucida Console");
            string Which = "Nothing";
        Brush    brosse = ActivityBrush(gv.Activity);
            if (gv.Activity == GymHistory.GymType.AquaAerobics)
            {  Which = "Aqua aerobics"; }
            else if (gv.Activity == GymHistory.GymType.GymTraining)
            {  Which = "Gym training"; }
            else if (gv.Activity == GymHistory.GymType.Other)
            {  Which = "Other activity"; }
            TextBlock td = new TextBlock() { Text = $"{gv.When.ToShortDateString()} {gv.When:ddd}", FontFamily = ff };
            TextBlock tw = new TextBlock() { Text = Which, FontFamily = ff, Foreground = brosse, Margin = new Thickness(8, 0, 0, 0) };
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            sp.Children.Add(td);
            sp.Children.Add(tw);
            return new ListBoxItem() { Content = sp, Tag = gv.Index, IsHitTestVisible = (gv.Index > 0) };
        }

        private ListBoxItem ListEntry(DateTime when, string what)
        {
            FontFamily ff = new FontFamily("Lucida Console");
            Thickness marge = new Thickness(6, 0, 0, 3);
            Brush brosse = Brushes.BurlyWood;
            
            TextBlock td = new TextBlock() { Text = $"{when:ddd}", FontFamily = ff };
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            sp.Children.Add(td);
                for (int x = 0; x < what.Length; x++)
                {
                    char q = what[x];
                    if (q == 'A') { brosse =ActivityBrush(GymHistory.GymType.AquaAerobics); } else if (q == 'G') { brosse =ActivityBrush(GymHistory.GymType.GymTraining); } else if (q == 'O') { brosse = ActivityBrush(GymHistory.GymType.Other); }
                    Rectangle oblong = new Rectangle() { Width = 20, Height = 9, Margin = marge, Fill = brosse };
                    sp.Children.Add(oblong);
                }
            
            return new ListBoxItem() { Content = sp,  IsHitTestVisible = false };
        }

        private string PercentageString(int num, double tot)
        {
            if (tot == 0) { return "ERR%"; }
            double all = (double)tot;
            double per = 70 * (num / all);
            per = Math.Round(per)/10;
            return $"{per:0.0} per week";
        }

        private Run Ticky(int v)
        {
            string tick = (v >= 5) ? " ü" : " û"; // tick or cross
            Brush brosse = (v >= 5) ? Brushes.Green : Brushes.Red;
            Run couru = new Run() { Text = tick, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Wingdings"), FontSize = 18, Foreground = brosse };
            return couru;
        }

        private void AddTodayButton_Click(object sender, RoutedEventArgs e)
        {
            GymDatePicker.SelectedDate = DateTime.Today;
        }

        private void AddYesterdayButton_Click(object sender, RoutedEventArgs e)
        {
            GymDatePicker.SelectedDate = DateTime.Today.AddDays(-1);
        }

        private void AddAvanthierButton_Click(object sender, RoutedEventArgs e)
        {
            GymDatePicker.SelectedDate = DateTime.Today.AddDays(-2);
        }

        private void AddBackwardButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime when = DateTime.Today;
            if (GymDatePicker.SelectedDate.HasValue)
            {
                when = GymDatePicker.SelectedDate.Value;
            }

            GymDatePicker.SelectedDate = when.AddDays(-1);
        }

        private void AddForwardsButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime when = DateTime.Today;
            if (GymDatePicker.SelectedDate.HasValue)
            {
                when = GymDatePicker.SelectedDate.Value;
            }

            GymDatePicker.SelectedDate = when.AddDays(1);
        }
    }
}
