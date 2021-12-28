using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Elevator : Part, IAerodynamic
    {
        GenericSurfaceData data;

        private float angleDegrees = 0;
        public float AngleDegrees
        {
            get => angleDegrees;
            set
            { 
                angleDegrees = value;
                angleRadians = (float)GameMath.DegToRad((double)value);
            }
        }

        private float angleRadians = 0;
        public float AngleRadians
        {
            get => angleRadians;
            set
            { 
                angleDegrees = value;
                angleRadians = (float)GameMath.DegToRad((double)value);
            }
        }

        public Elevator(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => data.LiftSurface * (float)(Math.Cos(angleRadians));

        public float GetDragSurface() => data.LiftSurface * (float)(Math.Sin(angleRadians));

        public float GetSideSurface() => 0;
    }
}