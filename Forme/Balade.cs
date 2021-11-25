using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class Balade : IComparable<Balade>
    {
        private readonly DateTime _jour;
        private readonly double _kilometresJ;
        private readonly string _caption;
        private readonly string _group;
        private readonly int _difficulty;
        private readonly VeloHistory.TripType _kind;

        public Balade(string specifier)
        {
            string[] s = specifier.Split("@".ToCharArray());
            long a = long.Parse(s[0]);
            DateTime dt = DateTime.FromBinary(a);
            _jour = dt.Date;
            _kilometresJ = double.Parse(s[1]);
            _caption = s[2];
            _group = s[3];
            _difficulty = int.Parse(s[4]);
            int k = int.Parse(s[5]);
            _kind = (VeloHistory.TripType)k;
        }

        public string Specification
        {
            get
            {
                return $"{_jour.Date.ToBinary()}@{_kilometresJ}@{_caption}@{_group}@{_difficulty}@{(int)_kind}";
            }
        }

        public Balade(DateTime dat, double kmJ, string cp, string gp, int diff, VeloHistory.TripType kind)
        {
            _kilometresJ = kmJ;
            _jour = dat;
            _caption = cp;
            _group = gp;
            _difficulty = diff;
            _kind = kind;
        }

        public string RideCaption { get { return _caption; } }

        public string RideGroup { get { return _group; } }

        public int Difficulty { get { return _difficulty; } }

        public DateTime RideDate { get { return _jour.Date; } }

        public double RideKm { get { return _kilometresJ; } }

        public string RideKmStringJbh { get { return (_kilometresJ > 0) ? _kilometresJ.ToString("0.00 km") : "-"; } }

        public VeloHistory.TripType Kind { get { return _kind; } }

        int IComparable<Balade>.CompareTo(Balade other)
        {
            return this._jour.CompareTo(other._jour);
        }

        public static string DifficultyCaption(int diff)
        {
            switch (diff)
            {
                case 1: { return "Easy & flat"; }
                case 3: { return "Difficult"; }
                default: { return "Intermediate"; }
            }
        }

    }
}
