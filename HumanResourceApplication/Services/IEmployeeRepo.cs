using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface IEmployeeRepo
    {
        Task AddEmployee(EmployeeDTO employee);
        Task ModifyEmployee(int  employeeId,EmployeeDTO employee);
        Task AssignJob(string JobId, EmployeeDTO employee);
        Task  AssignMan( decimal ManagerId, EmployeeDTO employee);
        Task AssignDep(decimal DepartmentId, EmployeeDTO employee);
        Task UpdateCommissionForDepartment(decimal departmentId, decimal commissionPercentage);
        Task<EmployeeDTO> FindByFirstName(string firstName);
        Task<EmployeeDTO> FindByEmail(string email);
        Task<EmployeeDTO> FindByPhoneNumber(string phone);
        Task<List<EmployeeDTO>> FindAllEmployeeWithNoCommission();
        Task<decimal> FindTotalCommissionIssuedToDepartment(decimal departmentId);
        Task<List<EmployeeDTO>> ListAllEmployeesByDepartment(decimal departmentId);
        Task<List<EmployeeDTO>> ListAllManagerDetails();
        Task<Dictionary<decimal, int>> CountAllEmployeesGroupByLocation();
        Task<(string JobDescription, decimal MaxSalary)> FindMaxSalaryOfJobByEmployeeId(decimal employeeId);
        Task UpdateEmployeeEmail(string email, EmployeeDTO employee);
    }
}
