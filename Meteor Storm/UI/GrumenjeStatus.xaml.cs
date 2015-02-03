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
    public partial class GrumenjeStatus : UserControl
    {
        private int _bodovi;
        public int Bodovi
        {
            get { return _bodovi; }
            set
            {
                _bodovi = value;
                txtGrumeni.Text = ": " + Bodovi.ToString();
            }
        }

        public GrumenjeStatus()
        {
            InitializeComponent();
        }
    }
}
