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

	public class InterestTC : Trait
	{
		public string name { get { return "InterestTC"; } }
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
			return new GenePair (this as Trait, a, b);
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

		static double[] f = { 0.5, 1.0, 1.5, 2.0, 3.0 };

		public string CreateValue (GenePair gene)
		{
			var c = CountBits (gene.a & gene.b);
			double tc = ProgenySettings.InterestTC * f[c];
			return tc.ToString ();
		}
	}
}
