using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WithSource.Identity.Models
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NormalizedRoleName { get; set; }

        public string RoleId => this.Id.ToString();
    }
}
