using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore;

namespace GUI.Core {
	public class TableCreatorMC : SimCore {

		private static readonly int[] _combinations = {
			111, 112, 113, 114, 115, 116, 121, 122, 123, 124, 125, 126, 131, 132, 133, 134, 135, 136, 141, 142, 143,
			144, 145, 146, 151, 152, 153, 154, 155, 156, 161, 162, 163, 164, 165, 166, 211, 212, 213, 214, 215, 216,
			221, 222, 223, 224, 225, 226, 231, 232, 233, 234, 235, 236, 241, 242, 243, 244, 245, 246, 251, 252, 253,
			254, 255, 256, 261, 262, 263, 264, 265, 266, 311, 312, 313, 314, 315, 316, 321, 322, 323, 324, 325, 326,
			331, 332, 333, 334, 335, 336, 341, 342, 343, 344, 345, 346, 351, 352, 353, 354, 355, 356, 361, 362, 363,
			364, 365, 366, 411, 412, 413, 414, 415, 416, 421, 422, 423, 424, 425, 426, 431, 432, 433, 434, 435, 436,
			441, 442, 443, 444, 445, 446, 451, 452, 453, 454, 455, 456, 461, 462, 463, 464, 465, 466, 511, 512, 513,
			514, 515, 516, 521, 522, 523, 524, 525, 526, 531, 532, 533, 534, 535, 536, 541, 542, 543, 544, 545, 546,
			551, 552, 553, 554, 555, 556, 561, 562, 563, 564, 565, 566, 611, 612, 613, 614, 615, 616, 621, 622, 623,
			624, 625, 626, 631, 632, 633, 634, 635, 636, 641, 642, 643, 644, 645, 646, 651, 652, 653, 654, 655, 656,
			661, 662, 663, 664, 665, 666,
		};

		public TableCreatorMC(DiceGame diceGame) {
			DiceGame = diceGame;
			Worker = null;
			WinChances = new Dictionary<int, double>();
			BestResponses = new Dictionary<int, int>();
		}

		public DiceGame DiceGame { get; set; }

		public Dictionary<int, double> WinChances { get; set; }

		/// <summary>
		/// Kluc je ciselna kombinacia prveho hraca. Hodnota je kombinacia ktoru by mal zvolit druhy hrac aby mal najvacsiu sancu vyhrat.
		/// </summary>
		public Dictionary<int, int> BestResponses { get; set; }

		public BackgroundWorker Worker { get; set; }

		public void CreateTable(int replications) {
			WinChances.Clear();
			BestResponses.Clear();
			foreach (int firstPlayer in _combinations) {
				Worker?.ReportProgress(1, firstPlayer);
				double bestChance = 0;// doposial najlepsia najdena sanca na vyhru druheho hraca
				int bestResponse = 0; // doposial najlepsia najdena odpoved na kombinaciu prveho hraca
				foreach (int secondPlayer in _combinations) {
					int combination = (firstPlayer * 1000) + secondPlayer;
					if (firstPlayer == secondPlayer) {
						WinChances.Add(combination, 0); //remiza
					}
					else {
						DiceGame.Reset();
						DiceGame.FirstPlayerRolls = firstPlayer;
						DiceGame.SecondPlayerRolls = secondPlayer;
						Simulate(replications);
						double secondPlayerChance = ((double)DiceGame.SecondPlayerWins / replications) * 100;
						WinChances.Add(combination, secondPlayerChance);
						//Console.WriteLine($"secondPlayer: {secondPlayer} bestPlayer: {bestResponse} second chance: {secondPlayerChance} best chance: {bestChance}");
						if (secondPlayerChance >= bestChance) {
							bestChance = secondPlayerChance;
							bestResponse = secondPlayer;
						}
					}
					
				}
				BestResponses.Add(firstPlayer, bestResponse);
			}
		}

		protected override void InitSimulation() {

		}

		protected override void DoReplication() {
			DiceGame.FindWinner();
		}

		public void WriteWinChancesToFile(string fileName) {
			string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName))) {
				outputFile.Write(" ;");
				foreach (int combination in _combinations) {
					outputFile.Write($"{combination};"); // hlavicka (kombinacie)
				}
				outputFile.WriteLine();
				outputFile.Write($"{_combinations[0]};"); // prva kombinacia prveho riadku
				int row = 0;
				int column = 0;
				foreach (KeyValuePair<int, double> keyValuePair in WinChances) {
					if (column == 216) {
						column = 0;
						outputFile.WriteLine();
						row++;
						outputFile.Write($"{_combinations[row]};"); // prva hodnota v riadku je kombinacia
					}	
					column++;
					outputFile.Write($"{keyValuePair.Value};");
				}
			}
		}

		public void WriteBestResponsesToFile(string fileName) {
			string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName))) {
				foreach (KeyValuePair<int, int> bestResponse in BestResponses) {
					outputFile.WriteLine($"{bestResponse.Key} {bestResponse.Value}");
				}
			}
		}

	}
}
