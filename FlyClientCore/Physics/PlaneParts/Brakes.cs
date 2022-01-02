namespace ClientCore.Physics.PlaneParts
{
    public class Brakes
    {
        public float Decceleration => 0.999f;
        public bool Enabled {get; set;} = false;

        public Brakes()
        {
        }
    }
}