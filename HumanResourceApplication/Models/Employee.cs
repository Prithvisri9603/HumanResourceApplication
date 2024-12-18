using System;
using System.Collections.Generic;

namespace HumanResourceApplication.Models;

public partial class Employee
{
    public decimal EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateOnly HireDate { get; set; }

    public string JobId { get; set; } = null!;

    public decimal? Salary { get; set; }

    public decimal? CommissionPct { get; set; }

    public decimal? ManagerId { get; set; }

    public decimal? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual Job Job { get; set; } = null!;

    public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

    public virtual Employee? Manager { get; set; }
}
