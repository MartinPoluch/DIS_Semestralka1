using SimulationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GUI.Core;


namespace Test {
	
	class Program {
		static void Main(string[] args) {




			//Random seeder = new Random();
			//int replications = 1000000;
			//DiceGame diceGame = new DiceGame(seeder);
			//RandomGameMC randomGame = new RandomGameMC(diceGame);
			//randomGame.Simulate(replications);
			//Console.WriteLine(randomGame.TextResult());
			bool checkArray = false;
			Random seeder = new Random();
			int replications = 100;
			DiceGame diceGame = new DiceGame(seeder);
			TableCreatorMC gameWithTable = new TableCreatorMC(diceGame);

			if (checkArray) {
				gameWithTable.CreateTableArray(replications);
				for (int i = 0; i < 216; i++) {
					for (int j = 0; j < 216; j++) {
						double item = gameWithTable.WinChancesArr[i, j];
						//Console.Write($"{gameWithTable.WinChancesArr[i, j]} ");
					}
					Console.WriteLine();
				}
			}
			else {
				gameWithTable.CreateTableDict(replications);
				for (int i = 0; i < 216; i++) {
					for (int j = 0; j < 216; j++) {
						double item = gameWithTable.WinChancesDict[111111];
					}
				}
				
			}

			Console.WriteLine();
			Console.Read();
		}

	}
}
