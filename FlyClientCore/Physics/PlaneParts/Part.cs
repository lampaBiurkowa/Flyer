using GeoLib;

namespace ClientCore.Physics.PlaneParts
{
    public class Part
    {
        public Vector2F Offset {get; private set;}

        public Part(Vector2F offset)
        {
            Offset = offset;
        }
    }
}