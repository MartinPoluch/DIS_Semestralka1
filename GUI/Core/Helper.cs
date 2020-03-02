using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Core {
	public class Helper {
		private Helper()
		{
		}

		public static bool  EqualRolls(int[] rolls1, int[] rolls2) {
			return (rolls1[0] == rolls2[0]) && (rolls1[1] == rolls2[1]) && (rolls1[2] == rolls2[2]);
		}
	}
}
