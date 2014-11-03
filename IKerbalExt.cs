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
using System.Linq;
using UnityEngine;

namespace KerbalStats {
	public interface IKerbalExt
	{
		string name { get; }
		void AddKerbal (ProtoCrewMember kerbal);
		void RemoveKerbal (ProtoCrewMember kerbal);
		void Load (ProtoCrewMember kerbal, ConfigNode node);
		void Save (ProtoCrewMember kerbal, ConfigNode node);

		string Get (ProtoCrewMember kerbal, string parms);
	}
}
