using GeoLib;
using System;

namespace ClientCore.Physics
{
    public class WindPhysics : IMoveable
    {
        const float DENSITY_MAX = 1.2f; //kg/m^3
        const float DENSITY_MIN = 0.24f; //kg/m^3
        const int MAX_HEIGHT = 13700; //m

        public Vector3F Direction {get; set;}
        public Vector3F Speed {get; set;}
        public float GetDensity(int height)
        {
            MathLine line = new MathLine(new Vector2(0, DENSITY_MAX), new Vector2(MAX_HEIGHT, DENSITY_MIN));
            return (float)line.ValueAt(height);
        }

        public Vector3F GetSpeed() => Speed; //??

        public Vector2F GetRelativeSpeedAdjusted(IMoveable moveable)
        {
            float secondPosFactor = 1.1f; //can be set to anything close to 1.0f (but greater)
            Vector2 moveableFakePosition = new Vector2(moveable.Direction.X, moveable.Direction.Y);
            Vector2 windFakePosition = new Vector2(Direction.X, Direction.Y);
            double moveableAngle = GameMath.GetAngleBetweenPointsLineAndHorizontalLine(moveableFakePosition, moveableFakePosition * secondPosFactor);
            double moveableLength = GameMath.GetDistance(moveableFakePosition, moveableFakePosition * secondPosFactor);
            double ownAngle = GameMath.GetAngleBetweenPointsLineAndHorizontalLine(windFakePosition, windFakePosition * secondPosFactor);
            double crossAngle = Math.Abs(moveableAngle - ownAngle);
            if (crossAngle > 180)
                crossAngle -= 180;
            float xRelative = (float)(Math.Sin(crossAngle) * moveableLength * Speed.X);
            float yRelative = (float)(Math.Cos(crossAngle) * moveableLength * Speed.Y);
            return new Vector2F(xRelative, yRelative);
        }
    }
}