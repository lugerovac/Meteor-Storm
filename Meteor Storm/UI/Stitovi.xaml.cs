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
    public partial class Stitovi : UserControl
    {
        private int _brojStitova;
        private double _stanjeStita = 0;
        
        public int BrojStitova
        {
            get { return _brojStitova; }
            set
            {
                _brojStitova = value;
                txtStitovi.Text = BrojStitova.ToString();
                if (BrojStitova == 0 && StanjeStita <= 0)
                {
                    elIkonaStita.Visibility = Visibility.Collapsed;
                    txtStitovi.Visibility = Visibility.Collapsed;
                }
                else
                {
                    elIkonaStita.Visibility = Visibility.Visible;
                    txtStitovi.Visibility = Visibility.Visible;
                }
            }
        }

        public double StanjeStita
        {
            get { return _stanjeStita; }
            set
            {
                _stanjeStita = value;
                pbStanjeStita.Value = StanjeStita;
                if (StanjeStita > 0)
                {
                    pbStanjeStita.Visibility = Visibility.Visible;
                }
                else
                {
                    _stanjeStita = 0;
                    pbStanjeStita.Visibility = Visibility.Collapsed;
                }
            }
        }

        public Stitovi()
        {
            InitializeComponent();
        }
    }
}
