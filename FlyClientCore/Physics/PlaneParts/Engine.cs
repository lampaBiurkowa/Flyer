using GeoLib;
using Shared.Objects;

namespace ClientCore.Physics.PlaneParts
{
    public class Engine : Part
    {
        const float DOUBLED_FUEL_PERCENTAGE_TRESHOLD = 0.9f;
        public const float FUEL_MASS = 10;
        public EngineData Data {get; private set;}
        public float CurrentSpeed {get; private set;} = 0;
        public float RemainingFuel {get; private set;}
        
        public Engine(Vector2 offset, EngineData data) : base(offset)
        {
            Data = data;
            RemainingFuel = data.FuelCapacity;
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
            if (CurrentSpeed / Data.MaxSpeed > DOUBLED_FUEL_PERCENTAGE_TRESHOLD)
                RemainingFuel -= 2 * Data.FuelPerSecond * delta;
            else if (CurrentSpeed > 0)
                RemainingFuel -= Data.FuelPerSecond * delta;

            if (RemainingFuel < 0)
                RemainingFuel = 0;
        }

        public float GetThrust(float delta, float airDensity)
        { //-windSpeed
            float flowMass = Data.Surface * airDensity * CurrentSpeed;
            return flowMass * CurrentSpeed;
        }
    }
}