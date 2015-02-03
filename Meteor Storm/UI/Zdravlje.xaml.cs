using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Meteor_Storm
{
    public partial class Zdravlje : UserControl
    {
        private double _health;

        public double Health
        {
            get { return _health; }
            set
            {
                if (value < 0) _health = 0;
                else _health = value;
                pbZdravlje.Value = Health;

                if(Health > 65) pbZdravlje.Foreground = new SolidColorBrush(Colors.Green);
                if(Health < 65 && Health > 35) pbZdravlje.Foreground = new SolidColorBrush(Colors.Black);
                if(Health < 35) pbZdravlje.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        public Zdravlje()
        {
            InitializeComponent();
        }
    }
}
