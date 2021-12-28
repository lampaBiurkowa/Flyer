using GeoLib;
using System;
using Shared.Objects;
using ClientCore.Physics.PlaneParts;

namespace ClientCore.Physics
{
    public class PlanePhysics : IMoveable
    {
        public Vector3 Direction {get; private set;}
        public FlightData FlightData {get; private set;}
        public PlaneData PlaneData {get; private set;}
        public AerodynamicsData AerodynamicsData {get; private set;}

        public PlanePhysics(FlightData flightData, PlaneData planeData, AerodynamicsData aerodynamicsData)
        {
            FlightData = flightData;
            PlaneData = planeData;
            AerodynamicsData = aerodynamicsData;
        }

        public Vector3 GetSpeed() => FlightData.Speed;
        public float GetForwardSpeed() => (float)Math.Sqrt(Math.Pow(FlightData.Speed.X, 2) + Math.Pow(FlightData.Speed.Z, 2));

        public void Update(FlightData flightData)
        {
            FlightData = flightData;
        }

        public float GetPartDrag(IAerodynamic part, WindPhysics wind)
        {
            const float DRAG_COEFFICENT = 1;
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetForwardSpeed(); //-wind spid
            float surface = part.GetDragSurface();

            return (float)(DRAG_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetPartLift(IAerodynamic part, WindPhysics wind)
        {
            const float LIFT_COEFFICENT = 1;
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetForwardSpeed();//-wind spid
            float surface = part.GetLiftSurface();

            return (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetPartSide(IAerodynamic part, WindPhysics wind)
        {
            const float SIDE_COEFFICENT = 1;
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = 1;//crosswind spid
            float surface = part.GetSideSurface();

            return (float)(SIDE_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetLeftLift(WindPhysics wind)
        {
            float leftLift = GetPartLift(AerodynamicsData.LeftFlap, wind);
            leftLift += GetPartLift(AerodynamicsData.LeftAileron, wind);
            leftLift += GetPartLift(AerodynamicsData.LeftSlat, wind);
            leftLift += GetPartLift(AerodynamicsData.LeftWing, wind);
            return leftLift;
        }

        public float GetRightLift(WindPhysics wind)
        {
            float rightLift = GetPartLift(AerodynamicsData.RightFlap, wind);
            rightLift += GetPartLift(AerodynamicsData.RightAileron, wind);
            rightLift += GetPartLift(AerodynamicsData.RightSlat, wind);
            rightLift += GetPartLift(AerodynamicsData.RightWing, wind);
            return rightLift;
        }

        /*public float GetLift(WindPhysics wind, FlightData flightData)
        {
            Direction = flightData.Speed / 100;
            
            float wingSurface = (float)(Math.Cos(GameMath.DegToRad(0)) * WingSize.X * WingSize.Y);
            int height = (int)(flightData.Position.Y * 2);
            float density = wind.GetDensity(height);
            float attackAngle = 1;//(float)(GameMath.DegToRad(Math.Abs(Rotation.X - 90)));
            float speedAgainst = (float)wind.GetRelativeSpeedAdjusted(this).Y;
            float forwardSpeed = (float)(Math.Sqrt(Math.Pow(flightData.Speed.X, 2) + Math.Pow(flightData.Speed.Z, 2)));
            float totalSpeed = Math.Abs(forwardSpeed - speedAgainst);
            
            float baseLift = (float)(density * (Math.Pow(totalSpeed, 2) / 2) * wingSurface * Math.Pow(Math.PI, 2) * attackAngle);
            return baseLift;
            //return new Vector2(baseLift, baseLift);
        }*/
    }
}