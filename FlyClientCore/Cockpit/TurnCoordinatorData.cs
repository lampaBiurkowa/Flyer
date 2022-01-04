using GeoLib;
using System;

namespace ClientCore.Cockpit
{
    public class TurnCoordinatorData
    {
        const float MAX_ANGLE = 18;
        const float MAX_YAW_RATE = 0.05f;//360 / (2 * 60 * 60); //360 in 2 min = 2 * 60 sec * 60fps
        const float BAR_LENGTH = 180;
        const float BAR_HEIGHT = 8;
        const float BAR_FLAT_LENGTH = 40;
        const float MAX_BUBBLE_YAW = 30;

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

        public Vector2F GetBubblePosition(float yaw, float roll)
        {
            if (roll > 0)
                yaw *= -1;
            
            if (yaw > MAX_BUBBLE_YAW)
                yaw = MAX_BUBBLE_YAW;
            else if (yaw < -MAX_BUBBLE_YAW)
                yaw = -MAX_BUBBLE_YAW;
            
            float scale = yaw / MAX_BUBBLE_YAW;
            float x = scale * (BAR_LENGTH / 2);
            float y = MathF.Abs(x) < (BAR_FLAT_LENGTH / 2) ? 0 : -(x - BAR_FLAT_LENGTH / 2) / BAR_HEIGHT;
            return new Vector2F(x, y);
        }
    }
}