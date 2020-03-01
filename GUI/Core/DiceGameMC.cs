using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI;
using SimulationCore.Generators;

namespace SimulationCore {
	public class DiceGameMC : SimCore {

		private readonly UniformRNG _firstPlayerGen; // Ferov generator
		private readonly UniformRNG _secondPlayerGen; // Jozov generator
		private readonly UniformRNG _sequenceGen; // generator pre sekvenciu

		private int[] _sequence;

		public DiceGameMC(Random seeder) {
			_firstPlayerGen = new UniformRNG(seeder.Next(), 1, 6);
			_secondPlayerGen = new UniformRNG(seeder.Next(), 1, 6);
			_sequenceGen = new UniformRNG(seeder.Next(), 1, 6);
			_sequence = new int[3];
			FirstPlayerRolls = new int[3];
			SecondPlayerRolls = new int[3];
			Worker = null;
		}

		public int[] FirstPlayerRolls { get; set; }

		public int[] SecondPlayerRolls { get; set; }

		public int FirstPlayerWins { get; private set; }

		public int SecondPlayerWins { get; private set; }

		public BackgroundWorker Worker { get; set; }

		private void DoThreeDiceRolls(int[] playerRolls, UniformRNG generator) {
			for (int roll = 0; roll < 3; roll++) {
				playerRolls[roll] = generator.NextInt();
			}
		}

		/// <summary>
		/// Posun cisiel v sekvencii. Prve cislo sa zahodi. Zvysne dve sa posunu "dolava". A na poslednu poziciu sa vygeneruje nove cislo.
		/// </summary>
		/// <param name="sequence"></param>
		private void IncreaseSequence() {
			_sequence[0] = _sequence[1];
			_sequence[1] = _sequence[2];
			_sequence[2] = _sequenceGen.NextInt();
		}

		protected override void DoReplication() {
			DoThreeDiceRolls(FirstPlayerRolls, _firstPlayerGen);
			DoThreeDiceRolls(SecondPlayerRolls, _secondPlayerGen);
			FindWinner();
		}

		private void FindWinner() {
			if (EqualRolls(FirstPlayerRolls, SecondPlayerRolls)) {
				return; // remiza
			}

			DoThreeDiceRolls(_sequence, _sequenceGen); // inicializujem sekvenciu
			bool winnerNotFound = true; // riesi problem aby sa do grafu vypisala aj posledna hodnota
			while (winnerNotFound) { // pokial nikdo nevyhral
				if (EqualRolls(FirstPlayerRolls, _sequence)) { // ci vyhral prvy hrac
					FirstPlayerWins++;
					winnerNotFound = false;
				}

				if (winnerNotFound && EqualRolls(SecondPlayerRolls, _sequence)) { // ci vyhral druhy hrac
					SecondPlayerWins++;
					winnerNotFound = false;
				}

				if (Worker != null) {
					Worker.ReportProgress(ActualReplication / NumberOfReplications, ActualReplication);
				}

				if (winnerNotFound) {
					IncreaseSequence(); // zmen sequenciu
				}
			}
		}

		private bool EqualRolls(int[] player, int[] sequence) {
			return (player[0] == sequence[0]) && (player[1] == sequence[1]) && (player[2] == sequence[2]);
		}

		public string TextResult() {
			if (ActualReplication == 0) { // osetrenie delenia nulou
				return "Actual replication is 0";
			}
			string feroOutput = $"Fero wins [%]: {((double) FirstPlayerWins/ActualReplication) * 100}";
			string jozoOutput = $"Jozo wins [%]: {((double) SecondPlayerWins / ActualReplication) * 100}";
			return $"{feroOutput}\n{jozoOutput}";
		}

	}
}
