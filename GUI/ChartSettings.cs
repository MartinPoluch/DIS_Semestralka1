using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI {
	public class ChartSettings {
		public ChartSettings(int step, int skipReplications) {
			Step = step;
			SkipReplications = skipReplications;
		}

		public int Step { get; set; }

		public int SkipReplications { get; set; }

		
	}
}
