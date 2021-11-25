using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class GymHistory
    {
        public enum GymType { GymTraining, AquaAerobics, Other, Void };

        private readonly List<GymVisit> _gymVisits = new List<GymVisit>();
        private readonly string _gymdatafile;
        public List<GymVisit> GymList { get { return _gymVisits; } }
     
        public GymHistory(string gfile)
        {
            // get data file path
            _gymdatafile = gfile;
            string GDatafile = AppManager.DataPath;
            GDatafile = System.IO.Path.Combine(GDatafile, _gymdatafile);
            if (System.IO.File.Exists(GDatafile))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(GDatafile))
                {
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        GymVisit g = new GymVisit() { Specification = s };
                        _gymVisits.Add(g);
                    }
                }
            }
        }

        public void SaveData()
        {
            string GDatafile = AppManager.DataPath;
            GDatafile = System.IO.Path.Combine(GDatafile, _gymdatafile);
            // backup existing data
            AppManager.CreateBackupDataFile(GDatafile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(_gymdatafile), MinimumDaysToKeep: 40, MinimumFilesToKeep: 4);

            // write new data
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(GDatafile))
            {
                foreach (GymVisit g in _gymVisits)
                {
                    sw.WriteLine(g.Specification);
                }
            }
        }
        
    }
}
