using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public abstract class GroupedStats
    {
        private double _minTripKmVelo;
        private double _maxTripKmVelo;
        private double _minTripKmPied;
        private double _maxTripKmPied;

        public GroupedStats()
        {
            TripCountPied = 0;
            TripCountVelo = 0;
            RiddenKilometres = 0;
            RideIdentifiers = new List<int>();
            WalkedKilometres = 0;
            WalkIdentifiers = new List<int>();
            _maxTripKmPied = 0;
            _maxTripKmVelo = 0;
            _minTripKmPied = double.MaxValue;
            _minTripKmVelo = double.MaxValue;
        }

        public void AddRide(Balade outing, int rideId)
        {
            if (outing.Kind == VeloHistory.TripType.Cycle)
            {
                TripCountVelo++;
                RiddenKilometres += outing.RideKm;
                _minTripKmVelo = Math.Min(_minTripKmVelo, outing.RideKm);
                _maxTripKmVelo = Math.Max(_maxTripKmVelo, outing.RideKm);
                RideIdentifiers.Add(rideId);
            }
            else
            {
                TripCountPied++;
                WalkedKilometres += outing.RideKm;
                _minTripKmPied = Math.Min(_minTripKmPied, outing.RideKm);
                _maxTripKmPied = Math.Max(_maxTripKmPied, outing.RideKm);
                WalkIdentifiers.Add(rideId);
            }
        }

        public int TripCountPied { get; private set; }
        public int TripCountVelo { get; private set; }
        public double RiddenKilometres { get; private set; }
        public double WalkedKilometres { get; private set; }
        public List<int> RideIdentifiers { get; }
        public List<int> WalkIdentifiers { get; }
        public int PerTripKmMinVelo { get => (int)Math.Round(_minTripKmVelo); }
        public int PerTripKmMaxVelo { get => (int)Math.Round(_maxTripKmVelo); }
        public int PerTripKmMinPied { get => (int)Math.Round(_minTripKmPied); }
        public int PerTripKmMaxPied { get => (int)Math.Round(_maxTripKmPied); }
        public int PerTripKmMeanVelo { get => (int)Math.Round(RiddenKilometres / TripCountVelo); }
        public int PerTripKmMeanPied { get => (int)Math.Round(WalkedKilometres / TripCountPied); }

    }
}
