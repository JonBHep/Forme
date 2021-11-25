using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{

    static class BodyStatics
    {
        public const double LowIdealBMI = 18.5;
        public const double HighIdealBMI = 25;

        public static double WeightAsPounds(double Kgs)
        {
            return Kgs / 0.45359237;
        }

        public static double HeightAsInches(double Mtrs)
        {
            double cm = Mtrs * 100;
            return cm / 2.54;
        }

        public static string WeightAsStonesAndPoundsString(double Kgs)
        {
            double lb = WeightAsPounds(Kgs);
            lb = Math.Round(lb);
            int lbs = (int)lb;
            int s = (int)Math.Floor(lbs / 14f);
            int p = lbs % 14;
            return s.ToString() + " stone " + p.ToString() + " lb";
        }

        public static string HeightAsFeetAndInchesString(double mtrs)
        {
            double inches = HeightAsInches(mtrs);
            int feet = (int)Math.Floor(inches / 12f);
            int usedinches = feet * 12;
            double ins = inches - usedinches;
            return $"{feet} ft {ins.ToString("0.0")} in";
        }

        public static double BmiOf(double Kg, double MyHeightInMetres)
        { return (float)(Kg / (Math.Pow(MyHeightInMetres, 2))); }

        private static double WeightInKgForBMI(double Bmi, double MyHeightInMetres)
        {
            return Bmi * Math.Pow(MyHeightInMetres, 2);
        }

        public static double IdealWeightLowerLimitKg(double HeightInMetres) { return WeightInKgForBMI(LowIdealBMI, HeightInMetres); }

        public static double IdealWeightHigherLimit(double HeightInMetres) { return WeightInKgForBMI(HighIdealBMI, HeightInMetres); }

        public static double TargetWaist { get { return 100; } }

        public static int CountDays(DateTime start, DateTime finish)
        {
            TimeSpan ts = finish.Subtract(start);
            return (int)ts.TotalDays;
        }

    }
}
