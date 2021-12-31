namespace ClientCore.Cockpit
{
    public class AirspeedData
    {
        const float MIN_SPEED = 20;
        const float MAX_SPEED = 260;
        const float ANGLE_PER_UNIT = 360 / (MAX_SPEED - MIN_SPEED);
        
        public AirspeedData()
        {
        }

        public float GetAngleForSpeed(float speed)
        {
            if (speed > MAX_SPEED)
                speed = MAX_SPEED;
            else if (speed < MIN_SPEED)
                speed = MIN_SPEED;
            return (speed - MIN_SPEED) * ANGLE_PER_UNIT;
        }
    }
}