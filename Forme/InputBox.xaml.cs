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
using System.Windows.Shapes;

namespace Jbh
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox(string BoxTitle, string PromptText, string DefaultResponse)
        {
            InitializeComponent();
            this.Title = BoxTitle;
            textblockPrompt.Text = PromptText;
            textboxResponse.Text = DefaultResponse;
        }

        public string ResponseText { get { return textboxResponse.Text; } }

        private void buttonOkay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Icon = this.Owner.Icon;
            textboxResponse.Focus();
        }
    }
}
