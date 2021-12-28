using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Aileron : Part, IAerodynamic
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

        public Aileron(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => data.LiftSurface * (float)Math.Sin(angleRadians);

        public float GetDragSurface() => GetLiftSurface() * data.LiftSurface * (float)Math.Cos(angleRadians); //risk

        public float GetSideSurface() => 0;
    }
}