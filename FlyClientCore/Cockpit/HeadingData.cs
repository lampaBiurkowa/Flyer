using System;

namespace ClientCore.Cockpit
{
    public class HeadingData
    {
        const float PRECISION = 2;
        float anglePerUnit;

        public HeadingData()
        {
            anglePerUnit = 360 / (10 * MathF.Pow(10, PRECISION));
        }

        public float GetAngleForYaw(float yaw)
        {
            yaw *= (float)Math.Pow(10, PRECISION);
            return yaw * anglePerUnit;
        }
    }
}