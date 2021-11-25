using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jbh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GymButton_Click(object sender, RoutedEventArgs e)
        {
            GymWindow w = new GymWindow() { Owner = this };
            w.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PaintingWindow ww = new PaintingWindow() { Owner = this };
            ww.ShowDialog();
        }

        private void VeloButton_Click(object sender, RoutedEventArgs e)
        {
            VeloMainWindow vm = new VeloMainWindow() { Owner = this };
            vm.ShowDialog();
        }

        private void MeasuresButton_Click(object sender, RoutedEventArgs e)
        {
            FitnessMainWindow fm = new FitnessMainWindow() { Owner = this };
            fm.ShowDialog();
        }

        private void BuButton_Click(object sender, RoutedEventArgs e)
        {
            BuWindow b = new BuWindow() { Owner = this };
            b.ShowDialog();
        }
    }
}
