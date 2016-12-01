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

namespace KerbalStats {
	public static class MathUtil {
		public static double WeibullCDF (double l, double k, double x)
		{
			return 1 - Math.Exp (-Math.Pow (x/l, k));
		}

		public static double WeibullQF (double l, double k, double p)
		{
			// t = l * (-ln(1-p)) ^ 1/k
			//ugh, why does .net not have log1p? Not that I expect the
			// random number generator to give that small a p
			return l * Math.Pow (-Math.Log (1 - p), 1/k);
		}

		public static float[] Binomial (float p, int n)
		{
			float q = 1 - p;
			float[] dist = new float[n + 1];
			float P = 1;
			float Q = 1;

			dist[0] = 1;
			for (int i = 0; i < n; i++) {
				dist[i + 1] = dist[i] * (n - i) / (i + 1);
				dist[i] *= P;
				P *= p;
			}
			dist[n] *= P;
			for (int i = n; i >= 0; i--) {
				dist[i] *= Q;
				Q *= q;
			}
			return dist;
		}

	}
}
