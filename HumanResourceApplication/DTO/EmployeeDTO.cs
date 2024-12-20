namespace HumanResourceApplication.DTO
{
    public class EmployeeDTO
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

    }
}
