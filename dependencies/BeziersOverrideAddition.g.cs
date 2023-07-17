using Elements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Elements20BasicSample
{
	/// <summary>
	/// Override metadata for BeziersOverrideAddition
	/// </summary>
	public partial class BeziersOverrideAddition : IOverride
	{
        public static string Name = "Beziers Addition";
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