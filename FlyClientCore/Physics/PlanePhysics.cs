using GeoLib;
using System;
using Shared.Plane;
using ClientCore.Physics.PlaneParts;

namespace ClientCore.Physics
{
    public class PlanePhysics : IMoveable
    {
        public Vector3F Direction {get; private set;}
        public PlaneData Plane {get; private set;}

        const float DRAG_COEFFICENT = 0.025f;
        const float LIFT_COEFFICENT = 0.025f;
        const float SIDE_COEFFICENT = 1.5f;
        const float SPEED_COEFFICENT = 2f;

        public PlanePhysics(PlaneData planeData)
        {
            Plane = planeData;
        }

        public Vector3F GetSpeed() => Plane.Flight.Speed;
        public float GetAirspeed() => SPEED_COEFFICENT * MathF.Sqrt(MathF.Pow(Plane.Flight.Speed.X, 2) + MathF.Pow(Plane.Flight.Speed.Z, 2));
        public float GetDiveForwardSpeed()
        {
            float val = Plane.Flight.Speed.Y * MathF.Sin(Plane.Flight.Pitch);
            if (val < 0)
                val = 0;

            return val;
        }

        public void Update(PlaneData planeData)
        {
            Plane = planeData;
        }

        public float GetPartDrag(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(Plane.Flight.Altitude);
            float speed = GetAirspeed(); //-wind spid
            float surface = part.GetDragSurface();

            return (float)(DRAG_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetPartLift(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(Plane.Flight.Altitude);
            float speed = GetAirspeed();//-wind spid
            float surface = part.GetLiftSurface();

            float lift = (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);
            lift *= MathF.Cos(Plane.Flight.Pitch);
            lift *= MathF.Cos(Plane.Flight.Roll);
            return lift;
        }

        public float GetPartSide(IAerodynamic part, WindPhysics wind)
        {
            float density = wind.GetDensity(Plane.Flight.Altitude);
            float speed = 1;//crosswind spid
            float surface = part.GetSideSurface();

            return (float)(SIDE_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * surface);           
        }

        public float GetLeftLift(WindPhysics wind)
        {
            float leftLift = GetPartLift(Plane.Configuration.LeftFlap, wind);
            leftLift += GetPartLift(Plane.Configuration.LeftAileron, wind);
            leftLift += GetPartLift(Plane.Configuration.LeftSlat, wind);
            leftLift += GetPartLift(Plane.Configuration.LeftWing, wind);
            return leftLift;
        }

        public float GetRightLift(WindPhysics wind)
        {
            float rightLift = GetPartLift(Plane.Configuration.RightFlap, wind);
            rightLift += GetPartLift(Plane.Configuration.RightAileron, wind);
            rightLift += GetPartLift(Plane.Configuration.RightSlat, wind);
            rightLift += GetPartLift(Plane.Configuration.RightWing, wind);
            return rightLift;
        }

        public float GetLeftDrag(WindPhysics wind)
        {
            float leftDrag = GetPartDrag(Plane.Configuration.LeftFlap, wind);
            leftDrag += GetPartDrag(Plane.Configuration.LeftAileron, wind);
            leftDrag += GetPartDrag(Plane.Configuration.LeftSlat, wind);
            leftDrag += GetPartDrag(Plane.Configuration.LeftWing, wind);
            return leftDrag;
        }

        public float GetRightDrag(WindPhysics wind)
        {
            float rightDrag = GetPartDrag(Plane.Configuration.RightFlap, wind);
            rightDrag += GetPartDrag(Plane.Configuration.RightAileron, wind);
            rightDrag += GetPartDrag(Plane.Configuration.RightSlat, wind);
            rightDrag += GetPartDrag(Plane.Configuration.RightWing, wind);
            return rightDrag;
        }

        public float GetTailDrag(WindPhysics wind) => GetPartDrag(Plane.Configuration.Gear, wind);

        public float GetTailSide(WindPhysics wind) => GetPartSide(Plane.Configuration.Rudder, wind);

        public float GetCentralSide(WindPhysics wind)
        {
            float density = wind.GetDensity(Plane.Flight.Altitude);
            float speed = GetAirspeed();//-wind spid

            float side = (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * Plane.Configuration.LeftWing.GetLiftSurface());
            side += (float)(LIFT_COEFFICENT * ((density * Math.Pow(speed, 2)) / 2) * Plane.Configuration.RightWing.GetLiftSurface());
            side *= (1 - MathF.Pow(MathF.Cos(Plane.Flight.Roll), 2));
            if (Math.Sin(Plane.Flight.Roll) < 0)
                side *= -1;
            return side;
        }
    }
}