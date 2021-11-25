using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class BikeGears
    {

        private List<Gearing> _gearings = new List<Gearing>();
        private string _bikeName = string.Empty;
        private int _wheelCircumferenceMm = 0;
        private List<int> _chainrings = new List<int>();
        private List<int> _sprockets = new List<int>();

        public string BikeName { get => _bikeName; set => _bikeName = value; }
        public List<Gearing> Gearings { get => _gearings; set => _gearings = value; }
        public int WheelCircumference { get => _wheelCircumferenceMm; set => _wheelCircumferenceMm = value; }
        public List<int> Chainrings { get => _chainrings; set => _chainrings = value; }
        public List<int> Sprockets { get => _sprockets; set => _sprockets = value; }

        public BikeGears(string cycleName)
        {
            _bikeName = cycleName;
        }

        public static List<int> CogWheelTeeth(string codeString)
        {
            string[] rings = codeString.Split("-".ToCharArray());
            List<int> teeth = new List<int>();
            bool fail = false;
            foreach (string part in rings)
            {
                if (int.TryParse(part, out int t))
                {
                    teeth.Add(t);
                }
                else
                {
                    fail = true;
                }
            }
            if (fail)
            {
                teeth = new List<int>();
            }
            return teeth;
        }

        public static string CogwheelSpecificationString(List<int> sourceList)
        {
            return string.Join("-", sourceList);
        }

        public void Load()
        {
            string filepath = System.IO.Path.Combine(AppManager.DataPath, _bikeName + ".jbh");
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
            {
                string inp = sr.ReadLine();
                _chainrings = CogWheelTeeth(inp);

                inp = sr.ReadLine();
                _sprockets = CogWheelTeeth(inp);

                inp = sr.ReadLine();
                _wheelCircumferenceMm = int.Parse(inp);
            }
        }

        public void Save()
        {
            string filepath = System.IO.Path.Combine(AppManager.DataPath, _bikeName + ".jbh");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath))
            {
                sw.WriteLine(CogwheelSpecificationString(_chainrings));
                sw.WriteLine(CogwheelSpecificationString(_sprockets));
                sw.WriteLine(_wheelCircumferenceMm.ToString());
            }
        }

        public void CalculateGearings()
        {
            _gearings = new List<Gearing>();
            for (int c = 0; c < _chainrings.Count; c++)
            {
                for (int s = 0; s < _sprockets.Count; s++)
                {
                    Gearing g = new Gearing(c + 1, _chainrings[c], s + 1, _sprockets[s]);
                    _gearings.Add(g);
                }
                _gearings.Sort();
            }
        }

    }
}
