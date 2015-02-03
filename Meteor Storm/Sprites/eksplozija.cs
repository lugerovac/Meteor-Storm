using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Meteor_Storm
{
    public class eksplozija : Sprite
    {
        public int vijekTrajanja = 25;

        public eksplozija(double width, double height, Point firstPosition, int brzina, int kut)
            : base(width, height, firstPosition, brzina, kut)
        {

        }

        public override Canvas RenderSpriteCanvas()
        {
            return LoadSpriteCanvas("Meteor_Storm.Sprites.eksplozija.xaml");
        }
    }
}
