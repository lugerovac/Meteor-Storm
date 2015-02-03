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
    public partial class Torpedi : UserControl
    {
        private int _torpedi;

        public int TorpediAmunicija
        {
            get { return _torpedi; }
            set
            {
                _torpedi = value;
                txtTorpedo.Text = TorpediAmunicija.ToString();
            }
        }

        public Torpedi()
        {
            InitializeComponent();
        }
    }
}
