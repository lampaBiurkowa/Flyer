using System;

namespace ClientCore.Cockpit
{
    public class VerticalSpeedData
    {
        const float MIN_SPEED = 0;
        const float MAX_SPEED = 20;
        const float PRECISION = 1;
        float anglePerUnit;

        public VerticalSpeedData()
        {
            anglePerUnit = 180 / (10 * MathF.Pow(10, PRECISION));
        }

        public float GetAngleForSpeed(float speed)
        {
            int speedSign = MathF.Sign(speed);
            float speedAbs = MathF.Abs(speed);
            if (speedAbs > MAX_SPEED)
                speedAbs = MAX_SPEED;
            else if (speedAbs < MIN_SPEED)
                speedAbs = MIN_SPEED;
            float scale = ((speedAbs - MIN_SPEED) / MAX_SPEED) * 100;
            return scale * anglePerUnit * speedSign;
        }
    }
}