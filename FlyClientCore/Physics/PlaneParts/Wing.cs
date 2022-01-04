using GeoLib;
using Shared.Objects;

namespace ClientCore.Physics.PlaneParts
{
    public class Wing : Part, IAerodynamic
    {
        GenericSurfaceData data;

        public Wing(Vector2F offset, GenericSurfaceData data) : base(offset)
        {
            this.data = data;
        }

        public float GetLiftSurface() => data.LiftSurface;

        public float GetDragSurface() => data.DragSurface;

        public float GetSideSurface() => 0;
    }
}