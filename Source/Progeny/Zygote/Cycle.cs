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

	public class Cycle
	{
		double cycleK;
		PRange cycleP;
		double cycleL;

		double ovulationK;
		PRange ovulationP;
		double ovulationL;

		double cycle_start;
		double cycle_end;
		double ovulation_time;

		public Cycle (GenePair[] genes, BioClock bioClock)
		{
			for (int i = 0; i < genes.Length; i++) {
				var g = genes[i];
				switch (g.trait.name) {
					case "CyclePeriodK":
						cycleK = (g.trait as TimeK).K (g);
						break;
					case "CyclePeriodP":
						cycleP = (g.trait as TimeP).P (g);
						break;
					case "OvulationTimeK":
						ovulationK = (g.trait as TimeK).K (g);
						break;
					case "OvulationTimeP":
						ovulationP = (g.trait as TimeP).P (g);
						break;
				}
			}

			cycleL = bioClock.CyclePeriod;
			ovulationL = bioClock.OvulationTime;
		}

		double CalcCyclePeriod (double p)
		{
			p = cycleP.P (p);
			return MathUtil.WeibullQF (cycleL, cycleK, p);
		}

		double CalcOvulationTime (double p)
		{
			p = ovulationP.P (p);
			return MathUtil.WeibullQF (ovulationL, ovulationK, p);
		}

		public void Update (double UT)
		{
			float p;

			while (cycle_end < UT) {
				// hopefully not too many cycles have gone between the last
				// update and now
				p = UnityEngine.Random.Range (0, 1f);
				cycle_start = cycle_end;
				cycle_end = cycle_start + CalcCyclePeriod (p);
			}
			p = UnityEngine.Random.Range (0, 1f);
			ovulation_time = cycle_start + CalcOvulationTime (p);
		}

		public double OvulationTime { get { return ovulation_time; } }

		public float NonmatingFactor (double UT)
		{
			// the lower this is, the more likely the female is to mate
			// put the peak of interest (or trough of rejection) a little
			// before ovulation
			var x = 1.75 * (UT - cycle_start) / (ovulation_time - cycle_start);
			return (float) (1 - x * x * Math.Exp (-x));
		}

		public void Load (ConfigNode node)
		{
			if (node.HasValue ("cycle_start")) {
				double.TryParse (node.GetValue ("cycle_start"), out cycle_start);
			}
			if (node.HasValue ("cycle_end")) {
				double.TryParse (node.GetValue ("cycle_end"), out cycle_end);
			}
			if (node.HasValue ("ovulation_time")) {
				double.TryParse (node.GetValue ("ovulation_time"), out ovulation_time);
			}
		}

		public void Save (ConfigNode node)
		{
			node.AddValue ("cycle_start", cycle_start.ToString ("G17"));
			node.AddValue ("cycle_end", cycle_end.ToString ("G17"));
			node.AddValue ("ovulation_time", ovulation_time.ToString ("G17"));
		}
	}
}
