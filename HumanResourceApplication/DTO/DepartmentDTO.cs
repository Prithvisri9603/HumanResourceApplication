namespace HumanResourceApplication.DTO
{
    public class DepartmentDTO
    {
        public decimal DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public decimal? ManagerId { get; set; }

        public decimal? LocationId { get; set; }

    }
}
