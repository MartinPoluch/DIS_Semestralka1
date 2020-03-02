using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore;

namespace GUI.Core {
	public class TableCreatorMC : SimCore {

		public TableCreatorMC(DiceGame diceGame) {
			DiceGame = diceGame;
			WinChancesDict = new Dictionary<int, double>();
		}

		public DiceGame DiceGame { get; set; }


		public Dictionary<int, double> WinChancesDict { get; set; }

		public void CreateTable(int replications) {
			for (int roll1Player1 = 1; roll1Player1 <= 6; roll1Player1++) {
				for (int roll2Player1 = 1; roll2Player1 <= 6; roll2Player1++) {
					for (int roll3Player1 = 1; roll3Player1 <= 6; roll3Player1++) {
						int firstPlayer = (roll1Player1 * 100) + (roll2Player1 * 10) + roll3Player1;
						for (int roll1Player2 = 1; roll1Player2 <= 6; roll1Player2++) {
							for (int roll2Player2 = 1; roll2Player2 <= 6; roll2Player2++) {
								for (int roll3Player2 = 1; roll3Player2 <= 6; roll3Player2++) {

									int secondPlayer = (roll1Player2 * 100) + (roll2Player2 * 10) + roll3Player2;
									int combination = (firstPlayer * 1000) + secondPlayer;
									if (firstPlayer != secondPlayer) {
										DiceGame.Reset();
										DiceGame.FirstPlayerRolls = firstPlayer;
										DiceGame.SecondPlayerRolls = secondPlayer;
										Simulate(replications);
										double secondPlayerChance = ((double)DiceGame.SecondPlayerWins / replications) * 100;
										WinChancesDict.Add(combination, secondPlayerChance);
									}
									else {
										WinChancesDict.Add(combination, 0);
									}
								}
							}
						}
					}
				}
			}
		}

		protected override void InitSimulation() {

		}

		protected override void DoReplication() {
			DiceGame.FindWinner();
		}

	}
}
