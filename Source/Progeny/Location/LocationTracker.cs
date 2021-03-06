/*
This file is part of KerbalStats.

KerbalStats is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

KerbalStats is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with KerbalStats.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

using KSP.IO;

namespace KerbalStats.Progeny {
	public class LocationTracker
	{
		public Dictionary<Guid, VesselPart> vessel_parts;

		public AstronautComplex astronaut_complex;
		public EVA eva;
		public Unknown unknown;
		public Wilds wilds;
		public Womb womb;
		public Tomb tomb;

		public Location location (string loc, object parm)
		{
			switch (loc) {
				case "Vessel":
					Vessel v = parm as Vessel;
					if (!vessel_parts.ContainsKey (v.id)) {
						vessel_parts[v.id] = new VesselPart (v);
					}
					return vessel_parts[v.id];
				case "EVA":
					return eva;
				case "Wilds":
					return wilds;
				case "Unknown":
					return unknown;
				case "Womb":
					return womb;
				case "Tomb":
					return tomb;
				case "AstronautComplex":
					return astronaut_complex;
			}
			return null;
		}

		public LocationTracker ()
		{
			vessel_parts = new Dictionary<Guid, VesselPart> ();
			astronaut_complex = new AstronautComplex ();
			eva = new EVA ();
			unknown = new Unknown ();
			wilds = new Wilds ();
			womb = new Womb ();
			tomb = new Tomb ();
		}

		public Location Parse (string locstr)
		{
			try
			{
				string[] parms = locstr.Split(',');
				Location location = null;
				switch (parms[0])
				{
					case "VesselPart":
						Guid id = new Guid(parms[1]);
						if (!vessel_parts.ContainsKey(id))
						{
							Vessel vessel = FlightGlobals.Vessels.Where(v => v.id == id).FirstOrDefault();
							vessel_parts[vessel.id] = new VesselPart(vessel);
						}
						location = vessel_parts[id];
						break;
					case "EVA":
						location = eva;
						break;
					case "Wilds":
						location = wilds;
						break;
					case "Womb":
						location = womb;
						break;
					case "Tomb":
						location = tomb;
						break;
					case "AstronautComplex":
						location = astronaut_complex;
						break;
					default:
						Debug.LogWarningFormat("[KerbalStat] LocationTracker.Parser: {0} is not a recognizable location!", location);
						break;
				}
				return location;
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("Error parsing [{0}]", locstr);
				throw e;
			}
		}
	}
}
