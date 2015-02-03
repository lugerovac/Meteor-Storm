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
    public class Ship : Sprite
    {
        //arg1 - određuje je li brod zaštičen ili nije

        public Ship(double width, double height, Point firstPosition, int brzina, int kut, int arg1)
            : base(width, height, firstPosition, brzina, kut, arg1)
        {
            
        }

        public override Canvas RenderSpriteCanvas()
        {
            if (arg1 == 1) return LoadSpriteCanvas("Meteor_Storm.Sprites.ShipShielded.xaml");
            else return LoadSpriteCanvas("Meteor_Storm.Sprites.Ship.xaml");
        }

        public override void Update(System.TimeSpan elapsedTime)
        {
            if (Position.X > MaxX)
            {
                Position = new Point(MaxX, Position.Y);
                Velocity = new Vector(0, 0);
            }
            if (Position.X < MinX)
            {
                Position = new Point(MinX, Position.Y);
                Velocity = new Vector(0, 0);
            }
            if(Position.Y > MaxY)
            {
                Position = new Point(Position.X, MaxY);
                Velocity = new Vector(0, 0);
            }
            if(Position.Y < MinY)
            {
                Position = new Point(Position.X, MinY);
                Velocity = new Vector(0, 0);
            }
            base.Update(elapsedTime);
        }

        public Torpedo Fire()
        {
            Point pozicija = new Point();
            pozicija.X = Position.X;
            pozicija.Y = Position.Y + 16;

            Torpedo pucanj = new Torpedo(5, 14, pozicija, 1000, kut);
            pucanj.kut = kut;
            pucanj.Velocity = Vector.CreateVectorFromAngle(pucanj.kut, pucanj.brzina);
            return pucanj;
        }
    }
}
