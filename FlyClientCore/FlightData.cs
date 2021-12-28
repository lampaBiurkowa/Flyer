using GeoLib;

namespace ClientCore
{
    public class FlightData
    {
        const float ALTITUDE_FACTOR = 2;
        public Vector3 Position {get; private set;}
        private Vector3 rotation;
        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                Altitude = (int)(rotation.Y * ALTITUDE_FACTOR);
            }
        }
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