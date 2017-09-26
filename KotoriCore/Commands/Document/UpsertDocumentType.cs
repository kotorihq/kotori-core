using System;
using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class UpsertDocumentType : Command, IInstance, IProject
    {
        public string Instance { get; }
        public string ProjectId { get; }

        public override IEnumerable<ValidationResult> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
