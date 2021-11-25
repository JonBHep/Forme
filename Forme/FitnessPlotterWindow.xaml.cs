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
    /// Interaction logic for FitnessPlotterWindow.xaml
    /// </summary>
    public partial class FitnessPlotterWindow : Window
    {
        public FitnessPlotterWindow(PersonProfile core)
        {
            InitializeComponent();
            _coreData = core;
        }

        private PersonProfile _coreData;
        private double _horizontalDateIncrement = 1.5; // sets the horizontal pixels per day

        private struct GraphPoint
        {
            public double x { get; }
            public double y { get; }
            public GraphPoint(double xval, double yval)
            { x = xval; y = yval; }
        }
        
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonPlotWeight_Click(object sender, RoutedEventArgs e)
        { PlotWeight(); }

        public void PlotWeight()
        {
            StatisticsBorder.Visibility = Visibility.Visible;

            DateTime earliestDate = DateTime.MaxValue;

            double newmeasuredweight = 0;
            double minmeasuredweight = double.MaxValue;
            double maxmeasuredweight = double.MinValue;

            DateTime newdate = DateTime.Today;
            DateTime mindate = DateTime.Today;
            DateTime maxdate = DateTime.Today;

            foreach (PersonProfile.Weight wt in _coreData.WeightReadings)
            {
                if (wt.wgtDate.CompareTo(earliestDate) < 0) { earliestDate = wt.wgtDate; }
                newmeasuredweight = wt.wgtKilograms;
                newdate = wt.wgtDate;
                if (wt.wgtKilograms < minmeasuredweight) { minmeasuredweight = wt.wgtKilograms; mindate = wt.wgtDate; }
                if (wt.wgtKilograms > maxmeasuredweight) { maxmeasuredweight = wt.wgtKilograms; maxdate = wt.wgtDate; }
            }

            NewDateTB.Text = newdate.ToShortDateString();
            MinDateTB.Text = mindate.ToShortDateString();
            MaxDateTB.Text = maxdate.ToShortDateString();
            NewValueTB.Text = $"{newmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(newmeasuredweight)} = BMI {BodyStatics.BmiOf(newmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";
            MinValueTB.Text = $"{minmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(minmeasuredweight)} = BMI {BodyStatics.BmiOf(minmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";
            MaxValueTB.Text = $"{maxmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(maxmeasuredweight)} = BMI {BodyStatics.BmiOf(maxmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";

            // Create a list of points
            List<GraphPoint> pointList = new List<GraphPoint>();
            foreach (PersonProfile.Weight wt in _coreData.WeightReadings)
            {
                TimeSpan ts = wt.wgtDate - earliestDate; // elapsed time from base date
                GraphPoint gp = new GraphPoint(ts.TotalDays, wt.wgtKilograms);
                pointList.Add(gp);
            }

            // find min and max values on each axis
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            foreach (GraphPoint gp in pointList)
            {
                if (gp.x < minX) { minX = gp.x; }
                if (gp.x > maxX) { maxX = gp.x; }
                if (gp.y < minY) { minY = gp.y; }
                if (gp.y > maxY) { maxY = gp.y; }
            }

            // ensure my ideal weight's upper and lower limits are within these limits
            double tgtLo = _coreData.IdealWeightLowerLimit;
            if (tgtLo < minY) { minY = tgtLo; }
            if (tgtLo > maxY) { maxY = tgtLo; }
            double tgtHi = _coreData.IdealWeightHigherLimit;
            if (tgtHi < minY) { minY = tgtHi; }
            if (tgtHi > maxY) { maxY = tgtHi; }
            // add some leeway below minimum and above maximum
            minY -= 0.8;
            maxY += 0.8;

            double valueSpanHorizontal = maxX - minX;
            double valueSpanVertical = maxY - minY;

            canvasGraph.Children.Clear();
            double marginHorizontal = 12;
            double marginVertical = 12;

            if (canvasGraph.ActualHeight == 0) { return; }

            double physicalSpanFitToScreen = canvasGraph.ActualWidth - (2 * marginHorizontal);
            double physicalSpanSpaceOutValues = _horizontalDateIncrement * valueSpanHorizontal;
            double physicalSpanHorizontal;
            if (physicalSpanSpaceOutValues > physicalSpanFitToScreen)
            {
                physicalSpanHorizontal = physicalSpanSpaceOutValues;
                canvasGraph.Width = physicalSpanHorizontal + (2 * marginHorizontal);
            }
            else
            {
                physicalSpanHorizontal = physicalSpanFitToScreen;
            }
            double physicalSpanVertical = canvasGraph.ActualHeight - (2 * marginVertical);

            // test graph area
            Rectangle outliner = new Rectangle
            {
                Width = physicalSpanHorizontal,
                Height = physicalSpanVertical,
                Stroke = new SolidColorBrush(Colors.Gray),
                Fill = new SolidColorBrush(Colors.WhiteSmoke)
            };
            Canvas.SetLeft(outliner, marginHorizontal);
            Canvas.SetTop(outliner, marginVertical);
            canvasGraph.Children.Add(outliner);

            // Draw vertical lines and labels for each January 1st
            int year = earliestDate.Year;
            int thisYear = DateTime.Today.Year;
            while (year < thisYear)
            {
                year++;
                DateTime newyearsday = new DateTime(year, 1, 1);
                TimeSpan ts = newyearsday - earliestDate; // elapsed time from base date
                Line ln = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1,
                    X1 = marginHorizontal + (ts.TotalDays - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                    Y1 = marginVertical + physicalSpanVertical
                };
                ln.X2 = ln.X1;
                ln.Y2 = marginVertical;
                canvasGraph.Children.Add(ln);

                Border br = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.SaddleBrown),
                    CornerRadius = new CornerRadius(4),
                    Background = new SolidColorBrush(Colors.SeaShell)
                };
                TextBlock tb = new TextBlock();
                Canvas.SetLeft(br, ln.X1 + 2);
                Canvas.SetTop(br, marginVertical + 2);
                tb.Background = new SolidColorBrush(Colors.SeaShell);
                tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
                tb.Margin = new Thickness(4);
                tb.Padding = new Thickness(4);
                tb.Text = year.ToString();
                br.Child = tb;
                canvasGraph.Children.Add(br);
            }

            // add horizontal line and label to indicate my minimum recorded weight
            Line lineMin = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 1,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((minmeasuredweight - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lineMin.Y2 = lineMin.Y1;
            lineMin.StrokeDashArray = new DoubleCollection { 4, 8 };
            canvasGraph.Children.Add(lineMin);
            // add label to indicate my minimum recorded weight
            Border borderMin = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkGreen),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.LightCyan)
            };
            TextBlock textblockMin = new TextBlock()
            {
                Background = new SolidColorBrush(Colors.LightCyan),
                Foreground = new SolidColorBrush(Colors.DarkGreen),
                Margin = new Thickness(2),
                Padding = new Thickness(4, 2, 4, 2),
                Text = $"Minumum recorded weight {minmeasuredweight.ToString("#0.0")} kg"
            };
            borderMin.Child = textblockMin;
            Canvas.SetRight(borderMin, 200);
            Canvas.SetTop(borderMin, lineMin.Y1 - 14);
            canvasGraph.Children.Add(borderMin);

            // draw horizontal lines to indicate my ideal weight/BMI - lower and upper limits and target

            Line lineLoIdeal = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 1,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((tgtLo - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lineLoIdeal.Y2 = lineLoIdeal.Y1;
            canvasGraph.Children.Add(lineLoIdeal);
            // add label to indicate my target weight/BMI
            Border borderLoIdeal = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Green),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.MintCream)
            };
            TextBlock textblockLoIdeal = new TextBlock()
            {
                Background = new SolidColorBrush(Colors.MintCream),
                Foreground = new SolidColorBrush(Colors.Green),
                Margin = new Thickness(2),
                Padding = new Thickness(4, 2, 4, 2),
                Text = $"Ideal weight minimum {tgtLo.ToString("#0.0")} kg = BMI {BodyStatics.BmiOf(tgtLo, _coreData.HeightInMetres).ToString("0.0")}"
            };
            Canvas.SetRight(borderLoIdeal, 200);
            Canvas.SetTop(borderLoIdeal, lineLoIdeal.Y1 - 14);
            borderLoIdeal.Child = textblockLoIdeal;
            canvasGraph.Children.Add(borderLoIdeal);

            Line lineHiIdeal = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 1,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((tgtHi - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lineHiIdeal.Y2 = lineHiIdeal.Y1;
            canvasGraph.Children.Add(lineHiIdeal);
            // add label to indicate my target weight/BMI
            Border borderHiIdeal = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Green),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.MintCream)
            };
            TextBlock textblockHiIdeal = new TextBlock()
            {
                Background = new SolidColorBrush(Colors.MintCream),
                Foreground = new SolidColorBrush(Colors.Green),
                Margin = new Thickness(2),
                Padding = new Thickness(4, 2, 4, 2),
                Text = $"Ideal weight maximum {tgtHi.ToString("#0.0")} kg = BMI {BodyStatics.BmiOf(tgtHi, _coreData.HeightInMetres).ToString("0.0")}"
            };
            Canvas.SetRight(borderHiIdeal, 200);
            Canvas.SetTop(borderHiIdeal, lineHiIdeal.Y1 - 14);
            borderHiIdeal.Child = textblockHiIdeal;
            canvasGraph.Children.Add(borderHiIdeal);

            double tgtMd = tgtLo + ((tgtHi - tgtLo) / 2);
            Line lineMdIdeal = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 2,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((tgtMd - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lineMdIdeal.Y2 = lineMdIdeal.Y1;
            canvasGraph.Children.Add(lineMdIdeal);
            // add label to indicate my target weight/BMI
            Border borderMdIdeal = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Green),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.MintCream)
            };
            TextBlock textblockMdIdeal = new TextBlock()
            {
                Background = new SolidColorBrush(Colors.MintCream),
                Foreground = new SolidColorBrush(Colors.Green),
                Margin = new Thickness(2),
                Padding = new Thickness(4, 2, 4, 2),
                Text = $"Ideal weight mean {tgtMd.ToString("#0.0")} kg = BMI {BodyStatics.BmiOf(tgtMd, _coreData.HeightInMetres).ToString("0.0")}"
            };
            Canvas.SetRight(borderMdIdeal, 200);
            Canvas.SetTop(borderMdIdeal, lineMdIdeal.Y1 - 14);
            borderMdIdeal.Child = textblockMdIdeal;
            canvasGraph.Children.Add(borderMdIdeal);

            // Plot line graph
            double lastx = -11267;
            double lasty = 0;
            foreach (GraphPoint gp in pointList)
            {
                if (lastx != -11267)
                {
                    Line ln = new Line
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        StrokeThickness = 2,
                        X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y1 = marginVertical + physicalSpanVertical - ((lasty - minY) * (physicalSpanVertical / valueSpanVertical)),
                        X2 = marginHorizontal + (gp.x - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y2 = marginVertical + physicalSpanVertical - ((gp.y - minY) * (physicalSpanVertical / valueSpanVertical))
                    };
                    canvasGraph.Children.Add(ln);
                }
                lastx = gp.x;
                lasty = gp.y;
            }
            GraphScrollViewer.ScrollToRightEnd();
        }

        private void buttonPlotWaist_Click(object sender, RoutedEventArgs e)
        { PlotWaist(); }

        public void PlotWaist()
        {
            StatisticsBorder.Visibility = Visibility.Visible;

            DateTime earliestDate = DateTime.MaxValue;

            double newmeasuredwaist = 0;
            double minmeasuredwaist = double.MaxValue;
            double maxmeasuredwaist = double.MinValue;

            DateTime newdate = DateTime.Today;
            DateTime mindate = DateTime.Today;
            DateTime maxdate = DateTime.Today;

            foreach (PersonProfile.Waist wt in _coreData.WaistReadings)
            {
                if (wt.wstDate.CompareTo(earliestDate) < 0) { earliestDate = wt.wstDate; }
                newmeasuredwaist = wt.wstCentimetres;
                newdate = wt.wstDate;
                if (wt.wstCentimetres < minmeasuredwaist) { minmeasuredwaist = wt.wstCentimetres; mindate = wt.wstDate; }
                if (wt.wstCentimetres > maxmeasuredwaist) { maxmeasuredwaist = wt.wstCentimetres; maxdate = wt.wstDate; }
            }

            NewDateTB.Text = newdate.ToShortDateString();
            MinDateTB.Text = mindate.ToShortDateString();
            MaxDateTB.Text = maxdate.ToShortDateString();
            NewValueTB.Text = $"{newmeasuredwaist.ToString("0.0")} cm";
            MinValueTB.Text = $"{minmeasuredwaist.ToString("0.0")} cm";
            MaxValueTB.Text = $"{maxmeasuredwaist.ToString("0.0")} cm";

            // Create a list of points
            List<GraphPoint> pointList = new List<GraphPoint>();
            foreach (PersonProfile.Waist wt in _coreData.WaistReadings)
            {
                TimeSpan ts = wt.wstDate - earliestDate; // elapsed time from base date
                GraphPoint gp = new GraphPoint(ts.TotalDays, wt.wstCentimetres);
                pointList.Add(gp);
            }

            // find min and max values on each axis
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            foreach (GraphPoint gp in pointList)
            {
                if (gp.x < minX) { minX = gp.x; }
                if (gp.x > maxX) { maxX = gp.x; }
                if (gp.y < minY) { minY = gp.y; }
                if (gp.y > maxY) { maxY = gp.y; }
            }
            double minmeasuredweight = minY;
            // ensure my target waist measurement is within these limits
            double tgt = BodyStatics.TargetWaist;
            if (tgt < minY) { minY = tgt; }
            if (tgt > maxY) { maxY = tgt; }
            // add some leeway below minimum and above maximum on Y axis
            minY -= 0.8;
            maxY += 0.8;

            double valueSpanHorizontal = maxX - minX;
            double valueSpanVertical = maxY - minY;

            canvasGraph.Children.Clear();
            double marginHorizontal = 12;
            double marginVertical = 12;

            if (canvasGraph.ActualHeight == 0) { return; }
            canvasGraph.Width = _horizontalDateIncrement * valueSpanHorizontal + (2 * marginHorizontal);
            double physicalSpanHorizontal = _horizontalDateIncrement * valueSpanHorizontal;
            double physicalSpanVertical = canvasGraph.ActualHeight - (2 * marginVertical);

            // test graph area
            Rectangle outliner = new Rectangle
            {
                Width = physicalSpanHorizontal,
                Height = physicalSpanVertical,
                Stroke = new SolidColorBrush(Colors.Gray),
                Fill = new SolidColorBrush(Colors.WhiteSmoke)
            };
            Canvas.SetLeft(outliner, marginHorizontal);
            Canvas.SetTop(outliner, marginVertical);
            canvasGraph.Children.Add(outliner);

            // Draw vertical lines and labels for each January 1st
            int year = earliestDate.Year;
            while (year < DateTime.Today.Year)
            {
                year++;
                DateTime newyearsday = new DateTime(year, 1, 1);
                TimeSpan ts = newyearsday - earliestDate; // elapsed time from base date
                Line ln = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1,
                    X1 = marginHorizontal + (ts.TotalDays - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                    Y1 = marginVertical + physicalSpanVertical
                };
                ln.X2 = ln.X1;
                ln.Y2 = marginVertical;
                canvasGraph.Children.Add(ln);

                Border br = new Border()
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.SaddleBrown),
                    CornerRadius = new CornerRadius(4),
                    Background = new SolidColorBrush(Colors.SeaShell)
                };
                TextBlock tb = new TextBlock();
                Canvas.SetLeft(br, ln.X1 + 2);
                Canvas.SetTop(br, marginVertical + 2);
                tb.Background = new SolidColorBrush(Colors.SeaShell);
                tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
                tb.Margin = new Thickness(4);
                tb.Padding = new Thickness(4);
                tb.Text = year.ToString();
                br.Child = tb;
                canvasGraph.Children.Add(br);
            }

            // draw horizontal line to indicate my target waist measurement
            Line lntarget = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 2,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((tgt - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lntarget.Y2 = lntarget.Y1;
            canvasGraph.Children.Add(lntarget);
            // add label to indicate my target waist measurement
            Border brtarget = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Green),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.MintCream)
            };
            TextBlock tbtarget = new TextBlock();
            Canvas.SetRight(brtarget, 200);
            Canvas.SetTop(brtarget, lntarget.Y1 - 14);
            tbtarget.Background = new SolidColorBrush(Colors.MintCream);
            tbtarget.Foreground = new SolidColorBrush(Colors.Green);
            tbtarget.Margin = new Thickness(2);
            tbtarget.Padding = new Thickness(4, 2, 4, 2);
            tbtarget.Text = $"Target waist measurement {tgt.ToString("0.0")} cm";
            brtarget.Child = tbtarget;
            canvasGraph.Children.Add(brtarget);

            // draw horizontal line to indicate my minimum recorded waist
            Line lnmin = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 1,
                X1 = marginHorizontal + physicalSpanHorizontal,
                Y1 = marginVertical + physicalSpanVertical - ((minmeasuredweight - minY) * (physicalSpanVertical / valueSpanVertical)),
                X2 = marginHorizontal
            };
            lnmin.Y2 = lnmin.Y1;
            lnmin.StrokeDashArray = new DoubleCollection { 4, 8 };
            canvasGraph.Children.Add(lnmin);
            // add label to indicate my minimum recorded waist
            Border brmin = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkGreen),
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.LightCyan)
            };
            TextBlock tbmin = new TextBlock()
            {
                Background = new SolidColorBrush(Colors.LightCyan),
                Foreground = new SolidColorBrush(Colors.DarkGreen),
                Margin = new Thickness(2),
                Padding = new Thickness(4, 2, 4, 2),
                Text = $"Minumum recorded waist measurement {minmeasuredweight.ToString("0.0")}"
            };
            Canvas.SetRight(brmin, 200);
            Canvas.SetTop(brmin, lnmin.Y1 - 14);
            brmin.Child = tbmin;
            canvasGraph.Children.Add(brmin);

            // Plot line graph
            double lastx = -11267;
            double lasty = 0;
            foreach (GraphPoint gp in pointList)
            {
                if (lastx != -11267)
                {
                    Line ln = new Line
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        StrokeThickness = 2,
                        X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y1 = marginVertical + physicalSpanVertical - ((lasty - minY) * (physicalSpanVertical / valueSpanVertical)),
                        X2 = marginHorizontal + (gp.x - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y2 = marginVertical + physicalSpanVertical - ((gp.y - minY) * (physicalSpanVertical / valueSpanVertical))
                    };
                    canvasGraph.Children.Add(ln);
                }
                lastx = gp.x;
                lasty = gp.y;
            }
            GraphScrollViewer.ScrollToRightEnd();
        }

        private void buttonPlotBloodPressure_Click(object sender, RoutedEventArgs e)
        { PlotBloodPressure(); }

        public void PlotBloodPressure()
        {
            StatisticsBorder.Visibility = Visibility.Hidden;

            DateTime earliestDate = DateTime.MaxValue;
            DateTime latestDate = DateTime.MinValue;
            foreach (PersonProfile.BloodPressure wt in _coreData.BloodPressureReadings)
            {
                if (wt.BprDate.CompareTo(earliestDate) < 0) { earliestDate = wt.BprDate; }
                if (wt.BprDate.CompareTo(latestDate) > 0) { latestDate = wt.BprDate; }
            }

            // Create lists of points
            List<GraphPoint> syspointList = new List<GraphPoint>();
            List<GraphPoint> diapointList = new List<GraphPoint>();
            List<GraphPoint> pulpointList = new List<GraphPoint>();
            foreach (PersonProfile.BloodPressure wt in _coreData.BloodPressureReadings)
            {
                TimeSpan ts = wt.BprDate - earliestDate; // elapsed time from base date
                GraphPoint gpd = new GraphPoint(ts.TotalDays, wt.BprDiastolic);
                diapointList.Add(gpd);
                GraphPoint gps = new GraphPoint(ts.TotalDays, wt.BprSystolic);
                syspointList.Add(gps);
                GraphPoint gpp = new GraphPoint(ts.TotalDays, wt.BprPulse);
                pulpointList.Add(gpp);
            }

            // find min and max values on each axis
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            foreach (GraphPoint gp in diapointList)
            {
                if (gp.x < minX) { minX = gp.x; }
                if (gp.x > maxX) { maxX = gp.x; }
                if (gp.y < minY) { minY = gp.y; }
                if (gp.y > maxY) { maxY = gp.y; }
            }
            foreach (GraphPoint gp in syspointList)
            {
                if (gp.y < minY) { minY = gp.y; }
                if (gp.y > maxY) { maxY = gp.y; }
            }
            foreach (GraphPoint gp in pulpointList)
            {
                if (gp.y < minY) { minY = gp.y; }
                if (gp.y > maxY) { maxY = gp.y; }
            }
            // ensure ideal ranges are within chart
            if (139 > maxY) { maxY = 139; }
            if (85 < minY) { minY = 85; }
            // add some leeway below minimum and above maximum
            minY -= 0.8;
            maxY += 0.8;

            double valueSpanHorizontal = maxX - minX;
            double valueSpanVertical = maxY - minY;

            canvasGraph.Children.Clear();
            double marginHorizontal = 12;
            double marginVertical = 12;

            if (canvasGraph.ActualHeight == 0) { return; }
            canvasGraph.Width = _horizontalDateIncrement * valueSpanHorizontal + (2 * marginHorizontal);
            double physicalSpanHorizontal = _horizontalDateIncrement * valueSpanHorizontal;
            if (physicalSpanHorizontal < 100) { physicalSpanHorizontal = 100; }
            double physicalSpanVertical = canvasGraph.ActualHeight - (2 * marginVertical);

            // test graph area
            Rectangle outliner = new Rectangle
            {
                Width = physicalSpanHorizontal,
                Height = physicalSpanVertical,
                Stroke = new SolidColorBrush(Colors.Gray),
                Fill = new SolidColorBrush(Colors.WhiteSmoke)
            };
            Canvas.SetLeft(outliner, marginHorizontal);
            Canvas.SetTop(outliner, marginVertical);
            canvasGraph.Children.Add(outliner);

            // Indicate ideal ranges 85-89 and 130-139
            Rectangle rct = new Rectangle
            {
                Stroke = System.Windows.Media.Brushes.PaleGreen,
                Fill = System.Windows.Media.Brushes.PaleGreen,
                Height = 4 * (physicalSpanVertical / valueSpanVertical),
                Width = physicalSpanHorizontal - 2
            };
            Canvas.SetTop(rct, marginVertical + physicalSpanVertical - ((89 - minY) * (physicalSpanVertical / valueSpanVertical)));
            Canvas.SetLeft(rct, marginHorizontal + 1);
            canvasGraph.Children.Add(rct);

            rct = new Rectangle
            {
                Stroke = System.Windows.Media.Brushes.PaleGreen,
                Fill = System.Windows.Media.Brushes.PaleGreen,
                Height = 9 * (physicalSpanVertical / valueSpanVertical),
                Width = physicalSpanHorizontal - 2
            };
            Canvas.SetTop(rct, marginVertical + physicalSpanVertical - ((139 - minY) * (physicalSpanVertical / valueSpanVertical)));
            Canvas.SetLeft(rct, marginHorizontal + 1);
            canvasGraph.Children.Add(rct);

            // Draw vertical lines and labels for each January 1st
            int year = earliestDate.Year;
            while (year < latestDate.Year)
            {
                year++;
                DateTime newyearsday = new DateTime(year, 1, 1);
                TimeSpan ts = newyearsday - earliestDate; // elapsed time from base date
                Line ln = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1,
                    X1 = marginHorizontal + (ts.TotalDays - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                    Y1 = marginVertical + physicalSpanVertical
                };
                ln.X2 = ln.X1;
                ln.Y2 = marginVertical;
                canvasGraph.Children.Add(ln);

                Border br = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.SaddleBrown),
                    CornerRadius = new CornerRadius(4),
                    Background = new SolidColorBrush(Colors.SeaShell)
                };
                TextBlock tb = new TextBlock();
                Canvas.SetLeft(br, ln.X1 + 2);
                Canvas.SetTop(br, marginVertical + 2);
                tb.Background = new SolidColorBrush(Colors.SeaShell);
                tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
                tb.Margin = new Thickness(4);
                tb.Padding = new Thickness(4);
                tb.Text = year.ToString();
                br.Child = tb;
                canvasGraph.Children.Add(br);
            }

            // Plot line graph
            double lastx = -11267;
            double lasty = 0;
            foreach (GraphPoint gp in diapointList)
            {
                if (lastx != -11267)
                {
                    Line ln = new Line
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        StrokeThickness = 2,
                        X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y1 = marginVertical + physicalSpanVertical - ((lasty - minY) * (physicalSpanVertical / valueSpanVertical)),
                        X2 = marginHorizontal + (gp.x - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y2 = marginVertical + physicalSpanVertical - ((gp.y - minY) * (physicalSpanVertical / valueSpanVertical))
                    };
                    canvasGraph.Children.Add(ln);
                }
                lastx = gp.x;
                lasty = gp.y;
            }
            // Plot line graph
            lastx = -11267;
            lasty = 0;
            foreach (GraphPoint gp in syspointList)
            {
                if (lastx != -11267)
                {
                    Line ln = new Line
                    {
                        Stroke = new SolidColorBrush(Colors.Green),
                        StrokeThickness = 2,
                        X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y1 = marginVertical + physicalSpanVertical - ((lasty - minY) * (physicalSpanVertical / valueSpanVertical)),
                        X2 = marginHorizontal + (gp.x - minX) * (physicalSpanHorizontal / valueSpanHorizontal),
                        Y2 = marginVertical + physicalSpanVertical - ((gp.y - minY) * (physicalSpanVertical / valueSpanVertical))
                    };
                    canvasGraph.Children.Add(ln);
                }
                lastx = gp.x;
                lasty = gp.y;
            }
            // Plot pulse rate as points
            foreach (GraphPoint gp in pulpointList)
            {
                Ellipse pt = new Ellipse()
                {
                    Width = 9,
                    Height = 9,
                    Fill = new SolidColorBrush(Colors.Lime),
                    Stroke = new SolidColorBrush(Colors.DarkGreen),
                    StrokeThickness = 1
                };
                Canvas.SetLeft(pt, marginHorizontal - 4 + (gp.x - minX) * (physicalSpanHorizontal / valueSpanHorizontal));
                Canvas.SetTop(pt, marginVertical - 4 + (physicalSpanVertical - ((gp.y - minY) * (physicalSpanVertical / valueSpanVertical))));
                canvasGraph.Children.Add(pt);
            }
            GraphScrollViewer.ScrollToRightEnd();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            StatisticsBorder.Visibility = Visibility.Hidden;
            double aw = OuterCanvas.ActualWidth;
            double ah = OuterCanvas.ActualHeight;
            GraphScrollViewer.Width = OuterCanvas.ActualWidth;
            GraphScrollViewer.Height = OuterCanvas.ActualHeight;
            //PlotWeight();
            //PlotWaist();
        }
    }
}
