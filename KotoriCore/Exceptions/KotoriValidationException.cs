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
        public KotoriValidationException(IEnumerable<ValidationResult> validationResults) : base(validationResults?.Where(vr => !vr.IsValid).ToImplodedString(" "))
        {
        }
    }
}
