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
    public partial class Informacije : UserControl
    {
        private double _rijetkost;
        private string _bonus;
        public bool nestalo;

        public void prorijedi()
        {
            txtInfo.Opacity = _rijetkost -= 0.01;
            if (_rijetkost < 0.1) nestalo = true;
        }

        public string Bonus
        {
            get { return _bonus; }
            set
            {
                _bonus = value;
                txtInfo.Text = Bonus;
            }
        }

        public Informacije()
        {
            InitializeComponent();
            _rijetkost = 1;
            nestalo = false;
        }
    }
}
