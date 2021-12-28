namespace Shared.Objects
{
    public class GenericSurfaceData
    {
        public int DragSurface {get; private set;}
        public int LiftSurface {get; private set;}
        public int SideSurface {get; private set;}
        
        public GenericSurfaceData(int dragSurface, int liftSurface, int sideSurface)
        {
            DragSurface = dragSurface;
            LiftSurface = liftSurface;
            SideSurface = sideSurface;
        }
    }
}