using SimulationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test {
	/**
	 * 19818
	 */
	class Program {
		static void Main(string[] args) {
			Random seeder = new Random(1);
			int replications = 1000000;
			DiceGameMC diceGame = new DiceGameMC(seeder);
			diceGame.Simulate(replications);
			Console.WriteLine(diceGame.TextResult());
			Console.Read();
		}

	}
}
