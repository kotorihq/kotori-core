using System.Collections.Generic;
using System.Linq;
using KotoriCore.Helpers;
using Sushi2;

namespace KotoriCore.Exceptions
{
    /// <summary>
    /// Kotori validation exception.
    /// </summary>
    public class KotoriValidationException : KotoriException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriValidationException"/> class.
        /// </summary>
        /// <param name="validationResults">Validation results.</param>
        public KotoriValidationException(IEnumerable<ValidationResult> validationResults) : base(validationResults?.Where(vr => vr != null && !vr.IsValid).Select(vr2 => vr2.Message).ToImplodedString(" ") ?? "Unknown error.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:KotoriCore.Exceptions.KotoriValidationException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public KotoriValidationException(string message) : base(message)
        {
        }
    }
}
