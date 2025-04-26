using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public abstract string GetRole();
    }
}
