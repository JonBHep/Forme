using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jbh
{
    public class PersonProfile
    {
        public PersonProfile()
        {
            LoadHeightData();
            LoadBloodPressureData();
            LoadWeightData();
            LoadWaistData();
        }

        public class BloodPressure : IComparable
        {
            public DateTime BprDate { get; set; }
            public int BprDiastolic { get; set; }
            public int BprSystolic { get; set; }
            public int BprPulse { get; set; }
            public int CompareTo(object other)
            {
                BloodPressure al = (BloodPressure)other;
                if (al == null) { return 1; }
                return this.BprDate.CompareTo(al.BprDate);
            }
        }

        public class Waist : IComparable
        {
            public DateTime wstDate { get; set; }
            public double wstCentimetres { get; set; }
            public int CompareTo(object other)
            {
                Waist al = (Waist)other;
                if (al == null) { return 1; }
                return this.wstDate.CompareTo(al.wstDate);
            }
        }

        public class Weight : IComparable
        {
            public DateTime wgtDate { get; set; }
            public double wgtKilograms { get; set; }
            public int CompareTo(object other)
            {
                Weight al = (Weight)other;
                if (al == null) { return 1; }
                return this.wgtDate.CompareTo(al.wgtDate);
            }
        }

        private List<BloodPressure> _bloodPressureReadings;
        private List<Waist> _waistReadings;
        private List<Weight> _weightReadings;

        private double _heightInMetres;

        private void LoadBloodPressureData()
        {
            _bloodPressureReadings = new List<BloodPressure>();
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.tension");
            if (!System.IO.File.Exists(dataFile)) { return; }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
            {
                while (!sr.EndOfStream)
                {
                    BloodPressure r = new BloodPressure
                    {
                        BprDate = DateTime.Parse(sr.ReadLine()),
                        BprSystolic = int.Parse(sr.ReadLine()),
                        BprDiastolic = int.Parse(sr.ReadLine()),
                        BprPulse = int.Parse(sr.ReadLine())
                    };
                    _bloodPressureReadings.Add(r);
                }
            };
            _bloodPressureReadings.Sort();
        }

        private void LoadHeightData()
        {
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.height");
            if (!System.IO.File.Exists(dataFile)) { return; }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
            {
                HeightInMetres = double.Parse(sr.ReadLine());
            };
        }

        private void LoadWaistData()
        {
            _waistReadings = new List<Waist>();
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.waist");
            if (!System.IO.File.Exists(dataFile)) { return; }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
            {
                while (!sr.EndOfStream)
                {
                    Waist r = new Waist
                    {
                        wstDate = DateTime.Parse(sr.ReadLine()),
                        wstCentimetres = double.Parse(sr.ReadLine())
                    };
                    _waistReadings.Add(r);
                }
            };
            _waistReadings.Sort();
        }

        private void LoadWeightData()
        {
            _weightReadings = new List<Weight>();
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.weight");
            if (!System.IO.File.Exists(dataFile)) { return; }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
            {
                while (!sr.EndOfStream)
                {
                    Weight r = new Weight
                    {
                        wgtDate = DateTime.Parse(sr.ReadLine()),
                        wgtKilograms = double.Parse(sr.ReadLine())
                    };
                    _weightReadings.Add(r);
                }
            };
            _weightReadings.Sort();
        }

        private void SaveHeightData()
        {
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.height");
            AppManager.CreateBackupDataFile(dataFile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(dataFile), MinimumDaysToKeep: 20, MinimumFilesToKeep: 4);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile))
            {
                sw.WriteLine(HeightInMetres.ToString());
            };
        }

        private void SaveBloodPressureData()
        {
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath,  "Data.tension");
            AppManager.CreateBackupDataFile(dataFile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(dataFile), MinimumDaysToKeep: 20, MinimumFilesToKeep: 4);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile))
            {
                foreach (BloodPressure r in _bloodPressureReadings)
                {
                    sw.WriteLine(r.BprDate.ToString());
                    sw.WriteLine(r.BprSystolic.ToString());
                    sw.WriteLine(r.BprDiastolic.ToString());
                    sw.WriteLine(r.BprPulse.ToString());
                }
            };
        }

        private void SaveWaistData()
        {
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.waist");
            AppManager.CreateBackupDataFile(dataFile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(dataFile), MinimumDaysToKeep: 20, MinimumFilesToKeep: 4);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile))
            {
                foreach (Waist r in _waistReadings)
                {
                    sw.WriteLine(r.wstDate.ToString());
                    sw.WriteLine(r.wstCentimetres.ToString());
                }
            };
        }

        private void SaveWeightData()
        {
            string dataFile = System.IO.Path.Combine(Jbh.AppManager.DataPath, "Data.weight");
            AppManager.CreateBackupDataFile(dataFile);
            AppManager.PurgeOldBackups(FileExtension: System.IO.Path.GetExtension(dataFile), MinimumDaysToKeep: 20, MinimumFilesToKeep: 4);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile))
            {
                foreach (Weight r in _weightReadings)
                {
                    sw.WriteLine(r.wgtDate.ToString());
                    sw.WriteLine(r.wgtKilograms.ToString());
                }
            };
        }

        public void SaveAllData()
        {
            SaveBloodPressureData();
            SaveWaistData();
            SaveWeightData();
            SaveHeightData();
        }

        public List<BloodPressure> BloodPressureReadings { get { return _bloodPressureReadings; } }
        public List<Waist> WaistReadings { get { return _waistReadings; } }
        public List<Weight> WeightReadings { get { return _weightReadings; } }

        public void EditWaistReading(int index, DateTime newDate, double newCm)
        {
            _waistReadings[index].wstDate = newDate;
            _waistReadings[index].wstCentimetres = newCm;
        }

        public void EditWeightReading(int index, DateTime newDate, double newKg)
        {
            _weightReadings[index].wgtDate = newDate;
            _weightReadings[index].wgtKilograms = newKg;
        }

        public void EditBloodPressureReading(int index, DateTime newDate, int newDiastolic, int newSystolic, int newPulse)
        {
            _bloodPressureReadings[index].BprDate = newDate;
            _bloodPressureReadings[index].BprDiastolic = newDiastolic;
            _bloodPressureReadings[index].BprPulse = newPulse;
            _bloodPressureReadings[index].BprSystolic = newSystolic;
        }

        public void AddWaistReading(DateTime newDate, double newCm)
        {
            Waist n = new Waist
            {
                wstDate = newDate,
                wstCentimetres = newCm
            };
            _waistReadings.Add(n);
        }

        public void AddWeightReading(DateTime newDate, double newKg)
        {
            Weight n = new Weight
            {
                wgtDate = newDate,
                wgtKilograms = newKg
            };
            _weightReadings.Add(n);
        }

        public void AddBloodPressureReading(DateTime newDate, int newDiastolic, int newSystolic, int newPulse)
        {
            BloodPressure n = new BloodPressure
            {
                BprDate = newDate,
                BprDiastolic = newDiastolic,
                BprPulse = newPulse,
                BprSystolic = newSystolic
            };
            _bloodPressureReadings.Add(n);
        }

        public DateTime LastBloodPressureReadingDate
        {
            get
            {
                return _bloodPressureReadings[_bloodPressureReadings.Count - 1].BprDate;
            }
        }

        public DateTime LastWaistReadingDate
        {
            get
            {
                return _waistReadings[_waistReadings.Count - 1].wstDate;
            }
        }

        public DateTime LastWeightReadingDate
        {
            get
            {
                return _weightReadings[_weightReadings.Count - 1].wgtDate;
            }
        }

        public string LastBloodPressureReadingValue
        {
            get
            {
                string s = "B.P. = " + _bloodPressureReadings[_bloodPressureReadings.Count - 1].BprSystolic.ToString() + " / " + _bloodPressureReadings[_bloodPressureReadings.Count - 1].BprDiastolic.ToString();
                s += " Pulse = " + _bloodPressureReadings[_bloodPressureReadings.Count - 1].BprPulse.ToString();
                return s;
            }
        }

        public string LastWaistReadingValue
        {
            get
            {
                return _waistReadings[_waistReadings.Count - 1].wstCentimetres.ToString() + " cm";
            }
        }
        public string LastWeightReadingImperialString
        {
            get
            {
                return BodyStatics.WeightAsStonesAndPoundsString(_weightReadings[_weightReadings.Count - 1].wgtKilograms);
            }
        }

        public double LastWeightReadingKg { get { return _weightReadings[_weightReadings.Count - 1].wgtKilograms; } }

        public double LastWeightReadingBmi { get { return BodyStatics.BmiOf(_weightReadings[_weightReadings.Count - 1].wgtKilograms, _heightInMetres); } }

        public double HeightInMetres { get => _heightInMetres; set => _heightInMetres = value; }

        public double IdealWeightLowerLimit { get { return BodyStatics.IdealWeightLowerLimitKg(_heightInMetres); } }

        public double IdealWeightHigherLimit { get { return BodyStatics.IdealWeightHigherLimit(_heightInMetres); } }

    }
}
