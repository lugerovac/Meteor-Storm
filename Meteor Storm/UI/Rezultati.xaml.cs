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
    public partial class Rezultati : UserControl
    {
        public void prikazi(int brojMeteora, int bodovi, int udarci, int pokupljeno)
        {
            txtGrumeni.Text = "Pokupljeno grumenja: " + pokupljeno.ToString();
            txtStatusMP.Text = "Količina meteora koji su prošli: " + brojMeteora.ToString();
            txtStatusUM.Text = "Količina uništenih meteora: " + bodovi.ToString();
            txtUdarci.Text = "Količina pretrpljenih udara: " + udarci.ToString();
        }

        public Rezultati()
        {
            InitializeComponent();
        }
    }
}
