using GeoLib;

namespace ClientCore.Physics
{
    public interface IMoveable
    {
        Vector3 Direction {get;}
        Vector3 Speed {get;}
    }
}