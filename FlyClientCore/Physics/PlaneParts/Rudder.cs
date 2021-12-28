using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Rudder : Part, IAerodynamic
    {
        const float ANGLE = 30;
        GenericSurfaceData data;

        public Localization Localization {get; private set;} = Localization.CENTER;

        public Rudder(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => 0;

        public float GetDragSurface() => 0;

        public float GetSideSurface()
        {
            if (Localization == Localization.CENTER)
                return 0;
            if (Localization == Localization.LEFT)
                return data.SideSurface * (float)Math.Cos(ANGLE) - data.SideSurface * (float)(Math.Sin(ANGLE));
            
            return -data.SideSurface * (float)Math.Cos(ANGLE) + data.SideSurface * (float)(Math.Sin(ANGLE));
        }
    }
}