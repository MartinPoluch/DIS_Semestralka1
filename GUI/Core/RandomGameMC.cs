﻿using System;
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

		public RandomGameMC(DiceGame diceGame, GameMode gameMode) {
			DiceGame = diceGame;
			Worker = null;
			GameMode = gameMode;
			GameTable = null;
		}

		public GameMode GameMode { get; set; }

		public TableCreatorMC GameTable { get; set; }

		public ChartSettings ChartSettings { get; set; }

		public BackgroundWorker Worker { get; set; }

		public DiceGame DiceGame { get; set; }

		protected override void InitSimulation() {
			DiceGame.Reset();
		}

		protected override void DoReplication() {
			DiceGame.GenerateFirstPlayer();

			switch (GameMode) {
				case GameMode.AllRandom: {
					DiceGame.GenerateSecondPlayer();
					break;
				}
				case GameMode.UnlimitedTable: {
					DiceGame.SecondPlayerRolls = GameTable.BestResponses[DiceGame.FirstPlayerRolls];// vyberiem podla tabulky tu najlepsiu odpoved
					break;
				}
				case GameMode.LimitedTable: {
					DiceGame.GenerateTwoRollsForSecondPlayer();
					DiceGame.SecondPlayerRolls = GameTable.GetBestResponseFromLimitedTable(DiceGame.FirstPlayerRolls, DiceGame.SecondPlayerRolls);
					break;
				}
				case GameMode.OwnStrategy: {

					break;
				}
			}
			
			DiceGame.FindWinner();

			if ((Worker != null) && (ActualReplication > ChartSettings.SkipReplications) && (ActualReplication % ChartSettings.Step == 0)) {
				Worker.ReportProgress(ActualReplication / NumberOfReplications, ActualReplication); //TODO poslat vsetky parametre
			}
		}

		private void BestResponseAlgorithm() {
			int firstPlayer = DiceGame.FirstPlayerRolls;
		}

		public string TextResult() {
			string feroOutput = $"Fero wins [%]: {((double)DiceGame.FirstPlayerWins / (ActualReplication + 1)) * 100}"; // +1 pretoze sa este neskoncila aktualna replikacia
			string jozoOutput = $"Jozo wins [%]: {((double)DiceGame.SecondPlayerWins / (ActualReplication + 1)) * 100}";
			return $"{feroOutput}\n{jozoOutput}";
		}

	}
}
