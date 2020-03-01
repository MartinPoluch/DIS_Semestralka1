using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.Generators {
	public class UniformRNG {

		private readonly int _minValue;
		private readonly int _maxValue;
		private readonly Random random;

		public UniformRNG(int seed, int minValue, int maxValue) {
			_minValue = minValue;
			_maxValue = maxValue;
			Seed = seed;
			random = new Random(seed);
		}

		public int Seed { get;}

		/// <summary>
		/// Generuje cisla od minimalnej hodnoty (vratane) po maximalnu hodnotu (VRATANE !!!)
		/// </summary>
		public int NextInt() {
			return random.Next(_minValue, _maxValue + 1);
		}
	}
}
