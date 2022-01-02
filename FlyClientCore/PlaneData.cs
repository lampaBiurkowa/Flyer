using ClientCore.Physics.PlaneParts;
using GeoLib;
using System.Collections.Generic;
using Shared.Plane;
using Shared.Objects;

namespace ClientCore
{
    public class PlaneData
    {
        public Aileron LeftAileron {get; private set;}
        public Aileron RightAileron {get; private set;}
        public Elevator Elevator {get; private set;}
        public List<Engine> Engines {get; private set;} = new List<Engine>();
        public Flap LeftFlap {get; private set;}
        public Flap RightFlap {get; private set;}
        public Gear Gear {get; private set;}
        public Rudder Rudder {get; private set;}
        public Slat LeftSlat {get; private set;}
        public Slat RightSlat {get; private set;}
        public Wing LeftWing {get; private set;}
        public Wing RightWing {get; private set;}
        public Brakes Brakes {get; private set;} = new Brakes();

        public PlaneData(MachineData data)
        {
            LeftAileron = new Aileron(data.Left, data.Aileron);
            RightAileron = new Aileron(data.Right, data.Aileron);
            Elevator = new Elevator(data.Tail, data.Elevator);
            Gear = new Gear(data.Tail, data.Gear);
            Engines = new List<Engine>();
            foreach (var e in data.Engines)
            {
                Vector2 offset;
                if (e.Item2 == Localization.LEFT)
                    offset = data.Left;
                else if (e.Item2 == Localization.RIGHT)
                    offset = data.Right;
                else
                    offset = data.Tail;
                
                Engines.Add(new Engine(offset, e.Item1));
            }
            LeftFlap = new Flap(data.Left, data.Flap);
            RightFlap = new Flap(data.Right, data.Flap);
            Rudder = new Rudder(data.Tail, data.Rudder);
            LeftSlat = new Slat(data.Left, data.Slat);
            RightSlat = new Slat(data.Right, data.Slat);
            LeftWing = new Wing(data.Left, data.Wing);
            RightWing = new Wing(data.Right, data.Wing);
        }

        public void Update(float delta)
        {
            Elevator.Update(delta);
            Rudder.Update(delta);
            LeftAileron.Update(delta);
            RightAileron.Update(delta);
        }
    }
}