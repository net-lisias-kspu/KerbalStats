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
using System.Linq;

namespace KerbalStats.Progeny {
	using Genome;

	public class BioClockTC : Trait
	{
		public string name { get { return "BioClockTC"; } }
		public int GeneSize { get { return 4; } }

		int CountBits (uint x)
		{
			uint count = 0;
			while (x > 0) {
				count += x & 1;
				x >>= 1;
			}
			return (int) count;
		}

		public GenePair CreateGene ()
		{
			uint a = (uint) UnityEngine.Random.Range (0, 16);
			uint b = (uint) UnityEngine.Random.Range (0, 16);
			return new GenePair (this, a, b);
		}

		public GenePair CreateGene (string bio)
		{
			return CreateGene ();
		}

		public GenePair CreateGene (ProtoCrewMember pcm)
		{
			var gene = Genome.Prefab (this, pcm);
			if (gene == null) {
				gene = CreateGene ();
			}
			return gene;
		}

		public string CreateValue (GenePair gene)
		{
			var c = gene.a & gene.b;
			return c.ToString ();
		}

		double factor (GenePair bc, GenePair bci, int ind, double time)
		{
			uint c = bc.a & bc.b;
			var k = CountBits (c & (uint)(0xf >> (3 - ind)));
			var n = (bci.trait as BioClockInverse).Inverse (bci);
			var o = k > 0 ? 1 : 0;
			var x0 = Math.Log (ProgenySettings.EggLife);
			var x1 = Math.Log (ProgenySettings.AgingTime);
			var x = Math.Abs (Math.Log (time) - x0);
			var m = 0.2 / (x1 - x0);
			var y = o * n * m * Math.Pow (x, 1 + 0.2 * k);
			return Math.Exp (y);
		}

		public double SpermLife (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.SpermLife;
			return time * factor (bc, bci, 0, time * 1.2);
		}

		public double EggLife (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.EggLife;
			return time * factor (bc, bci, 0, time * 1.2);
		}

		public double OvulationTime (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.OvulationTime;
			return time * factor (bc, bci, 0, time);
		}

		public double CyclePeriod (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.CyclePeriod;
			return time * factor (bc, bci, 0, time);
		}

		public double GestationPeriod (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.GestationPeriod;
			return time * factor (bc, bci, 1, time);
		}

		public double MaturationTime (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.MaturationTime;
			return time * factor (bc, bci, 2, time);
		}

		public double AgingTime (GenePair bc, GenePair bci)
		{
			var time = ProgenySettings.AgingTime;
			return time * factor (bc, bci, 3, time);
		}
	}
}
