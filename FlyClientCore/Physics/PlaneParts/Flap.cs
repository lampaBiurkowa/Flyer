using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Flap : Part, IAerodynamic
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
                angleDegrees = (float)GameMath.RadToDeg((double)value);
                angleRadians = value;
            }
        }

        const float MAX_ANGLE = 70;
        public const uint CONFIGURATION_COUNT = 3;

        public int CurrentConfiguration {get; private set;} = 0;

        public Flap(Vector2F offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public void SwitchConfiguration()
        {
            CurrentConfiguration = (int)((CurrentConfiguration + 1) % CONFIGURATION_COUNT);
            AngleDegrees = (MAX_ANGLE / (CONFIGURATION_COUNT - 1)) * CurrentConfiguration;
        }

        public float GetLiftSurface() => data.LiftSurface * MathF.Sin(angleRadians); //kretynizm

        public float GetDragSurface() => data.LiftSurface * MathF.Sin(angleRadians);

        public float GetSideSurface() => 0;
    }
}