using GeoLib;
using System;
using System.Collections.Generic;

namespace Shared.Objects
{
    public class PlaneData
    {
        public GenericSurfaceData Aileron {get; private set;}
        public GenericSurfaceData Elevator {get; private set;}
        public GenericSurfaceData Flap {get; private set;}
        public GenericSurfaceData Rudder {get; private set;}
        public GenericSurfaceData Slat {get; private set;}
        public GenericSurfaceData Wing {get; private set;}
        public List<Tuple<EngineData, Localization>> Engines {get; private set;}
        public Vector2 Left {get; private set;}
        public Vector2 Right {get; private set;}
        public Vector2 Tail {get; private set;}
        public float Length {get; private set;}

        public PlaneData(GenericSurfaceData aileron, GenericSurfaceData elevator, GenericSurfaceData flap,
        GenericSurfaceData rudder, GenericSurfaceData slat, GenericSurfaceData wing, 
        List<Tuple<EngineData, Localization>> engines, float length)
        {
            Aileron = aileron;
            Elevator = elevator;
            Flap = flap;
            Rudder = rudder;
            Slat = slat;
            Wing = wing;
            Engines = engines;
            Length = length;
            Left = new Vector2(Wing.LiftSurface / 2, 0);
            Right = new Vector2(-Wing.LiftSurface / 2, 0);
            Tail = new Vector2(0, Length);
        }
    }
}