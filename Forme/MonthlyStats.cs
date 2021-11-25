using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class MonthlyStats : GroupedStats
    {
        private readonly DateTime _anyDay;

        public MonthlyStats(DateTime anyDate)
        {
            _anyDay = anyDate.Date; // any date within the target month
        }

        public void ConsiderRide(Balade outing, int rideIdent)
        {
            if (outing.RideDate.Year != _anyDay.Year) { return; }
            if (outing.RideDate.Month != _anyDay.Month) { return; }
            this.AddRide(outing, rideIdent);
        }

        public int YearNumber { get { return _anyDay.Year; } }
        public int MonthNumber { get { return _anyDay.Month; } }
    }
}
