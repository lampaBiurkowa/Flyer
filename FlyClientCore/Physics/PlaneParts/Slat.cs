using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Slat : Part, IAerodynamic
    {
        GenericSurfaceData data;

        public bool Enabled {get; private set;} = false;

        public Slat(Vector2 offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => Enabled ? data.LiftSurface : 0;

        public float GetDragSurface() => Enabled ? data.DragSurface : 0;

        public float GetSideSurface() => 0;
    }
}