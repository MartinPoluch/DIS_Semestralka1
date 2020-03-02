using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI;
using GUI.Core;
using SimulationCore.Generators;

namespace SimulationCore {
	public class RandomGameMC : SimCore {


		public RandomGameMC(DiceGame diceGame) {
			DiceGame = diceGame;
			Worker = null;
		}

		public ChartSettings ChartSettings { get; set; }

		public BackgroundWorker Worker { get; set; }

		public DiceGame DiceGame { get; set; }


		protected override void DoReplication() {
			DiceGame.GenerateFirstPlayer();
			DiceGame.GenerateSecondPlayer();
			DiceGame.FindWinner();

			if ((Worker != null) && (ActualReplication > ChartSettings.SkipReplications) && (ActualReplication % ChartSettings.Step == 0)) {
				Worker.ReportProgress(ActualReplication / NumberOfReplications, ActualReplication);
			}
		}

		public string TextResult() {
			if (ActualReplication == 0) { // osetrenie delenia nulou
				return "Actual replication is 0";
			}
			string feroOutput = $"Fero wins [%]: {((double) DiceGame.FirstPlayerWins / NumberOfReplications) * 100}";
			string jozoOutput = $"Jozo wins [%]: {((double) DiceGame.SecondPlayerWins / NumberOfReplications) * 100}";
			return $"{feroOutput}\n{jozoOutput}";
		}

	}
}
