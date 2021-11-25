using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Forme
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _suppressRuntimeDeletion;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _suppressRuntimeDeletion = false;
            bool OkToLaunch = true;
            string titl = Jbh.AppManager.AppName;
            if (string.IsNullOrWhiteSpace(Jbh.AppManager.DataPath))
            {
                MessageBox.Show("The folder for the " + titl + " application data within the Databank\\AppData directory has not been found.\n\nThe application will now be shut down.", titl, MessageBoxButton.OK, MessageBoxImage.Error);
                OkToLaunch = false;
            }
            else
            {
                DateTime? launched = Jbh.AppManager.RunStart;
                if (launched.HasValue)
                {

                    MessageBoxResult answer = MessageBox.Show(titl + " appears to be already running.\n\nLaunched at " + launched.Value.ToShortTimeString() + " on " + launched.Value.ToShortDateString() + "\n\nOnly continue if you are sure that " + titl + " is not currently running.\n\nContinue?", titl, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.OK)
                    {
                        OkToLaunch = false;
                    }
                }
            }
            if (OkToLaunch)
            {
                Jbh.AppManager.SetRunStart();
                Jbh.MainWindow w = new Jbh.MainWindow();
                w.Show();
            }
            else
            {
                _suppressRuntimeDeletion = true;
                App.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (!_suppressRuntimeDeletion)
            {
                Jbh.AppManager.DeleteRuntimeFile();
            }
        }
    }
}
