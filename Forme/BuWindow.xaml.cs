﻿using System;
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

            double perweek =7* pasbuBeans/(double)(buBeans+pasbuBeans);

            RubricTextBlock.Text = $"For every day on which I don't drink, {BeanString(pasbuBeans)} {AreString(pasbuBeans)} put into the pot. For every day on which I do drink, {BeanString(buBeans)} {AreString(buBeans)} taken out of the pot. The aim is to keep the number of beans in the pot positive. This equates to {perweek} drinking days per week.";
            CycleBorder.Visibility = Visibility.Hidden;
            LoadData();
        }
        
        private string BeanString(int b)
        {
            string r = "beans";
            if (b==1 || b == -1) { r = "bean"; }
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
            DateTime yesterday = DateTime.Today.AddDays(-1);
            int hier = BuDate.DayOfLife(yesterday);
            while (jour <= hier)
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
            foreach(int j in buDictionary.Keys)
            {
                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock bloc = new TextBlock() { Text = BuDate.DateStamp(j), FontFamily = new FontFamily("Consolas"),VerticalAlignment = VerticalAlignment.Center };
                TextBlock sign = new TextBlock() {Width=32 , VerticalAlignment= VerticalAlignment.Center, Background= Brushes.AntiqueWhite, TextAlignment= TextAlignment.Center };
                beans = BeansAfter(beans, buDictionary[j]);
                switch (buDictionary[j])
                {
                    case BuDate.Ivresse.SaisPas: { sign.Text= "s"; sign.FontFamily = new FontFamily("Webdings");sign.FontSize = 14; sign.Foreground = Brushes.Red; break; }
                    case BuDate.Ivresse.Bu: { sign.Text = "S"; sign.FontFamily = new FontFamily("Wingdings"); sign.FontSize = 16;sign.Foreground = Brushes.BlueViolet; break; }
                    case BuDate.Ivresse.PasBu: { sign.Text = "r"; sign.FontFamily = new FontFamily("Webdings"); sign.FontSize = 14; sign.Foreground = Brushes.ForestGreen; break; }
                }
                panel.Children.Add(bloc);
                panel.Children.Add(sign);
                panel.Children.Add(new TextBlock() { Text = $"{beans} beans", Foreground = beans < 0 ? Brushes.Red : Brushes.DarkGreen, VerticalAlignment= VerticalAlignment.Center, FontFamily = new FontFamily("Consolas") });
                DatesListBox.Items.Add(new ListBoxItem() {Tag=j,Height=24, Content = panel} );
            }
            DatesListBox.SelectedIndex = f;
            DatesListBox.ScrollIntoView(DatesListBox.SelectedItem);
            RefreshGraphics();
        }

        private void RefreshGraphics()
        {
            PlotRecentDays();
            int z = PlotRollingPeriod(WeekCanvas, 7);
            WeeklyBalanceTextBlock.Text = $"{z}";
            z = PlotRollingPeriod(MonthCanvas, 28);
            MonthlyBalanceTextBlock.Text = $"{z}";
            z = PlotRollingPeriod(TotalCanvas, 99999);
            TotalBalanceTextBlock.Text = $"{z}";
        }

        private void PlotRecentDays()
        {
            RecentDaysDockPanel.Children.Clear();
            int currentcode = BuDate.DayOfLife(DateTime.Today);
            for (int z=0; z<56; z++)
            {
                currentcode--;
                Brush pinceau;
                if (buDictionary.ContainsKey(currentcode))
                {
                    switch (buDictionary[currentcode])
                    {
                        case BuDate.Ivresse.Bu: { pinceau = Brushes.OrangeRed; break; }
                        case BuDate.Ivresse.PasBu: { pinceau = Brushes.ForestGreen; break; }
                        default: { pinceau = Brushes.Silver; break; }
                    }
                    Ellipse elly = new Ellipse() { Width = 16, Height = 16, Fill = pinceau, Margin=new Thickness(4,2,0,2), VerticalAlignment = VerticalAlignment.Center };
                    DockPanel.SetDock(elly, Dock.Right);
                    RecentDaysDockPanel.Children.Add(elly);
                }
            }

        }

        private int PlotRollingPeriod(Canvas whichCanvas, int spanDays)
        {
            whichCanvas.Children.Clear();
            double cheight = whichCanvas.ActualHeight;
            double elapsedDays = (DateTime.Today - startDate).TotalDays;
            double dayInterval = 4;
            whichCanvas.Width = dayInterval * elapsedDays;
            (int, int) minimax = PeriodMinMaxBeans(spanDays);
            double tspan = minimax.Item2 - minimax.Item1;
            double yratio = 0.9 * cheight / tspan;
            double prop = minimax.Item2 / tspan;
            double yorigin = cheight * prop;
             whichCanvas.Children.Add(new Line() { X1 = 0, X2 = dayInterval * elapsedDays, Y1 = yorigin, Y2 = yorigin, StrokeThickness = 1, Stroke = Brushes.DimGray });
            int firstdate = buDictionary.Keys.First();
            double xposition = 0;
            
            double lastx = 0;
            double lasty = yorigin;
            int beans=0;
            foreach (int target in buDictionary.Keys)
            {
                beans = 0;
                int debut = target - (spanDays-1);
                if (debut < firstdate) { debut = firstdate; }
                for (int x = debut; x <= target; x++)
                {
                    beans = BeansAfter(beans, buDictionary[x]);
                }
                double thisY = yorigin - (beans * yratio);
                if (xposition > 0)
                {
                    Brush pinceau = (beans < 0) ? Brushes.Red : Brushes.Blue;
                    whichCanvas.Children.Add(new Line() { X1 = lastx, Y1 = lasty, X2 = xposition, Y2 = thisY, Stroke = Brushes.OrangeRed, StrokeThickness = 1.5 });
                }

                lastx = xposition;
                lasty = thisY;
                xposition += dayInterval;
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