using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore;

namespace GUI.Core {
	public class GameWithTableMC : SimCore {


		public GameWithTableMC(DiceGame diceGame) {
			DiceGame = diceGame;
		}

		public DiceGame DiceGame { get; set; }

		public void CreateTable() {
			
		}

		protected override void BeforeSimulation() {
			
		}

		protected override void DoReplication() {
			
		}

	}
}
