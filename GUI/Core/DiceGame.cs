using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore.Generators;

namespace GUI.Core {
	public class DiceGame {

		private readonly UniformRNG _firstPlayerGen; // Ferov generator
		private readonly UniformRNG _secondPlayerGen; // Jozov generator
		private readonly UniformRNG _sequenceGen; // generator pre sekvenciu

		private int _sequence;

		public DiceGame(Random seeder) {
			_firstPlayerGen = new UniformRNG(seeder.Next(), 1, 6);
			_secondPlayerGen = new UniformRNG(seeder.Next(), 1, 6);
			_sequenceGen = new UniformRNG(seeder.Next(), 1, 6);
			Reset();
		}

		public int FirstPlayerRolls { get; set; }

		public int SecondPlayerRolls { get; set; }

		public int FirstPlayerWins { get; private set; }

		public int SecondPlayerWins { get; private set; }

		public void Reset() {
			_sequence = 0;
			FirstPlayerRolls = 0;
			SecondPlayerRolls = 0;
			FirstPlayerWins = 0;
			SecondPlayerWins = 0;
		}

		private int DoThreeDiceRolls(UniformRNG generator) {
			int firstRoll = generator.NextInt();
			int secondRoll = generator.NextInt();
			int thirdRoll = generator.NextInt();
			return (firstRoll * 100) + (secondRoll * 10) + thirdRoll;
		}

		private void IncreaseSequence() {
			_sequence %= 100;
			_sequence *= 10;
			_sequence += _sequenceGen.NextInt();
		}

		public void GenerateFirstPlayer() {
			FirstPlayerRolls = DoThreeDiceRolls(_firstPlayerGen);
		}

		public void GenerateSecondPlayer() {
			SecondPlayerRolls = DoThreeDiceRolls(_secondPlayerGen);
		}

		public void FindWinner() {
			if (FirstPlayerRolls == SecondPlayerRolls) {
				return; // remiza
			}

			_sequence = DoThreeDiceRolls(_sequenceGen); // inicializujem sekvenciu
			while (true) { // pokial nikdo nevyhral
				if (FirstPlayerRolls == _sequence) { // ci vyhral prvy hrac
					FirstPlayerWins++;
					return;
				}

				if (SecondPlayerRolls == _sequence) { // ci vyhral druhy hrac
					SecondPlayerWins++;
					return;
				}
				IncreaseSequence(); // zmen sequenciu
			}
		}
	}
}
