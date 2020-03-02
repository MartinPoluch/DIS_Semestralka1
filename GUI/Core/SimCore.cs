using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore {
	public abstract class SimCore {
		protected SimCore() {
			Stop = false;
			ActualReplication = 0;
		}

		public bool Stop { get; set; }
		public int NumberOfReplications { get; set; }
		public int ActualReplication { get; set; }

		protected abstract void DoReplication();

		protected abstract void BeforeSimulation();

		public void Simulate(int replications) {
			BeforeSimulation();
			NumberOfReplications = replications;
			for (int replication = 0; replication < replications; replication++) {
				ActualReplication = replication;
				if (Stop) {
					break;
				}

				DoReplication();
			}
		}
	}
}
