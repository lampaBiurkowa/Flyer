using System;

namespace ClientCore.Cockpit
{
    public class AltimeterData
    {
        const float PRECISION = 2;
        public int DigitsToDisplay => 5;
        float anglePerUnit;
        public AltimeterData()
        {
            anglePerUnit = 360 / (10 * MathF.Pow(10, PRECISION));
        }

        public float GetAngleForNumber(float number)
        {
            number *= MathF.Pow(10, PRECISION);
            return number * anglePerUnit;
        }
    }
}