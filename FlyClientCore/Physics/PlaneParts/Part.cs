using GeoLib;

namespace ClientCore.Physics.PlaneParts
{
    public class Part
    {
        public Vector2 Offset {get; private set;}

        public Part(Vector2 offset)
        {
            Offset = offset;
        }
    }
}