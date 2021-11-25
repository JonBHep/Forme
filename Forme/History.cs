using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class VeloHistory
    {

        public enum TripType { Walk, Cycle };

        //private readonly List<GymVisit> _gymVisits = new List<GymVisit>();
        private readonly List<Balade> _trips = new List<Balade>();
        private readonly string _veldatafile;
        private RollingMonthStats _rollingMonthStats;
        private int _outingsV = 0;
        private double _averageKmPerTripV = 0;
        private double _averageKmPerDayV = 0;
        private int _totalDaysV = 0;
        private double _maxTripKmV = 0;
        private double _totalKmV = 0;

        private int _outingsW = 0;
        private double _averageKmPerTripW = 0;
        private double _averageKmPerDayW = 0;
        private int _totalDaysW = 0;
        private double _maxTripKmW = 0;
        private double _totalKmW = 0;

        private DateTime _dateFirstV = DateTime.MaxValue;
        private DateTime _dateFirstW = DateTime.MaxValue;
        public static System.Windows.Media.Brush BrushEasy { get { return System.Windows.Media.Brushes.DarkSeaGreen; } }
        public static System.Windows.Media.Brush BrushIntermediate { get { return new System.Windows.Media.SolidColorBrush(ColourMix(System.Windows.Media.Colors.DarkSeaGreen, System.Windows.Media.Colors.LightCoral, 0.5)); } }
        public static System.Windows.Media.Brush BrushHard { get { return System.Windows.Media.Brushes.LightCoral; } }
        public static System.Windows.Media.Brush BrushWalk { get { return System.Windows.Media.Brushes.White; } }

        public VeloHistory(string vfile)
        {
            // get data file path
            _veldatafile = vfile;
            string VDatafile = AppManager.DataPath;
            VDatafile = System.IO.Path.Combine(VDatafile, _veldatafile);
            if (System.IO.File.Exists(VDatafile))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(VDatafile))
                {
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        Balade b = new Balade(s);
                        _trips.Add(b);
                    }
                }
            }
            
            Recalculate();
        }

        public void SaveData()
        {
            // get data file path
            string VDatafile = AppManager.DataPath;
            VDatafile = System.IO.Path.Combine(VDatafile, _veldatafile);
            // backup existing data
            AppManager.CreateBackupDataFile(VDatafile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(_veldatafile), MinimumDaysToKeep: 40, MinimumFilesToKeep: 4);

            // write new data
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(VDatafile))
            {
                foreach (Balade b in _trips)
                {
                    sw.WriteLine(b.Specification);
                }
            }
        }

        public int TripCountCycling
        {
            get
            {
                int c = 0;
                foreach (Balade b in _trips)
                {
                    if (b.Kind == TripType.Cycle) { c++; }
                }
                return c;
            }
        }

        public int TripCountPied
        {
            get
            {
                int c = 0;
                foreach (Balade b in _trips)
                {
                    if (b.Kind == TripType.Walk) { c++; }
                }
                return c;
            }
        }

        private void Recalculate()
        {
            _outingsV = 0;
            _maxTripKmV = 0;
            _totalKmV = 0;

            _outingsW = 0;
            _maxTripKmW = 0;
            _totalKmW = 0;

            if (_trips.Count < 1) { return; }

            _rollingMonthStats = new RollingMonthStats(NotionalToday);

            _trips.Sort();
            int z = 0;
            foreach (Balade b in _trips)
            {
                _rollingMonthStats.ConsiderRide(b, z);
                double j = b.RideKm;
                if (b.Kind == TripType.Walk)
                {
                    if (j > 0)
                    {
                        _outingsW++;
                        _maxTripKmW = Math.Max(_maxTripKmW, j);
                        _totalKmW += j;
                    }
                    if (_outingsW > 0) { _averageKmPerTripW = (_totalKmW / _outingsW); }
                }
                else
                {
                    if (j > 0)
                    {
                        _outingsV++;
                        _maxTripKmV = Math.Max(_maxTripKmV, j);
                        _totalKmV += j;
                    }
                    if (_outingsV > 0) { _averageKmPerTripV = (_totalKmV / _outingsV); }
                }
                z++;
            }

            _dateFirstV = _dateFirstW = DateTime.MaxValue;
            foreach (Balade b in _trips)
            {
                if (b.Kind == TripType.Walk)
                {
                    if (_dateFirstW > b.RideDate) { _dateFirstW = b.RideDate; }
                }
                else
                {
                    if (_dateFirstV > b.RideDate) { _dateFirstV = b.RideDate; }
                }
            }
            TimeSpan t = NotionalToday - _dateFirstV;
            double DaysSpan = 1 + t.TotalDays;
            _totalDaysV = (int)DaysSpan;
            _averageKmPerDayV = _totalKmV / DaysSpan;

            t = NotionalToday - _dateFirstW;
            DaysSpan = 1 + t.TotalDays;
            _totalDaysW = (int)DaysSpan;
            _averageKmPerDayW = _totalKmW / DaysSpan;
        }

        public double PeriodDistance(DateTime first, DateTime last, TripType kind)
        {
            double accum = 0;
            foreach (Balade b in _trips)
            {
                if (b.Kind == kind)
                {
                    if ((b.RideDate >= first) && (b.RideDate <= last))
                    {
                        accum += b.RideKm;
                    }
                }
            }
            return accum;
        }
        public int PeriodTripCount(DateTime first, DateTime last, TripType kind)
        {
            int counted = 0;
            foreach (Balade b in _trips)
            {
                if (b.Kind == kind)
                {
                    if ((b.RideDate >= first) && (b.RideDate <= last)) { counted++; }
                }
            }
            return counted;
        }
        public RollingMonthStats RollingMonth { get { return _rollingMonthStats; } }

        public double RollingYearMonthlyMeanKm(int annee, int mois, VeloHistory.TripType kind)
        {
            DateTime g = new DateTime(annee, mois, 1);
            g = g.AddMonths(1);
            g = g.AddDays(-1);
            DateTime yearEnd = g;
            DateTime yearStart = yearEnd.AddYears(-1);
            double yearKm = PeriodDistance(yearStart.AddDays(1), yearEnd, kind);
            return yearKm / 12;
        }

        public double RollingYearTotalKm(int annee, int mois, VeloHistory.TripType kind)
        {
            DateTime g = new DateTime(annee, mois, 1);
            g = g.AddMonths(1);
            g = g.AddDays(-1);
            DateTime yearEnd = g;
            DateTime yearStart = yearEnd.AddYears(-1);
            return PeriodDistance(yearStart.AddDays(1), yearEnd, kind);
        }
        public DateTime HistoryFirstDate { get { if (_dateFirstV > _dateFirstW) { return _dateFirstW; } else { return _dateFirstV; } } }

        public int DistanceRanking(double distance, TripType kind)
        {
            int rnk = 1;
            foreach (Balade b in _trips)
            {
                if (b.Kind == kind)
                {
                    if (b.RideKm > distance) { rnk++; }
                }
            }
            return rnk;
        }

        public void AddTrip(Balade newBallade)
        {
            _trips.Add(newBallade);
            Recalculate();
        }

        //public void RemoveTripNumber(int index)
        //{
        //    _trips.RemoveAt(index);
        //    Recalculate();
        //}
        public void RemoveTripOnDate(DateTime d)
        {
            int z = -1;
            for (int i=0;i<_trips.Count; i++)
            {
                if (_trips[i].RideDate.Date == d.Date) { z = i; break; }
            }
            if (z >= 0) { _trips.RemoveAt(z); }
            Recalculate();
        }
        public double MaximumTripKmVelo { get { return _maxTripKmV; } }

        public double AverageTripKmVelo { get { return _averageKmPerTripV; } }

        public double AverageDailyKmVelo { get { return _averageKmPerDayV; } }

        public double Average4WeeklyKmVelo { get { return _averageKmPerDayV * 28; } }

        public double TotalDistanceKmV { get { return _totalKmV; } }

        public int TotalDaysCycling { get { return _totalDaysV; } }

        public double MaximumTripKmPied { get { return _maxTripKmW; } }

        public double AverageTripKmPied { get { return _averageKmPerTripW; } }

        public double AverageDailyKmPied { get { return _averageKmPerDayW; } }

        public double Average4WeeklyKmPied { get { return _averageKmPerDayW * 28; } }

        public double TotalDistanceKmW { get { return _totalKmW; } }

        public int TotalDaysPied { get { return _totalDaysW; } }

        public static double MilesFromKm(double km) { return km * 0.621371192; }

        public static double KmFromMiles(double miles) { return miles * 1.609344; }

        public DateTime NotionalToday
        {
            get
            {
                DateTime nt = DateTime.Today;
                // If there is no ride recorded for today, take stats up to yesterday rather than today, as we don't want to reduce the averages when there could still be a trip recorded today. We assume there isn't still a trip to be recorded for yesterday!
                if (_trips.Count < 1) { return nt; }
                if (!_trips[_trips.Count - 1].RideDate.Equals(DateTime.Today))
                {
                    nt = DateTime.Today.AddDays(-1);
                }
                return nt;
            }
        }

        public bool TripExistsForDate(DateTime q)
        {
            DateTime qd = q.Date;
            bool rv = false;
            foreach (Balade r in _trips)
            {
                if (r.RideDate.Equals(qd)) { rv = true; }
            }
            return rv;
        }

        public Balade TripOnDate(DateTime q)
        {
            DateTime qd = q.Date;
            Balade rv = null;
            foreach (Balade r in _trips)
            {
                if (r.RideDate.Equals(qd)) { rv = r; }
            }
            return rv;
        }

        public static string Ordinal(int r)
        {
            string v = r.ToString();
            int tens = r % 100;
            int units = tens % 10;
            tens /= 10;
            switch (tens)
            {
                case 1:
                    {
                        v += "th";
                        break;
                    }
                default:
                    {
                        switch (units)
                        {
                            case 1:
                                {
                                    v += "st";
                                    break;
                                }
                            case 2:
                                {
                                    v += "nd";
                                    break;
                                }
                            case 3:
                                {
                                    v += "rd";
                                    break;
                                }
                            default:
                                {
                                    v += "th";
                                    break;
                                }
                        }
                        break;
                    }
            }
            return v;
        }

        /// <summary>
        /// Returns a System.Windows.Media.Color that is a mix of the first and second given Colors in the specified proportion
        /// </summary>
        /// <param name="first">First colour</param>
        /// <param name="second">Second colour</param>
        /// <param name="mix">Proportion of second colour to mix with first colour (0 = 100% first colour, 1 = 100% second colour)</param>
        /// <returns></returns>
        public static System.Windows.Media.Color ColourMix(System.Windows.Media.Color first, System.Windows.Media.Color second, double mix)
        {
            byte redF = first.R;
            byte grnF = first.G;
            byte bluF = first.B;
            byte redS = second.R;
            byte grnS = second.G;
            byte bluS = second.B;
            double fMix = 1 - mix;
            double redMix = (fMix * redF) + (mix * redS);
            double grnMix = (fMix * grnF) + (mix * grnS);
            double bluMix = (fMix * bluF) + (mix * bluS);
            byte redM = (byte)(redMix + 0.5); // Adding 0.5 because (int) rounds down
            byte grnM = (byte)(grnMix + 0.5);
            byte bluM = (byte)(bluMix + 0.5);
            return System.Windows.Media.Color.FromRgb(redM, grnM, bluM);
        }

        public List<string> GroupList
        {
            get
            {
                List<string> groups = new List<string>();
                foreach (Balade r in _trips)
                {
                    if (!string.IsNullOrWhiteSpace(r.RideGroup))
                    {
                        if (!groups.Contains(r.RideGroup)) { groups.Add(r.RideGroup); }
                    }
                }
                groups.Sort();
                return groups;
            }
        }

        public int TripCountV { get { return _outingsV; } }
        public int TripCountW { get { return _outingsW; } }
        public int TripCountAll { get { return _trips.Count; } }
        public List<Balade> TripList { get { return _trips; } }
    }
}
