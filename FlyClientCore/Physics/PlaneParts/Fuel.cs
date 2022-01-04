namespace ClientCore.Physics.PlaneParts
{
    public class Fuel
    {
        const float DOUBLED_USE_PERCENTAGE_TRESHOLD = 0.9f;
        const float FUEL_PER_SECOND = 0.01f;
        const float MASS_PER_UNIT = 10f;
        public float Remaining {get; private set;}

        public Fuel(float startFuel)
        {
            Remaining = startFuel;
        }

        public void Update(float speedPercentage, float delta)
        {
            if (speedPercentage > DOUBLED_USE_PERCENTAGE_TRESHOLD)
                Remaining -= 2 * FUEL_PER_SECOND * delta;
            else if (speedPercentage > 0)
                Remaining -= FUEL_PER_SECOND * delta;

            if (Remaining < 0)
                Remaining = 0;
        }

        public float GetMass() => Remaining * MASS_PER_UNIT;
    }
}