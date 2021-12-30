using GeoLib;

namespace Shared
{
    public class FlightData
    {
        const float ALTITUDE_FACTOR = 4;
        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                Altitude = (int)(position.Y * ALTITUDE_FACTOR);
            }
        }
        public Vector3 Rotation {get; private set;}
        public Vector3 Speed {get; private set;}
        public int Altitude {get; private set;}
        
        public FlightData(Vector3 position, Vector3 rotation, Vector3 speed)
        {
            Position = position;
            Rotation = rotation;
            Speed = speed;
        }
    }
}