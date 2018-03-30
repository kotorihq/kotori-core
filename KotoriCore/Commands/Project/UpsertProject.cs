using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert project command.
    /// </summary>
    public class UpsertProject : Command, IUpsertProject
    {
        IRandomGenerator _randomGenerator;

        /// <summary>
        /// The instance.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The create only flag.
        /// </summary>
        public bool CreateOnly { get; set; }

        // TODO
        public UpsertProject(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator;
        }
        
        // TODO
        public void Init(bool createOnly, string instance, string projectId, string name)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;
            Name = name;

            if (CreateOnly &&
                string.IsNullOrEmpty(projectId))
            {
                ProjectId = _randomGenerator.GetId();
            }
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validation results.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");
        }
    }
}
