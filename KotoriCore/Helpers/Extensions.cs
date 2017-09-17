using System.Collections.Generic;
using System.Linq;
using Sushi2;

namespace KotoriCore.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Returns summarized validation result.
        /// </summary>
        /// <returns>The summarized validation result.</returns>
        /// <param name="validationResults">Validation results.</param>
        public static ValidationResult ToValidationResult(this IEnumerable<ValidationResult> validationResults)
        {
            return new ValidationResult(validationResults.Select(x => x.Message).ToImplodedString(" "));
        }
    }
}
