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




			Random seeder = new Random();
			int replications = 1000;
			DiceGame diceGame = new DiceGame(seeder);
			//TableCreatorMC table = new TableCreatorMC(diceGame);
			//table.CreateTable(100);
			RandomGameMC randomGame = new RandomGameMC(diceGame, GameMode.AllRandom);
			//randomGame.GameTable = table;
			randomGame.Simulate(replications);
			Console.WriteLine(randomGame.TextResult());

			//bool checkArray = false;
			//Random seeder = new Random();
			//int replications = 100;
			//DiceGame diceGame = new DiceGame(seeder);
			

			//if (checkArray) {
			//	gameWithTable.CreateTableArray(replications);
			//	for (int i = 0; i < 216; i++) {
			//		for (int j = 0; j < 216; j++) {
			//			double item = gameWithTable.WinChancesArr[i, j];
			//			//Console.Write($"{gameWithTable.WinChancesArr[i, j]} ");
			//		}
			//		Console.WriteLine();
			//	}
			//}
			//else {
			//	gameWithTable.CreateTableDict(replications);
			//	for (int i = 0; i < 216; i++) {
			//		for (int j = 0; j < 216; j++) {
			//			double item = gameWithTable.WinChances[111111];
			//		}
			//	}

			//}

			//Console.WriteLine();
			//Console.Read();



			//for (int i = 1; i <= 6; i++) {
			//	for (int j = 1; j <= 6; j++) {
			//		for (int k = 1; k <= 6; k++) {
			//			int number = i * 100 + j * 10 + k;
			//			Console.Write($"{number}, ");
			//		}
			//		Console.WriteLine();
			//	}

			//}

			Console.Read();
		}

	}
}
