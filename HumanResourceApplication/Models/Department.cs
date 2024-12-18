using System;
using System.Collections.Generic;

namespace HumanResourceApplication.Models;

public partial class Department
{
    public decimal DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public decimal? ManagerId { get; set; }

    public decimal? LocationId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

    public virtual Location? Location { get; set; }

    public virtual Employee? Manager { get; set; }
}
