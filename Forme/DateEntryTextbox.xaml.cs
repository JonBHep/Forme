﻿using System;
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
    /// Interaction logic for DateEntryTextbox.xaml
    /// </summary>
    public partial class DateEntryTextbox : UserControl
    {
        private DateTime _value;
        private bool _hasValue;
        public event EventHandler ValueChanged;

        public DateEntryTextbox()
        {
            InitializeComponent();
            Clear();
        }

        private void textboxDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = textboxDate.Text.Trim();
            if (String.IsNullOrWhiteSpace(q))
            {
                textboxDate.Opacity = 0.5;
                _hasValue = false;
                textboxDate.ToolTip = "Null date";
                textblockResult.Text = "Null date";
            }
            else
            {
                textboxDate.Opacity = 1;
                if (DateTime.TryParse(q, out _value))
                {
                    textboxDate.Foreground = Brushes.Black;
                    _hasValue = true;
                    textboxDate.ToolTip = _value.ToString("dd MMM yyyy");
                    textblockResult.Text = _value.ToString("dd MMM yyyy");
                }
                else
                {
                    textboxDate.Foreground = Brushes.Red;
                    _hasValue = false;
                    textboxDate.ToolTip = "Null date";
                    textblockResult.Text = "Null date";
                }
            }
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public DateTime? DateValue
        {
            get
            {
                if (_hasValue) { return _value; } else return null;
            }
            set
            {
                _value = value.Value;
                _hasValue = value.HasValue;
		if (_value.Ticks < 1)
                {
                    textboxDate.Clear();
                }
                else
                {
                    textboxDate.Text = _value.ToShortDateString();
                }

            }
        }

        public void Clear()
        {
            _value = new DateTime(1954, 1, 3);
            _hasValue = false;
            textboxDate.Clear();
        }

        private void textboxDate_GotFocus(object sender, RoutedEventArgs e)
        {
            gridBase.Background = Brushes.White;
        }

        private void textboxDate_LostFocus(object sender, RoutedEventArgs e)
        {
            gridBase.Background = Brushes.WhiteSmoke;
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            textblockResult.Text = string.Empty;
        }

    }
}
