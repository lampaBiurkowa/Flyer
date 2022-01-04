using ClientCore.Physics.PlaneParts;
using GeoLib;
using System.Collections.Generic;
using Shared.Plane;
using Shared.Objects;

namespace ClientCore
{
    public class PlaneConfiguration
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

        public PlaneConfiguration(MachineData machineData)
        {
            initializeAilerons(machineData);
            initializeElevator(machineData);
            initializeGear(machineData);
            initializeEngines(machineData);
            initializeFlaps(machineData);
            initializeRudder(machineData);
            initializeSlats(machineData);
            initializeWings(machineData);
        }

        void initializeAilerons(MachineData machineData)
        {
            LeftAileron = new Aileron(machineData.Left, machineData.Aileron);
            RightAileron = new Aileron(machineData.Right, machineData.Aileron);
        }

        void initializeElevator(MachineData machineData)
        {
            Elevator = new Elevator(machineData.Tail, machineData.Elevator);
        }

        void initializeGear(MachineData machineData)
        {
            Gear = new Gear(machineData.Tail, machineData.Gear);
        }

        void initializeEngines(MachineData machineData)
        {
            Engines = new List<Engine>();
            foreach (var e in machineData.Engines)
            {
                Vector2F offset;
                if (e.Item2 == Localization.LEFT)
                    offset = machineData.Left;
                else if (e.Item2 == Localization.RIGHT)
                    offset = machineData.Right;
                else
                    offset = machineData.Tail;
                
                Engines.Add(new Engine(offset, e.Item1, new Fuel(15))); //:D/
            }
        }

        void initializeFlaps(MachineData machineData)
        {
            LeftFlap = new Flap(machineData.Left, machineData.Flap);
            RightFlap = new Flap(machineData.Right, machineData.Flap);
        }

        void initializeRudder(MachineData machineData)
        {
            Rudder = new Rudder(machineData.Tail, machineData.Rudder);
        }

        void initializeSlats(MachineData machineData)
        {
            LeftSlat = new Slat(machineData.Left, machineData.Slat);
            RightSlat = new Slat(machineData.Right, machineData.Slat);
        }

        void initializeWings(MachineData machineData)
        {
            LeftWing = new Wing(machineData.Left, machineData.Wing);
            RightWing = new Wing(machineData.Right, machineData.Wing);
        }

        public void Update(float delta)
        {
            Elevator.Update(delta);
            Rudder.Update(delta);
            LeftAileron.Update(delta);
            RightAileron.Update(delta);
            foreach (var e in Engines)
                e.UpdateFuel(delta);
        }
    }
}