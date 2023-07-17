using Elements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Elements20BasicSample
{
	/// <summary>
	/// Override metadata for PolylinesOverrideAddition
	/// </summary>
	public partial class PolylinesOverrideAddition : IOverride
	{
        public static string Name = "Polylines Addition";
        public static string Dependency = null;
        public static string Context = "[*discriminator=Elements.Polylinework]";
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