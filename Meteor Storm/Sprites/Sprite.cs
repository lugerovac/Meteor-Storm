using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Meteor_Storm
{
    public abstract class Sprite
    {
        public int kut { get; set; }
        public int brzina { get; set; }
        public double MaxX { get; set; }
        public double MinX { get; set; }
        public double MaxY { get; set; }
        public double MinY { get; set; }
        public double CentarX { get; set; }
        public double CentarY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int arg1 { get; set; }
        public Vector Velocity { get; set; }
        public Canvas SpriteCanvas { get; set; }
        public double rotacija { get; set; }
        public double CollisionRadius;
        private Point _position;
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                SpriteCanvas.SetValue(Canvas.TopProperty, _position.Y - (Height / 2));
                SpriteCanvas.SetValue(Canvas.LeftProperty, _position.X - (Width / 2));
            }
        }

        public Sprite(Double initialWidth, Double initialHeight, Point initialPosition, int initialSpeed, int initialAngle)
        {
            Width = initialWidth;
            Height = initialHeight;
            kut = initialAngle;
            brzina = initialSpeed;

            CentarX = initialWidth / 2;
            CentarY = initialHeight / 2;
            rotacija = 0;

            SpriteCanvas = RenderSpriteCanvas();

            SpriteCanvas.Width = Width;
            SpriteCanvas.Height = Height;
            Position = initialPosition;
            CollisionRadius = Width * .5;
        }

        public Sprite(Double initialWidth, Double initialHeight, Point initialPosition, int initialSpeed, int initialAngle, int initial_arg1)
        {
            arg1 = initial_arg1;

            Width = initialWidth;
            Height = initialHeight;
            kut = initialAngle;
            brzina = initialSpeed;

            CentarX = initialWidth / 2;
            CentarY = initialHeight / 2;
            rotacija = 0;

            SpriteCanvas = RenderSpriteCanvas();

            SpriteCanvas.Width = Width;
            SpriteCanvas.Height = Height;
            Position = initialPosition;
            CollisionRadius = Width * .5;
        }

        public abstract Canvas RenderSpriteCanvas();

        public Canvas LoadSpriteCanvas(string xamlPath)
        {
            System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream(xamlPath);
            return (Canvas)XamlReader.Load(new System.IO.StreamReader(s).ReadToEnd());
        }

        public virtual void Update(TimeSpan elapsedTime)
        {
            Position = (Position + Velocity * elapsedTime.TotalSeconds);
        }

        public static bool Collides(Sprite s1, Sprite s2)
        {
            Vector v = new Vector((s1.Position.X) - (s2.Position.X), (s1.Position.Y) - (s2.Position.Y));
            if (s1.CollisionRadius + s2.CollisionRadius > v.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
