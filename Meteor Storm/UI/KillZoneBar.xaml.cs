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
    public partial class KillZoneBar : UserControl
    {
        public void Update(int vrij, int amunicija)
        {
            int _vrijednost = Math.Abs(vrij/2 - 100);
            if (_vrijednost < 100 || amunicija < 10) pbKillZone.Foreground = new SolidColorBrush(Colors.Black);
            else pbKillZone.Foreground = new SolidColorBrush(Colors.Red);
            pbKillZone.Value = _vrijednost;
        }

        public KillZoneBar()
        {
            InitializeComponent();
        }
    }
}
