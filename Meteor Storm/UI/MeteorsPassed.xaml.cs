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
    public partial class MeteorsPassed : UserControl
    {
        private int _meteoriScore;

        public int MeteoriScore
        {
            get { return _meteoriScore; }
            set
            {
                _meteoriScore = value;
                txtMP.Text = "Prošlo meteora: " + MeteoriScore.ToString();
            }
        }

        public MeteorsPassed()
        {
            InitializeComponent();
        }
    }
}
