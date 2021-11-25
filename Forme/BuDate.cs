using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    class BuDate
    {
        private static readonly DateTime birth = new DateTime(1954, 1, 3);

        public int QuandInteger { get; set; }
        public Ivresse Quoi { get; set; }
        internal enum Ivresse { SaisPas = 0, Bu = 1, PasBu = 2 };

        public static int DayOfLife(DateTime target)
        {
            TimeSpan ts = target - birth;
            return (int)(ts.TotalDays + 1);
        }

        public static DateTime DateFromDayOfLife(int DayOfLife)
        {
            int days = DayOfLife - 1;
            return birth.AddDays(days);
        }

        public string Specification
        {
            get
            {
                return $"{QuandInteger:00000}{(int)Quoi}";
            }
            set
            {
                string quoistr = value.Substring(5, 1);
                if (int.TryParse(quoistr, out int i))
                {
                    Quoi = (Ivresse)i;
                }
                else
                {
                    Quoi = Ivresse.SaisPas;
                }
                string quandstr = value.Substring(0, 5);
                if (int.TryParse(quandstr, out int j))
                {
                    QuandInteger = j;
                }
                else
                {
                    QuandInteger = 0;
                }
            }
        }

        public static string DateStamp(int a)
        {
            DateTime dt = DateFromDayOfLife(a);
            return $"{dt:d}"; // short date
        }
    }
}