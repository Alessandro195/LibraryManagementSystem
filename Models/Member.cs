using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Member : Person
    {
        public DateTime JoinDate { get; set; }

        public override string GetRole() => "Member";
    }
}
