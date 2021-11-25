using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class Epoch : IComparable<Epoch>
    {
        // grouping of rides by time or location
        private string _caption;
        private DateTime _firstDate;
        private DateTime _lastDate;
        public string Caption { get => _caption; set => _caption = value; }
        public DateTime FirstDate { get => _firstDate; set => _firstDate = value; }
        public DateTime LastDate { get => _lastDate; set => _lastDate = value; }

        public Epoch(string caption)
        {
            _caption = caption;
            _firstDate = DateTime.MaxValue;
            _lastDate = DateTime.MinValue;
        }

        public int CompareTo(Epoch other)
        {
            return _firstDate.CompareTo(other._firstDate);
        }
    }
}
