using System;

namespace ClientCore.Cockpit
{
    public class TurnCoordinatorData
    {
        const float MAX_ANGLE = 18;
        const float MAX_YAW_RATE = 0.05f;//360 / (2 * 60 * 60); //360 in 2 min = 2 * 60 sec * 60fps

        public TurnCoordinatorData()
        {
        }

        public float GetAngleForYawRate(float yawRate)
        {
            float scale = yawRate / MAX_YAW_RATE;
            if (scale > 1)
                scale = 1;
            else if (scale < -1)
                scale = -1;

            return scale * MAX_ANGLE;
        }
    }
}