using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Chart {
	/// <summary>
	/// Jeden bod v grafe.
	/// </summary>
	public class MeasureModel {
		public int Replications { get; set; }
		public double WinPercentage { get; set; }
	}
}
