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
    /// Interaction logic for RatiosWindow.xaml
    /// </summary>
    public partial class RatiosWindow : Window
    {
        public RatiosWindow()
        {
            InitializeComponent();
        }

        private List<BikeGears> _bikes = new List<BikeGears>();

        private void JhChainringsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _bikes[0].Chainrings = BikeGears.CogWheelTeeth(JhChainringsTextBox.Text);
            JhChainringsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[0].Chainrings);
        }

        private void RsChainringsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _bikes[1].Chainrings = BikeGears.CogWheelTeeth(RsChainringsTextBox.Text);
            RsChainringsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[1].Chainrings);
        }

        private void JhSprocketsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _bikes[0].Sprockets = BikeGears.CogWheelTeeth(JhSprocketsTextBox.Text);
            JhSprocketsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[0].Sprockets);
        }

        private void RsSprocketsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _bikes[1].Sprockets = BikeGears.CogWheelTeeth(RsSprocketsTextBox.Text);
            RsSprocketsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[1].Sprockets);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int z = 0; z < _bikes.Count(); z++)
            {
                _bikes[z].Save();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string[] jbhFiles = System.IO.Directory.GetFiles(Jbh.AppManager.DataPath, "*.jbh", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string path in jbhFiles)
            {
                string file = System.IO.Path.GetFileNameWithoutExtension(path);
                BikeGears velo = new BikeGears(file);
                velo.Load();
                _bikes.Add(velo);
            }
            JhTitleTextBlock.Text = _bikes[0].BikeName;
            JhChainringsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[0].Chainrings);
            JhSprocketsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[0].Sprockets);
            JhCircumfTextBlock.Text = $"{_bikes[0].WheelCircumference} mm";

            RsTitleTextBlock.Text = _bikes[1].BikeName;
            RsChainringsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[1].Chainrings);
            RsSprocketsTextBlock.Text = BikeGears.CogwheelSpecificationString(_bikes[1].Sprockets);
            RsCircumfTextBlock.Text = $"{_bikes[1].WheelCircumference} mm";

            DisplayFigures();
        }

        private void DisplayFigures()
        {
            _bikes[0].CalculateGearings();
            _bikes[1].CalculateGearings();

            ListRatios(_bikes[0].Chainrings, _bikes[0].Sprockets, 0);
            ListRatios(_bikes[1].Chainrings, _bikes[1].Sprockets, 2);
            PlotRatios(_bikes[0].Chainrings, _bikes[0].Sprockets, _bikes[0].Gearings, _bikes[0].WheelCircumference, 1, 500);
            PlotRatios(_bikes[1].Chainrings, _bikes[1].Sprockets, _bikes[1].Gearings, _bikes[1].WheelCircumference, 3, 500);
        }

        private void ListRatios(List<int> crings, List<int> sproks, int gridCol)
        {
            Grid RatioGrid = new Grid();
            Grid.SetColumn(RatioGrid, gridCol);
            Grid.SetRow(RatioGrid, 7);
            RatioGrid.Background = Brushes.Beige;
            MainGrid.Children.Add(RatioGrid);
            int rings = crings.Count;
            int cogs = sproks.Count;
            for (int n = 0; n <= rings; n++) { RatioGrid.ColumnDefinitions.Add(new ColumnDefinition()); }
            for (int n = 0; n <= cogs; n++) { RatioGrid.RowDefinitions.Add(new RowDefinition()); }
            for (int n = 0; n < rings; n++)
            {
                TextBlock tb = new TextBlock() { Text = (n + 1).ToString(), Foreground = Brushes.Green, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(tb, n + 1);
                Grid.SetRow(tb, 0);
                RatioGrid.Children.Add(tb);
            }
            for (int n = 0; n < cogs; n++)
            {
                TextBlock tb = new TextBlock() { Text = (n + 1).ToString(), Foreground = Brushes.Green, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, n + 1);
                RatioGrid.Children.Add(tb);
            }
            for (int x = 0; x < rings; x++)
            {
                for (int y = 0; y < cogs; y++)
                {
                    double cr = ToothRatio(crings[x], sproks[y]);
                    TextBlock tb = new TextBlock() { Text = cr.ToString("0.00"), FontWeight = FontWeights.Medium, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    Grid.SetColumn(tb, x + 1);
                    Grid.SetRow(tb, y + 1);
                    RatioGrid.Children.Add(tb);
                }
            }
        }

        private void PlotRatios(List<int> crings, List<int> sproks, List<Gearing> gearings, int wheelCircumference, int gridCol, double graphheight)
        {
            Canvas RatioCanvas = new Canvas();
            Grid.SetColumn(RatioCanvas, gridCol);
            Grid.SetRow(RatioCanvas, 7);
            RatioCanvas.Background = Brushes.BlanchedAlmond;
            MainGrid.Children.Add(RatioCanvas);
            for (int x = 0; x < crings.Count; x++)
            {
                double minR = double.MaxValue;
                double maxR = double.MinValue;
                for (int y = 0; y < sproks.Count; y++)
                {
                    double cr = ToothRatio(crings[x], sproks[y]);
                    if (cr < minR) { minR = cr; }
                    if (cr > maxR) { maxR = cr; }
                    SolidColorBrush brosse = Brushes.DarkGreen;
                    if (Gearing.IsCrossover(crings.Count, sproks.Count, x + 1, y + 1)) { brosse = Brushes.Red; }
                    Line gline = new Line() { X1 = 10 + (x * 20), X2 = 30 + (x * 20), Y1 = graphheight - (graphheight * cr / 6), Y2 = graphheight - (graphheight * cr / 6), Stroke = brosse, StrokeThickness = 1 };
                    RatioCanvas.Children.Add(gline);
                }
                Line scope = new Line() { X1 = 20 + (x * 20), X2 = 20 + (x * 20), Y1 = graphheight - (graphheight * minR / 6), Y2 = graphheight - (graphheight * maxR / 6), Stroke = Brushes.SaddleBrown, StrokeThickness = 2 };
                RatioCanvas.Children.Add(scope);
            }

            ListBox box = new ListBox() { Width = 140, Height = 500 };
            Canvas.SetLeft(box, 90);
            Canvas.SetTop(box, 20);
            RatioCanvas.Children.Add(box);
            foreach (Gearing g in gearings)
            {
                TextBlock tbk = new TextBlock() { Text = $"{g.ChainringIndex}-{g.SprocketIndex} = {g.Ratio.ToString("0.00")} = {(g.Ratio * wheelCircumference).ToString("0")}mm", Foreground = Brushes.DarkGreen };
                //if (g.IsCrossover(crings.Count, sproks.Count)) { tbk.Foreground = Brushes.Red; }
                if (!g.IsCrossover(crings.Count, sproks.Count)) { box.Items.Add(tbk); }
                // box.Items.Add(tbk);
            }
        }

        private double ToothRatio(int chainringTeeth, int sprocketTeeth)
        {
            double cr = (double)chainringTeeth;
            return cr / sprocketTeeth;
        }

        private void JhCircumfTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(JhCircumfTextBox.Text, out int cmf))
            {
                _bikes[0].WheelCircumference = cmf;
                JhCircumfTextBlock.Text = cmf.ToString();
            }
            else
            {
                _bikes[0].WheelCircumference = 1000;
                JhCircumfTextBlock.Text = "Failed";
            }
        }

        private void RsCircumfTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RsCircumfTextBox.Text, out int cmf))
            {
                _bikes[1].WheelCircumference = cmf;
                RsCircumfTextBlock.Text = cmf.ToString();
            }
            else
            {
                _bikes[1].WheelCircumference = 1000;
                RsCircumfTextBlock.Text = "Failed";
            }
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
    }
}
