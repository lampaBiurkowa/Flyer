using System;

namespace ClientCore.Cockpit
{
    public class FuelData
    {
        const float MIN_VALUE = 0;
        const float MAX_VALUE = 15;
        const float PRECISION = 2;
        const float MAX_ANGLE = 90;
        float anglePerUnit;
        public float OriginX => 300;
        public float OriginY => 397;

        public FuelData()
        {
            anglePerUnit = MAX_ANGLE / (10 * MathF.Pow(10, PRECISION));
        }

        public float GetAngleForFuel(float fuel)
        {
            if (fuel < MIN_VALUE)
                fuel = MIN_VALUE;
            else if (fuel > MAX_VALUE)
                fuel = MAX_VALUE;
            float scale = fuel / MAX_VALUE;
            return MAX_ANGLE * scale;
        }
    }
}