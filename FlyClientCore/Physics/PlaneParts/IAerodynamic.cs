using GeoLib;

namespace ClientCore.Physics.PlaneParts
{
    public interface IAerodynamic
    {
        float GetLiftSurface();
        float GetDragSurface();
        float GetSideSurface();
    }
}