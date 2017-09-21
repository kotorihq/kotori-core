using System.Collections.Generic;
using KotoriCore.Helpers;

namespace KotoriCore.Commands
{
    public class GetProjects : Command, IInstance
    {        
        public string Instance { get; }
        
        public GetProjects(string instance) : base(Enums.Priority.DoItNow)
        {
            Instance = instance;            
        }
        
        public override IEnumerable<ValidationResult> Validate()
        {
            return null;
        }
    }
}
