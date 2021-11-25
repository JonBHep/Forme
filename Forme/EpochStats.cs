using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    class EpochStats : GroupedStats
    {
        private string _groupCaption;
        private string _groupKey;

        public EpochStats(Epoch whenWhere)
        {
            string dates;
            if (whenWhere.FirstDate.Year.Equals(whenWhere.LastDate.Year))
            {
                if (whenWhere.FirstDate.Month.Equals(whenWhere.LastDate.Month))
                {
                    if (whenWhere.FirstDate.Day.Equals(whenWhere.LastDate.Day))
                    {
                        // same date
                        dates = $"({whenWhere.FirstDate.ToString("dd MMM yyyy")})";
                    }
                    else
                    {
                        // same month
                        dates = $"({whenWhere.FirstDate.ToString("dd")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
                    }
                }
                else
                {
                    // same year
                    dates = $"({whenWhere.FirstDate.ToString("dd MMM")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
                }
            }
            else
            {
                // different year
                dates = $"({whenWhere.FirstDate.ToString("dd MMM yyyy")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
            }
            _groupCaption = $"{whenWhere.Caption} {dates}";
            GroupKey = whenWhere.Caption;
        }

        public void ConsiderRide(Balade outing, int rideIdent)
        {
            if (outing.RideGroup != GroupKey) { return; }
            this.AddRide(outing, rideIdent);
        }

        public string GroupCaption { get { return _groupCaption; } }

        public string GroupKey { get => _groupKey; set => _groupKey = value; }

        public VeloHistory.TripType TripKind
        {
            get
            {
                if (TripCountPied > 0) { return VeloHistory.TripType.Walk; }
                return VeloHistory.TripType.Cycle;
            }
        }

        public bool MixedTripKinds
        {
            get
            {
                return ((TripCountPied > 0) && (TripCountVelo > 0));
            }
        }

    }
}
