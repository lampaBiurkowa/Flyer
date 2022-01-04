using GeoLib;

namespace ClientCore.Physics
{
    public interface IMoveable
    {
        Vector3F Direction {get;}
        Vector3F GetSpeed();
    }
}