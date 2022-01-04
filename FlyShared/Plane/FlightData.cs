using GeoLib;

namespace Shared.Plane
{
    public class FlightData
    {
        const float ALTITUDE_FACTOR = 4;
        private Vector3F position;
        public Vector3F Position
        {
            get => position;
            set
            {
                position = value;
                Altitude = (int)(position.Y * ALTITUDE_FACTOR);
            }
        }
        public Vector3F RotationRadians {get; private set;}
        public Vector3F Speed {get; private set;}
        public int Altitude {get; private set;}

        public float Roll {get; private set;} = 0;
        public float Pitch {get; private set;} = 0;
        public float Yaw {get; private set;} = 0;
        
        public FlightData(Vector3F position, Vector3F rotation, Vector3F speed)
        {
            Position = position;
            RotationRadians = rotation;
            Speed = speed;
		    Roll = rotation.Z ;//+ (float)GeoLib.GameMath.DegToRad(180);
		    Pitch = rotation.X;
		    Yaw = rotation.Y;
        }

        public FlightData()
        {
            Position = new Vector3F(0, 0, 0);
            RotationRadians = new Vector3F(0, 0, 0);
            Speed = new Vector3F(0, 0, 0);
        }
    }
}