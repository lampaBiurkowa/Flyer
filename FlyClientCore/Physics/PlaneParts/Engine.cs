using GeoLib;
using Shared.Objects;

namespace ClientCore.Physics.PlaneParts
{
    public class Engine : Part
    {
        public EngineData Data {get; private set;}
        public Fuel Fuel {get; private set;}
        public float CurrentSpeed {get; private set;} = 0;
        
        public Engine(Vector2F offset, EngineData data, Fuel fuel) : base(offset)
        {
            Data = data;
            Fuel = fuel;
        }

        public void UpdateThrust(bool thrustUp, float delta)
        {
            updateThrust(thrustUp, delta);
            normalizeThrust();
        }

        void updateThrust(bool thrustUp, float delta)
        {
            if (thrustUp && CurrentSpeed < Data.MaxSpeed)
                CurrentSpeed += Data.Acceleration * delta;
            else if (!thrustUp && CurrentSpeed > 0)
                CurrentSpeed -= Data.Decceleration * delta;
        }

        void normalizeThrust()
        {
            if (CurrentSpeed > Data.MaxSpeed)
                CurrentSpeed = Data.MaxSpeed;
            if (CurrentSpeed < 0)
                CurrentSpeed = 0;
        }

        public void UpdateFuel(float delta)
        {
            Fuel.Update(CurrentSpeed / Data.MaxSpeed, delta);
        }

        public float GetThrust(float delta, float airDensity)
        { //-windSpeed
            float flowMass = Data.Surface * airDensity * CurrentSpeed;
            return flowMass * CurrentSpeed;
        }
    }
}