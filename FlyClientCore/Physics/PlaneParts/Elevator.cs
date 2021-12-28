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
            private set
            { 
                angleDegrees = value;
                angleRadians = (float)GameMath.DegToRad((double)value);
            }
        }

        private float angleRadians = 0;
        public float AngleRadians
        {
            get => angleRadians;
            private set
            { 
                angleDegrees = value;
                angleRadians = (float)GameMath.DegToRad((double)value);
            }
        }

        public const float STEP = 0.8f;
        float accelerationDegrees = 0;

        public Elevator(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public void Move(bool pitchUp)
        {
            if (pitchUp)
                accelerationDegrees += STEP;
            else
                accelerationDegrees -= STEP;
        }

        public void Update(float delta)
        {
            AngleDegrees = AngleDegrees + accelerationDegrees * delta;
            if (AngleDegrees > 90)
                AngleDegrees = 90;
            if (AngleDegrees < -90)
                AngleDegrees = -90;
        }

        public float GetLiftSurface() => data.LiftSurface * (float)(Math.Sin(angleRadians));

        public float GetDragSurface() => 0;//data.LiftSurface * (float)(Math.Sin(angleRadians));

        public float GetSideSurface() => 0;
    }
}