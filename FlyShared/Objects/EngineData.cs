namespace Shared.Objects
{
    public class EngineData
    {
        public int MaxSpeed {get; private set;}
        public int RestartAltitude {get; private set;}
        public float Surface {get; private set;}
        public float Acceleration {get; private set;}
        public float Decceleration {get; private set;}
        public EngineData(int maxSpeed, int restartAltitude, int surface, float acceleration, float decceleration)
        {
            MaxSpeed = maxSpeed;
            RestartAltitude = restartAltitude;
            Surface = surface;
            Acceleration = acceleration;
            Decceleration = decceleration;
        }
    }
}