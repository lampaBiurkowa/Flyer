using GeoLib;
using System;

namespace ClientCore.Physics
{
    public class PlanePhysics : IMoveable
    {
        public Vector2 WingSize {get; private set;}
        public Vector3 Direction {get; private set;}
        public Vector3 Speed {get; private set;}
        public Vector3 Position {get; private set;}
        public Vector3 Rotation {get; private set;}

        public PlanePhysics(Vector2 wingSize)
        {
            WingSize = wingSize / 5;
        }

        public float GetLift(WindPhysics wind, Vector3 Rotation, Vector3 Speed, Vector3 Position)
        {
            this.Rotation = Rotation;
            this.Speed = Speed;
            this.Position = Position;
            Direction = Speed / 100;
            //float wingSurface = 0;
            ///if (Rotation.Z >= -90 && Rotation.Z <= 90)
            //{
                //float localRotation = (float)Math.Abs(Rotation.Z);
            float wingSurface = 2 * (float)(Math.Cos(GameMath.DegToRad(0)) * WingSize.X * WingSize.Y);
            //}
            int height = (int)(Position.Y * 2);
            float density = wind.GetDensity(height);
            float attackAngle = 1;//(float)(GameMath.DegToRad(Math.Abs(Rotation.X - 90)));
            float speedAgainst = (float)wind.GetRelativeSpeedAdjusted(this).Y;
            float forwardSpeed = (float)(Math.Sqrt(Math.Pow(Speed.X, 2) + Math.Pow(Speed.Z, 2)));
            float totalSpeed = Math.Abs(forwardSpeed - speedAgainst);
            return (float)(density * (Math.Pow(totalSpeed, 2) / 2) * wingSurface * Math.Pow(Math.PI, 2) * attackAngle);
        }
    }
}