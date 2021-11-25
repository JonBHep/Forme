using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class Gearing : IComparable<Gearing>
    {
        private readonly int _chainringTeeth;
        private readonly int _sprocketTeeth;
        private readonly int _chainringIndex;
        private readonly int _sprocketIndex;

        private double _ratio;
        public Gearing(int cindex, int cteeth, int sindex, int steeth)
        {
            _chainringIndex = cindex;
            _chainringTeeth = cteeth;
            _sprocketIndex = sindex;
            _sprocketTeeth = steeth;
            _ratio = (double)_chainringTeeth / SprocketTeeth;
        }
        public int ChainringTeeth { get => _chainringTeeth; }
        public int SprocketTeeth { get => _sprocketTeeth; }
        public double Ratio { get => _ratio; }
        public int ChainringIndex { get => _chainringIndex; }
        public int SprocketIndex { get => _sprocketIndex; }

        public bool IsCrossover(int chainrings, int sprockets)
        {
            // non-advised gearings where chain crosses sides on chainrings and sprockets
            return Gearing.IsCrossover(chainrings, sprockets, _chainringIndex, _sprocketIndex);
        }

        public static bool IsCrossover(int chainringCount, int sprocketCount, int chainringIndex, int sprocketIndex)
        {
            // non-advised gearings where chain crosses sides on chainrings and sprockets
            bool ok = true;
            double midchainring = (chainringCount + 1) / 2D;
            double midsprocket = (sprocketCount + 1) / 2D;
            if ((chainringIndex < midchainring) && (sprocketIndex > midsprocket)) { ok = false; }
            if ((chainringIndex > midchainring) && (sprocketIndex < midsprocket)) { ok = false; }
            return !ok;
        }

        int IComparable<Gearing>.CompareTo(Gearing other)
        {
            return this._ratio.CompareTo(other._ratio);
        }

    }
}
