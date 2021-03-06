using GeoLib;
using Shared.Objects;
using System;

namespace ClientCore.Physics.PlaneParts
{
    public class Gear : Part, IAerodynamic
    {
        GenericSurfaceData data;

        public bool Enabled {get; set;} = false;

        public Gear(Vector2F offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => 0;

        public float GetDragSurface() => Enabled ? data.DragSurface : 0;

        public float GetSideSurface() => 0;
    }
}