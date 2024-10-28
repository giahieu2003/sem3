using System;
using System.Collections.Generic;

namespace Sem3.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
    }
}
