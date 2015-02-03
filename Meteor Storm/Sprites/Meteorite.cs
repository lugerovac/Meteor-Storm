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
    public class Meteorite : Sprite
    {
        public Meteorite(double width, double height, Point firstPosition, int brzina, int kut)
            : base(width, height, firstPosition, brzina, kut)
        {

        }

        public override Canvas RenderSpriteCanvas()
        {
            if (Width < 50)
            {
                return LoadSpriteCanvas("Meteor_Storm.Sprites.MeteoriteMini.xaml");
            }
            else
            {
                return LoadSpriteCanvas("Meteor_Storm.Sprites.Meteorite.xaml");
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
        }

        public bool checkLocation()
        {
            if (Position.X < MinX || Position.X > MaxX || Position.Y < MinY || Position.Y > MaxY) return true;
            else return false;
        }

        public eksplozija unisten()
        {
            eksplozija explosion = new eksplozija(50, 50, Position, 0, 0);
            explosion.Velocity = Vector.CreateVectorFromAngle(0, 0);
            return explosion;
        }
    }
}
