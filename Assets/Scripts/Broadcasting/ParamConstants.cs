using UnityEngine;
using System.Collections;

/*
 * Holder for event names
 * Created By: NeilDG
 */ 
public class ParamConstants {
	public class Marker_Names {
		public const string PEBBLES = "PEBBLES";
		public const string WOODCHIPS = "WOODCHIPS";
		public const string GRASS = "GRASS";
		public const string ASPHALT = "ASPHALT";
	}

    public class Tracking_Modes {
		public const string ZERO_MARKER_MODE = "ZERO";
		public const string ONE_MARKER_MODE = "ONE";
		public const string TWO_MARKER_MODE = "TWO";
		public const string FOUR_MARKER_MODE = "FOUR";
	}

    public class Extra_Keys {
        public const string MARKER_NAME = "MARKER_NAME";
        public const string MARKER_MODE = "MARKER_MODE";
        public const string MARKER_STATUS = "MARKER_STATUS";
    }
}