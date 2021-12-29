using GeoLib;
using System;
using Shared;
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
        public Vector3 LocalRotation {get; private set;}

        public PlanePhysics(FlightData flightData, PlaneData planeData, AerodynamicsData aerodynamicsData)
        {
            FlightData = flightData;
            PlaneData = planeData;
            AerodynamicsData = aerodynamicsData;
        }

        public Vector3 GetSpeed() => FlightData.Speed;
        //public float GetForwardSpeed() => (float)Math.Sqrt(Math.Pow(FlightData.Speed.X, 2) + Math.Pow(FlightData.Speed.Z, 2));
        
        public float GetForwardSpeed()
        {
            float x = (float)FlightData.Speed.X * MathF.Cos((float)LocalRotation.Z); //yaw
            float y = (float)FlightData.Speed.Y * MathF.Sin((float)LocalRotation.Y); //pitch
            float z = (float)FlightData.Speed.Z * MathF.Cos((float)LocalRotation.X); //roll
            return MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2) + MathF.Pow(z, 2));
        }

        public void Update(FlightData flightData, Vector3 localRotation)
        {
            FlightData = flightData;
            LocalRotation = localRotation;
        }

        public float GetPartDrag(IAerodynamic part, WindPhysics wind)
        {
            const float DRAG_COEFFICENT = 0.0001f;
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetForwardSpeed(); //-wind spid
            float surface = part.GetDragSurface();

            return (float)(DRAG_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetPartLift(IAerodynamic part, WindPhysics wind)
        {
            const float LIFT_COEFFICENT = 0.0001f;
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

        public float GetLeftDrag(WindPhysics wind)
        {
            float leftDrag = GetPartDrag(AerodynamicsData.LeftFlap, wind);
            leftDrag += GetPartDrag(AerodynamicsData.LeftAileron, wind);
            leftDrag += GetPartDrag(AerodynamicsData.LeftSlat, wind);
            leftDrag += GetPartDrag(AerodynamicsData.LeftWing, wind);
            return leftDrag;
        }

        public float GetRightDrag(WindPhysics wind)
        {
            float rightDrag = GetPartDrag(AerodynamicsData.RightFlap, wind);
            rightDrag += GetPartDrag(AerodynamicsData.RightAileron, wind);
            rightDrag += GetPartDrag(AerodynamicsData.RightSlat, wind);
            rightDrag += GetPartDrag(AerodynamicsData.RightWing, wind);
            return rightDrag;
        }
    }
}