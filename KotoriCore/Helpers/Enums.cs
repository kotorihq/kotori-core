﻿namespace KotoriCore.Helpers
{
    /// <summary>
    /// Enums.
    /// </summary>
    public static class Enums
    {
		/// <summary>
		/// Claim types.
		/// </summary>
		public enum ClaimType
		{
			/// <summary>
			/// The master.
			/// </summary>
			Master = 0,
			/// <summary>
			/// The project.
			/// </summary>
			Project = 1
		}

        /// <summary>
        /// Priorities.
        /// </summary>
        public enum Priority
        {
            /// <summary>
            /// Max priority. Do it now.
            /// </summary>
            DoItNow = 0,
            /// <summary>
            /// Normal priority. Do it later.
            /// </summary>
            DoItLater = 1
        }
    }
}
