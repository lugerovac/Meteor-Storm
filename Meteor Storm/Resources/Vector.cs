using System;
using System.Windows;

// This code was addapted from BlueRoseGames.com and others
namespace Meteor_Storm
{
    public struct Vector
    {
        public double X;
        public double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(LengthSquared);
            }
        }

        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public void Normalize()
        {
            double length = Length;
            X /= length;
            Y /= length;
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y);
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(scalar * vector.X, scalar * vector.Y);
        }

        public static Point operator +(Point point, Vector vector)
        {
            return new Point(point.X + vector.X, point.Y + vector.Y);
        }

        static public Vector CreateVectorFromAngle(double angleInDegrees, double length)
        {
            double x = Math.Sin(DegreesToRadians(180 - angleInDegrees)) * length;
            double y = Math.Cos(DegreesToRadians(180 - angleInDegrees)) * length;
            return new Vector(x, y);
        }

        static public double DegreesToRadians(double degrees)
        {
            double radians = ((degrees / 360) * 2 * Math.PI);
            return radians;
        }
    }
}
