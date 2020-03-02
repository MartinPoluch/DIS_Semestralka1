using SimulationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Core;


namespace Test {
	
	class Program {
		static void Main(string[] args) {
			Random seeder = new Random(1);
			int replications = 1000000;
			DiceGame diceGame = new DiceGame(seeder);
			RandomGameMC randomGame = new RandomGameMC(diceGame);
			randomGame.Simulate(replications);
			Console.WriteLine(randomGame.TextResult());
			Console.Read();
		}

	}
}
