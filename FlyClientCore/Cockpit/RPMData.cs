using System;

namespace ClientCore.Cockpit
{
    public class RPMData
    {
        const float PRECISION = 1;
        const float SMALL_ARROW_MAX_ANGLE = 360;
        const float BIG_ARROW_MAX_ANGLE = 300;
        const float BIG_ARROW_MAX_VALUE = 100;
        float anglePerUnitBigArrow;
        float anglePerUnitSmallArrow;
        public RPMData()
        {
            anglePerUnitSmallArrow = SMALL_ARROW_MAX_ANGLE / MathF.Pow(10, PRECISION);
            anglePerUnitBigArrow = (BIG_ARROW_MAX_ANGLE / BIG_ARROW_MAX_VALUE) / MathF.Pow(10, PRECISION);
        }

        public float GetSmallArrowAngleForNumber(float number)
        {
            number *= MathF.Pow(10, PRECISION);
            return number * anglePerUnitSmallArrow;
        }

        public float GetBigArrowAngleForNumber(float percentage)
        {
            percentage *= MathF.Pow(10, PRECISION);
            return percentage * anglePerUnitBigArrow;
        }
    }
}