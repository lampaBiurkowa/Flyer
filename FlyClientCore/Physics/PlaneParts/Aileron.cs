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

        float accelerationDegrees = 0;
        const float STEP = 0.5f;
        const float MAX_ANGLE = 30f;

        public Aileron(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public void Move(bool increaseAngle)
        {
            if (increaseAngle)
                accelerationDegrees = STEP;
            else
                accelerationDegrees = -STEP;
        }

        public void Update(float delta)
        {
            AngleDegrees = AngleDegrees + accelerationDegrees * delta;
            if (AngleDegrees > MAX_ANGLE)
                AngleDegrees = MAX_ANGLE;
            if (AngleDegrees < -MAX_ANGLE)
                AngleDegrees = -MAX_ANGLE;
        }

        public float GetLiftSurface() => data.LiftSurface * (float)Math.Sin(angleRadians);

        public float GetDragSurface() => GetLiftSurface() * data.LiftSurface * (float)Math.Cos(angleRadians); //risk

        public float GetSideSurface() => 0;
    }
}