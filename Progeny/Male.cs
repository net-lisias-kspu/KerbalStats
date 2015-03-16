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
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

using KSP.IO;

namespace KerbalStats.Progeny {
	public class Male
	{
		ProtoCrewMember kerbal;
		bool update_pending;
		ProtoCrewMember.RosterStatus oldStatus;

		public string name
		{
			get {
				return kerbal.name;
			}
		}

		public Male (ProtoCrewMember kerbal)
		{
			this.kerbal = kerbal;
			GameEvents.onKerbalStatusChange.Add (onKerbalStatusChange);
		}

		public Male (ProtoCrewMember kerbal, ConfigNode progeny)
		{
			this.kerbal = kerbal;
			GameEvents.onKerbalStatusChange.Add (onKerbalStatusChange);
		}

		internal void Save (ConfigNode progeny)
		{
		}

		~Male ()
		{
			GameEvents.onKerbalStatusChange.Remove (onKerbalStatusChange);
		}

		void onKerbalStatusChange (ProtoCrewMember kerbal, ProtoCrewMember.RosterStatus oldStatus, ProtoCrewMember.RosterStatus newStatus)
		{
			if (kerbal != this.kerbal || newStatus == oldStatus) {
				return;
			}
			KSProgenyRunner.instance.StartCoroutine (DelayStatusUpdate ());
		}

		internal IEnumerator<YieldInstruction> DelayStatusUpdate ()
		{
			if (update_pending) {
				yield break;
			}
			oldStatus = kerbal.rosterStatus;
			update_pending = true;
			yield return null;
			yield return null;
			if (kerbal.rosterStatus == oldStatus) {
				yield break;
			}
		}
	}
}
