using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Jbh
{
    /// <summary>
    /// Interaction logic for PaintingWindow.xaml
    /// </summary>
    public partial class PaintingWindow : Window
    {
        public PaintingWindow()
        {
            InitializeComponent();
        }

        private void PaintCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string title = GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
            string copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
            string description = GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);

            textblockVersion.Text = "Version ";
            DateTime startDate = new DateTime(2000, 1, 1);
            Version versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            int diffDays = versionInfo.Build;
            //DateTime computedDate = startDate.AddDays(diffDays);

            textblockTitle.Text = title;
            textblockDescription.Text = description;
            textblockCopyright.Text = copyright;

            //textblockBuild.Text = computedDate.ToLongDateString();
            textblockVersion.Text = versionInfo.Major.ToString() + "." + versionInfo.Minor.ToString();
        }

        public string GetAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}
