//po co to
using GeoLib;
using Shared;
using Shared.Objects;

namespace ClientCore
{
    public class Flight
    {
        public PlaneData PlaneData {get; private set;}
        public FlightData FlightData {get; private set;}
        public float Mass {get; private set;}

        public Flight(PlaneData plane, FlightData flight, float m)
        {
            PlaneData = plane;
            FlightData = flight;
            Mass = m;
        }
    }
}