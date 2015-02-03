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
    public class Torpedo : Sprite
    {
        public Torpedo(double width, double height, Point firstPosition, int brzina, int kut)
            : base(width, height, firstPosition, brzina, kut)
        {

        }

        public override Canvas RenderSpriteCanvas()
        {
            return LoadSpriteCanvas("Meteor_Storm.Sprites.Torpedo.xaml");
        }

        public override void Update(System.TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
        }

        public bool checkLocation()
        {
            if (Position.X < MinX || Position.X > MaxX || Position.Y < MinY || Position.Y > MaxY) return true;
            else return false;
        }
    }
}
