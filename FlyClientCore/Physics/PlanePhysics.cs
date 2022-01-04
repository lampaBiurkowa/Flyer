using GeoLib;
using System;
using Shared.Plane;
using ClientCore.Physics.PlaneParts;

namespace ClientCore.Physics
{
    public class PlanePhysics : IMoveable
    {
        public Vector3 Direction {get; private set;}
        public FlightData FlightData {get; private set;}
        public MachineData MachineData {get; private set;}
        public PlaneData PlaneData {get; private set;}
        public Vector3 LocalRotation {get; private set;}

        const float DRAG_COEFFICENT = 0.025f;
        const float LIFT_COEFFICENT = 0.025f;
        const float SIDE_COEFFICENT = 1.5f;
        const float SPEED_COEFFICENT = 2f;

        public PlanePhysics(FlightData flightData, MachineData machineData, PlaneData planeData)
        {
            FlightData = flightData;
            MachineData = machineData;
            PlaneData = planeData;
        }

        public Vector3 GetSpeed() => FlightData.Speed;
        public float GetAirspeed() => SPEED_COEFFICENT * MathF.Sqrt(MathF.Pow((float)FlightData.Speed.X, 2) + MathF.Pow((float)FlightData.Speed.Z, 2));
        public float GetDiveForwardSpeed()
        {
            float val = (float)FlightData.Speed.Y * MathF.Sin((float)LocalRotation.Y);
            if (val < 0)
                val = 0;

            return val;
        }

        public void Update(FlightData flightData, Vector3 localRotation)
        {
            FlightData = flightData;
            LocalRotation = localRotation;
        }

        public float GetPartDrag(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetAirspeed(); //-wind spid
            float surface = part.GetDragSurface();

            return (float)(DRAG_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetPartLift(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetAirspeed();//-wind spid
            float surface = part.GetLiftSurface();

            float lift = (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);
            lift *= MathF.Cos((float)LocalRotation.Y); //pitch
            lift *= MathF.Cos((float)LocalRotation.X); //roll
            return lift;
        }

        public float GetPartSide(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = 1;//crosswind spid
            float surface = part.GetSideSurface();

            return (float)(SIDE_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetLeftLift(WindPhysics wind)
        {
            float leftLift = GetPartLift(PlaneData.LeftFlap, wind);
            leftLift += GetPartLift(PlaneData.LeftAileron, wind);
            leftLift += GetPartLift(PlaneData.LeftSlat, wind);
            leftLift += GetPartLift(PlaneData.LeftWing, wind);
            return leftLift;
        }

        public float GetRightLift(WindPhysics wind)
        {
            float rightLift = GetPartLift(PlaneData.RightFlap, wind);
            rightLift += GetPartLift(PlaneData.RightAileron, wind);
            rightLift += GetPartLift(PlaneData.RightSlat, wind);
            rightLift += GetPartLift(PlaneData.RightWing, wind);
            return rightLift;
        }

        public float GetLeftDrag(WindPhysics wind)
        {
            float leftDrag = GetPartDrag(PlaneData.LeftFlap, wind);
            leftDrag += GetPartDrag(PlaneData.LeftAileron, wind);
            leftDrag += GetPartDrag(PlaneData.LeftSlat, wind);
            leftDrag += GetPartDrag(PlaneData.LeftWing, wind);
            return leftDrag;
        }

        public float GetRightDrag(WindPhysics wind)
        {
            float rightDrag = GetPartDrag(PlaneData.RightFlap, wind);
            rightDrag += GetPartDrag(PlaneData.RightAileron, wind);
            rightDrag += GetPartDrag(PlaneData.RightSlat, wind);
            rightDrag += GetPartDrag(PlaneData.RightWing, wind);
            return rightDrag;
        }

        public float GetTailDrag(WindPhysics wind) => GetPartDrag(PlaneData.Gear, wind);

        public float GetTailSide(WindPhysics wind) => GetPartSide(PlaneData.Rudder, wind);

        public float GetCentralSide(WindPhysics wind)
        {
            float density = wind.GetDensity(FlightData.Altitude);
            float speed = GetAirspeed();//-wind spid

            float side = (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * PlaneData.LeftWing.GetLiftSurface());
            side += (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * PlaneData.RightWing.GetLiftSurface());
            side *= (1 - MathF.Pow(MathF.Cos((float)LocalRotation.X), 2)); //roll
            if (Math.Sin(LocalRotation.X) < 0)
                side *= -1;
            return side;
        }
    }
}