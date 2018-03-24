using System.Collections.Generic;
using KotoriCore.Configurations;
using KotoriCore.Helpers;
using KotoriCore.Helpers.RandomGenerator;

namespace KotoriCore.Commands
{
    /// <summary>
    /// Upsert project key command.
    /// </summary>
    public class UpsertProjectKey : Command, IUpsertProjectKey
    {
        public string Instance { get; private set; }

        /// <summary>
        /// The project identifier.
        /// </summary>
        public string ProjectId { get; private set; }

        /// <summary>
        /// The project key.
        /// </summary>
        public ProjectKey ProjectKey;

        /// <summary>
        /// The create only flag.
        /// </summary>
        public bool CreateOnly { get; private set; }

        IRandomGenerator _randomGenerator;

        // TODO
        public UpsertProjectKey(IRandomGenerator randomGenerator)
        {
            _randomGenerator = randomGenerator;
        }

        // TODO
        public void Init(bool createOnly, string instance, string projectId, string projectKey, bool isReadonly)
        {
            CreateOnly = createOnly;
            Instance = instance;
            ProjectId = projectId;

            if (!string.IsNullOrEmpty(projectKey))
                ProjectKey = new ProjectKey(projectKey, isReadonly);

            if (CreateOnly &&
                ProjectKey == null)
            {
                var id = _randomGenerator.GetId();
                ProjectKey = new ProjectKey(id);
            }

            if (CreateOnly &&
                string.IsNullOrEmpty(ProjectKey.Key))
            {
                var id = _randomGenerator.GetId();
                ProjectKey.Key = id;
            }

            if (ProjectKey != null)
                ProjectKey.IsReadonly = isReadonly;
        }

        /// <summary>
        /// Validates the command.
        /// </summary>
        /// <returns>The validate result.</returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(Instance))
                yield return new ValidationResult("Instance must be set.");

            if (string.IsNullOrEmpty(ProjectId))
                yield return new ValidationResult("Project Id must be set.");

            if (ProjectKey == null ||
                string.IsNullOrEmpty(ProjectKey.Key))
            {
                yield return new ValidationResult("Project key must be set.");
            }

            if (!ProjectKey.Key.IsValidSlug())
            {
                yield return new ValidationResult("Project key is not a valid slug.");
            }
        }
    }
}
