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
	using Genome;
	public class Zygote
	{
		// This is an abstraction for all stages of kerbal development, from
		// conception to death.
		public string mother_id;
		public string father_id;
		public Location location;
		protected GenePair[] genes;
		public BioClock bioClock { get; private set; }

		public string id
		{
			get;
			private set;
		}

		void init ()
		{
			bioClock = new BioClock (genes);
		}

		public Zygote (ProtoCrewMember kerbal)
		{
			mother_id = "";
			father_id = "";
			id = ProgenyScenario.current.NextZygoteID ();
			genes = Genome.GetGenes (kerbal);
			init ();
		}

		public Zygote (Female mother, Male father)
		{
			mother_id = mother.id;
			father_id = father.id;
			id = ProgenyScenario.current.NextZygoteID ();
			genes = Genome.Combine (mother.genes, father.genes);
			init ();
		}

		public Zygote (Zygote prevStage)
		{
			mother_id = prevStage.mother_id;
			father_id = prevStage.father_id;
			id = prevStage.id;
			genes = prevStage.genes;
			init ();
		}

		public Zygote (ConfigNode node)
		{
			id = node.GetValue ("id");
			mother_id = node.GetValue ("mother");
			father_id = node.GetValue ("father");
			genes = Genome.ReadGenes (node.GetNode ("genome"));
			Genome.RebuildGenes (null, genes);
			init ();
			var l = ProgenyScenario.current.ParseLocation (node.GetValue ("location"));
			SetLocation (l);
		}

		public virtual void Save (ConfigNode node)
		{
			Debug.Log(String.Format ("[KS Zygote] Save: '{0}' '{1}' '{2}' '{3}' '{4}'", id, mother_id, father_id, genes, location));
			node.AddValue ("id", id);
			node.AddValue ("mother", mother_id);
			node.AddValue ("father", father_id);
			Genome.WriteGenes (genes, node.AddNode ("genome"));
			node.AddValue ("location", location.ToString ());
		}

		public void SetLocation (Location newLocation)
		{
			if (location == newLocation) {
				return;
			}
			if (location != null) {
				location.Remove (this);
			}
			newLocation.Add (this);
			location = newLocation;
		}
	}
}
