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
	public class Female : Adult, IComparable<Female>
	{
		double lastUpdate;
		double UT;
		Embryo embryo;
		Interest interest;
		Gamete gamete;
		Cycle cycle;

		FemaleFSM fsm;

		public bool isInterested ()
		{
			var p = UnityEngine.Random.Range (0, 1f);
			return p < interest.isInterested (UT);
		}

		public double GameteLife ()
		{
			var p = UnityEngine.Random.Range (0, 1f);
			return gamete.Life (p);
		}

		public Male SelectMate (List<Male> males)
		{
			float [] male_readiness = new float[males.Count + 1];
			male_readiness[0] = cycle.NonmatingFactor (UT);
			for (int i = 0; i < males.Count; i++) {
				male_readiness[i + 1] = males[i].isInterested (UT);
			}
			var dist = new Genome.DiscreteDistribution (male_readiness);
			int ind = dist.Value (UnityEngine.Random.Range (0, 1f)) - 1;
			if (ind < 0) {
				return null;
			}
			return males[ind];
		}

		public bool Mate (Male mate)
		{
			mate.Mate (UT);
			interest.Mate (UT);
			var ot = cycle.OvulationTime;
			float fv = 0, mv = 0;
			if (UT < ot) {
				mv = mate.gamete.Viability (ot - UT);
				fv = gamete.Viability (0);
			} else {
				mv = mate.gamete.Viability (0);
				fv = gamete.Viability (ot - UT);
			}
			float conceive_chance = fv * mv;
			if (UnityEngine.Random.Range (0, 1f) > conceive_chance) {
				return false;
			}
			embryo = new Embryo (this, mate);
			ProgenyScenario.current.AddEmbryo (embryo);
			return true;
		}

		void initialize ()
		{
			lastUpdate = Planetarium.GetUniversalTime ();
			fsm = new FemaleFSM (this);

			interest = new Interest (genes);
			gamete = new Gamete (genes, true, bioClock);
			cycle = new Cycle (genes, bioClock);
			embryo = null;
		}

		public Female (Juvenile juvenile) : base (juvenile)
		{
			initialize ();
			fsm.StartFSM ("Fertile");
		}

		public Female (ProtoCrewMember kerbal) : base (kerbal)
		{
			initialize ();
			fsm.StartFSM ("Fertile");
		}

		public Female (ConfigNode node) : base (node)
		{
			initialize ();
			interest.Load (node);
			cycle.Load (node);
			if (node.HasValue ("state")) {
				fsm.StartFSM (node.GetValue ("state"));
			} else {
				fsm.StartFSM ("Fertile");
			}
			if (node.HasValue ("embryo")) {
				var zid = node.GetValue ("embryo");
				embryo = ProgenyScenario.current.GetEmbryo (zid);
			}
		}

		public override void Save (ConfigNode node)
		{
			base.Save (node);
			interest.Save (node);
			cycle.Save (node);
			node.AddValue ("state", fsm.currentStateName);
			if (embryo != null) {
				node.AddValue ("embryo", embryo.id);
			}
		}

		public void Update ()
		{
			UT = Planetarium.GetUniversalTime ();
			if (UT - lastUpdate < 3600) {
				return;
			}
			cycle.Update (UT);
			fsm.UpdateFSM ();
			lastUpdate = UT;
		}

		public string State
		{
			get {
				UT = Planetarium.GetUniversalTime ();
				return fsm.currentStateName + " " + interest.isInterested (UT);
			}
		}

		public int CompareTo (Female other)
		{
			return name.CompareTo (other.name);
		}
	}
}
