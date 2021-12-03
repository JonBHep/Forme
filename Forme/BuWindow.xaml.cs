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
    /// Interaction logic for BuWindow.xaml
    /// </summary>
    public partial class BuWindow : Window
    {
        public BuWindow()
        {
            InitializeComponent();
            buDictionary = new Dictionary<int, BuDate.Ivresse>();
        }

        private const int buBeans= 5;
        private const int pasbuBeans = 2;
        private const string budatafile= "Data.bu";

        private readonly DateTime startDate = new DateTime(2021, 10, 15);
        private readonly Dictionary<int, BuDate.Ivresse> buDictionary;

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

            double perweek = 7 * pasbuBeans / (double)(buBeans + pasbuBeans);

            RubricTextBlock.Text = $"For every day on which I don't drink, {BeanString(pasbuBeans)} {AreString(pasbuBeans)} put into the pot. For every day on which I do drink, {BeanString(buBeans)} {AreString(buBeans)} taken out of the pot. The aim is to keep the number of beans in the pot positive. {perweek} drinking days per week will keep the beans steady.";
            CycleBorder.Visibility = Visibility.Hidden;
            LoadData();
        }
        
        private string BeanString(int b)
        {
            string r = "beans";
            if (b == 1 || b == -1) { r = "bean"; }
            return $"{b} {r}";
        }

        private string AreString(int b)
        {
            string r = "are";
            if (b == 1 || b == -1) { r = "is"; }
            return r;
        }

        private void SaveData()
        {
            string Bufile = System.IO.Path.Combine(AppManager.DataPath, budatafile);
            // backup existing data
            AppManager.CreateBackupDataFile(Bufile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(budatafile), MinimumDaysToKeep: 40, MinimumFilesToKeep: 4);

            // write new data
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Bufile))
            {
                foreach (int d in buDictionary.Keys)
                {
                    BuDate bud = new BuDate() { QuandInteger = d, Quoi = buDictionary[d] };
                    sw.WriteLine(bud.Specification);
                }
            }
        }

        private void LoadData()
        {
            InitialiseDictionary();
            string Bufile = System.IO.Path.Combine(AppManager.DataPath, budatafile);
            if (System.IO.File.Exists(Bufile))
            {
                using (System.IO.StreamReader rd = new System.IO.StreamReader(Bufile))
                {
                    while (!rd.EndOfStream)
                    {
                        string i = rd.ReadLine();
                        BuDate bd = new BuDate() { Specification = i };
                        int j = bd.QuandInteger;
                        if (buDictionary.ContainsKey(j))
                        {
                            buDictionary[j] = bd.Quoi;
                        }
                        else
                        {
                            buDictionary.Add(j, bd.Quoi); // theoretically, this should not occur as dictionary has been initialised with range of dates
                        }
                    }
                }
            }
            RefreshDateList();
        }

        private void InitialiseDictionary()
        {
            buDictionary.Clear();
            int jour = BuDate.DayOfLife(startDate);
            int maintenant = BuDate.DayOfLife(DateTime.Today);
            while (jour <= maintenant)
            {
                buDictionary.Add(jour, BuDate.Ivresse.SaisPas);
                jour++;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }

        private void RefreshDateList()
        {
            int f = DatesListBox.SelectedIndex;
            int beans = 0;
            DatesListBox.Items.Clear();
            List<int> days = new List<int>();
            foreach (int j in buDictionary.Keys)
            {
                days.Add(j);
            }
            days.Sort();
            foreach (int j in days)
            {
                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock bloc = new TextBlock() { Text = BuDate.DateStamp(j), FontFamily = new FontFamily("Consolas"),VerticalAlignment = VerticalAlignment.Center };
                TextBlock sign = new TextBlock() {Width=32 , VerticalAlignment= VerticalAlignment.Center, TextAlignment= TextAlignment.Center };
                beans = BeansAfter(beans, buDictionary[j]);
                switch (buDictionary[j])
                {
                    case BuDate.Ivresse.SaisPas: { sign.Text= "s"; sign.FontFamily = new FontFamily("Webdings");sign.FontSize = 14; sign.Foreground = Brushes.Red; break; }
                    case BuDate.Ivresse.Bu: { sign.Text = "S"; sign.FontFamily = new FontFamily("Wingdings"); sign.FontSize = 16; sign.Foreground = Brushes.BlueViolet; break; }
                    case BuDate.Ivresse.PasBu: { sign.Text = "r"; sign.FontFamily = new FontFamily("Webdings"); sign.FontSize = 14; sign.Foreground = Brushes.ForestGreen; break; }
                }
                panel.Children.Add(bloc);
                panel.Children.Add(sign);
                panel.Children.Add(new TextBlock() { Text = $"{beans} beans", Foreground = beans < 0 ? Brushes.Red : Brushes.DarkGreen, VerticalAlignment= VerticalAlignment.Center, FontFamily = new FontFamily("Consolas") });
                DatesListBox.Items.Insert(0,new ListBoxItem() {Tag= j, Height=24, Content = panel} );
            }
            DatesListBox.SelectedIndex = f;
            DatesListBox.ScrollIntoView(DatesListBox.SelectedItem);
            RefreshGraphics();
        }

        private void RefreshGraphics()
        {
            // Show graphics for periods up to today if today's result is known, otherwise to yesterday
            DateTime lastDate = DateTime.Today;
            int lastDol = BuDate.DayOfLife(lastDate);
            UntilTextBlock.Text = "Graphics up to TODAY";
            if (buDictionary[lastDol] == BuDate.Ivresse.SaisPas)
            {
                lastDate = DateTime.Today.AddDays(-1);
                lastDol = BuDate.DayOfLife(lastDate);
                UntilTextBlock.Text = "Graphics up to  YESTERDAY";
            }

            double elapsedDays = (lastDate - startDate).TotalDays+1;
            double elapsedweeks = elapsedDays / 7;
            PlotRecentDays(lastDol);
            int z = PlotRollingPeriod(WeekCanvas, 7, elapsedDays, lastDol);
            WeeklyBalanceTextBlock.Text = $"{z}";
            z = PlotRollingPeriod(FortnightCanvas, 14, elapsedDays, lastDol);
            FortnightlyBalanceTextBlock.Text = $"{z} ({z/2d} per week)";
            z = PlotRollingPeriod(MonthCanvas, 28, elapsedDays, lastDol);
            MonthlyBalanceTextBlock.Text = $"{z} ({z / 4d} per week)";
            z = PlotRollingPeriod(TotalCanvas, 99999, elapsedDays, lastDol);
            TotalBalanceTextBlock.Text = $"{z} ({z/elapsedweeks:0.0} per week)";
        }

        private void PlotRecentDays(int finalCode)
        {
            RecentDaysDockPanel.Children.Clear();
            int currentcode = finalCode + 1;
            for (int z=0; z<77; z++)
            {
                currentcode--;
                if (buDictionary.ContainsKey(currentcode))
                {
                    Polygon bouteille = Bottle(buDictionary[currentcode]);
                    DockPanel.SetDock(bouteille, Dock.Right);
                    RecentDaysDockPanel.Children.Add(bouteille);

                    if (z % 7 == 6)
                    {
                        // add week marker
                        Ellipse elyps=new Ellipse() { Width = 16, Height = 16, Fill = Brushes.Tan, Margin = new Thickness(1, 2, 1, 2) };
                        DockPanel.SetDock(elyps, Dock.Right);
                        RecentDaysDockPanel.Children.Add(elyps);
                    }
                }
            }
        }

        private Polygon Bottle(BuDate.Ivresse etat)
        {
            Brush pinceau;
            Brush outline;
            switch (etat)
            {
                case BuDate.Ivresse.Bu: { pinceau = Brushes.Firebrick; outline = Brushes.Brown; break; }
                case BuDate.Ivresse.PasBu: { pinceau = Brushes.Aquamarine; outline = Brushes.Black; break; }
                default: { pinceau = Brushes.Silver; outline = Brushes.Gainsboro; break; }
            }

            Polygon myPolygon = new Polygon
            {
                Stroke = outline,
                Fill = pinceau,
                StrokeThickness = 1,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };
            PointCollection myPointCollection = new PointCollection
                    {
                        new Point(0, 24),
                        new Point(7, 24),
                        new Point(7, 7),
                        new Point(4.67, 6),
                        new Point(4.67, 0),
                        new Point(2.33, 0),
                        new Point(2.33, 6),
                        new Point(0, 7)
                    };
            myPolygon.Points = myPointCollection;
            return myPolygon;
        }

        private int PlotRollingPeriod(Canvas whichCanvas, int spanDays, double totalDays, int endDateCode)
        {
            whichCanvas.Children.Clear();
            double cheight = whichCanvas.ActualHeight;
            double topmargin = 8;
            double usableheight = cheight - (2*topmargin);

            double wks = Math.Round(totalDays / 7);
            AllTimeCaptionBloc.Text = $"All time ({wks} weeks) bean balance";
            double dayInterval = 4;
            whichCanvas.Width = dayInterval * totalDays;
            (int, int) minimax = PeriodMinMaxBeans(spanDays);
            double tspan = minimax.Item2 - minimax.Item1;
            double yratio = usableheight / tspan;
            double prop = minimax.Item2 / tspan;
            double yorigin =topmargin+ usableheight * prop;
            whichCanvas.Children.Add(new Line() { X1 = 0, X2 = dayInterval * totalDays, Y1 = yorigin, Y2 = yorigin, StrokeThickness = 1.5, Stroke = Brushes.CornflowerBlue, StrokeDashArray = { 5, 3 } });
            int firstdate = buDictionary.Keys.First();

            double xposition = 0;
            double lastx = 0;
            double lasty = yorigin;
            int beans = 0;
            foreach (int target in buDictionary.Keys)
            {
                if (target <= endDateCode) // if lastdate is yesterday do not include today in graph
                {
                    xposition += dayInterval;
                    beans = 0;
                    int debut = target - (spanDays - 1);
                    if (debut < firstdate) { debut = firstdate; }
                    for (int x = debut; x <= target; x++)
                    {
                        beans = BeansAfter(beans, buDictionary[x]);
                    }
                    double thisY = yorigin - (beans * yratio);
                    Brush pinceau = (beans < 0) ? Brushes.Red : Brushes.Blue;
                    whichCanvas.Children.Add(new Line() { X1 = lastx, Y1 = lasty, X2 = xposition, Y2 = thisY, Stroke = Brushes.OrangeRed, StrokeThickness = 1.5 });

                    lastx = xposition;
                    lasty = thisY;
                }
            }
            return beans;
        }

        private (int, int) PeriodMinMaxBeans(int days)
        {
            int mini = 0;
            int maxi = 0;
            int firstdate = buDictionary.Keys.First();
            foreach (int target in buDictionary.Keys)
            {
                int xbeans = 0;
                int debut = target - (days-1);
                if (debut < firstdate) { debut = firstdate; }
                for (int x = debut; x <= target; x++)
                {
                    xbeans = BeansAfter(xbeans, buDictionary[x]);
                }

                mini = Math.Min(mini, xbeans);
                maxi = Math.Max(maxi, xbeans);
            }
            return (mini, maxi);
        }

        private int BeansAfter(int feves, BuDate.Ivresse quoi)
        {
            int apres;
            switch (quoi)
            {
                case BuDate.Ivresse.Bu: {apres=feves- buBeans;  break; }
                case BuDate.Ivresse.PasBu: {apres=feves+ pasbuBeans; break; }
                default: { apres = feves; break; }
            }
            return apres;
        }

        private void CycleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatesListBox.SelectedItem is ListBoxItem item)
            {
                if (item.Tag is int i)
                {
                    BuDate.Ivresse ivre = buDictionary[i];
                    BuDate.Ivresse nova = BuDate.Ivresse.SaisPas;
                    switch (ivre)
                    {
                        case BuDate.Ivresse.SaisPas: { nova = BuDate.Ivresse.Bu; break; }
                        case BuDate.Ivresse.Bu: { nova = BuDate.Ivresse.PasBu; break; }
                        case BuDate.Ivresse.PasBu: { nova = BuDate.Ivresse.SaisPas; break; }
                    }
                    buDictionary[i] = nova;
                    RefreshDateList();
                }
            }
        }

        private void DatesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CycleBorder.Visibility = (DatesListBox.SelectedIndex < 0) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
