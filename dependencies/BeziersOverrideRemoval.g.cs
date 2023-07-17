using Elements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Elements20BasicSample
{
	/// <summary>
	/// Override metadata for BeziersOverrideRemoval
	/// </summary>
	public partial class BeziersOverrideRemoval : IOverride
	{
        public static string Name = "Beziers Removal";
        public static string Dependency = null;
        public static string Context = "[*discriminator=Elements.Bezierwork]";
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