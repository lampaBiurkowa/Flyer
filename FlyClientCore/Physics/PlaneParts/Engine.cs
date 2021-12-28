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

        public void Update(float speed)
        {
            if (speed < Data.MaxSpeed)
                CurrentSpeed = speed;
        }

        public float GetThrustAndUpdate(float delta, float speed, float airDensity)
        {
            float mass = Data.Surface * airDensity * speed;
            float acceleration = (speed - CurrentSpeed) / delta;
            CurrentSpeed = speed;
            return mass * acceleration;
        }
    }
}