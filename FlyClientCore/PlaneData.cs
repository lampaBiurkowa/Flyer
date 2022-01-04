using Shared.Plane;

namespace ClientCore
{
    public class PlaneData
    {
        public PlaneConfiguration Configuration {get; private set;}
        public FlightData Flight {get; set;} = new FlightData();
        public MachineData Machine {get; private set;}

        public PlaneData(MachineData machineData)
        {
            Machine = machineData;
            Configuration = new PlaneConfiguration(machineData);
        }

        public void Update(float delta)
        {
            Configuration.Update(delta);
        }
    }
}