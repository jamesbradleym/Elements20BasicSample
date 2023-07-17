using Elements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Elements20BasicSample
{
	/// <summary>
	/// Override metadata for LinesOverrideRemoval
	/// </summary>
	public partial class LinesOverrideRemoval : IOverride
	{
        public static string Name = "Lines Removal";
        public static string Dependency = null;
        public static string Context = "[*discriminator=Elements.Linework]";
		public static string Paradigm = "Edit";

        /// <summary>
        /// Get the override name for this override.
        /// </summary>
        public string GetName() {
			return Name;
		}

		public object GetIdentity() {

			return Identity;
		}

	}

}