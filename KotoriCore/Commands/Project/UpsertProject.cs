using System.Collections.Generic;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert project command.
    /// </summary>
    public class UpsertProject : ICommand, IUpsertProject
    {
        IRandomGenerator _randomGenerator;

        public string Instance { get; internal set; }

        public string ProjectId { get; internal set; }

        public string Name { get; internal set; }

        public bool CreateOnly { get; internal set; }

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
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");
        }
    }
}
