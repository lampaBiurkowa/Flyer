using System;

namespace ClientCore.Cockpit
{
    public class HeadingData
    {
        const float PRECISION = 0;
        float anglePerUnit;

        public HeadingData()
        {
            anglePerUnit = 1 / MathF.Pow(10, PRECISION);
        }

        public float GetAngleForYaw(float yaw)
        {
            yaw *= (float)Math.Pow(10, PRECISION);
            return yaw * anglePerUnit;
        }
    }
}