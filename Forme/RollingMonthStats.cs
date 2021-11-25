using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class RollingMonthStats : GroupedStats
    {
        readonly DateTime _monthAgo;

        public RollingMonthStats(DateTime notionalToday)
        {
            _monthAgo = notionalToday.AddMonths(-1);
        }

        public void ConsiderRide(Balade outing, int rideIdent)
        {
            if (outing.RideDate <= _monthAgo) { return; }
            this.AddRide(outing, rideIdent);
        }

    }
}
