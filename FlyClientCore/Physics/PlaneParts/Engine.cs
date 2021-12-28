using GeoLib;
using Shared.Objects;

namespace ClientCore.Physics.PlaneParts
{
    public class Engine : Part
    {
        public EngineData Data {get; private set;}
        public float CurrentSpeed {get; private set;} = 0;
        
        public Engine(Vector2 offset, EngineData data) : base(offset)
        {
            Data = data;
        }

        public void Update(bool thrustUp, float delta)
        {
            if (thrustUp && CurrentSpeed < Data.MaxSpeed)
                CurrentSpeed += Data.Acceleration * delta;
            else if (!thrustUp && CurrentSpeed > 0)
                CurrentSpeed -= Data.Decceleration * delta;

            if (CurrentSpeed > Data.MaxSpeed)
                CurrentSpeed = Data.MaxSpeed;
            if (CurrentSpeed < 0)
                CurrentSpeed = 0;
        }

        public float GetThrust(float delta, float airDensity)
        { //-windSpeed
            float flowMass = Data.Surface * airDensity * CurrentSpeed;
            return flowMass * CurrentSpeed;
        }
    }
}