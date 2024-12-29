using System.Dynamic;
using FluentValidation;
using FluentValidation.Results;
using HumanResourceApplication.Controllers;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace Testing
{
       public class EmployeeTest
       {
            private readonly Mock<IEmployeeRepo> _employeeRepoMock;
            private readonly Mock<IValidator<EmployeeDTO>> _validatorMock;
            private readonly EmployeesController _controller;

            public EmployeeTest()
            {
                _employeeRepoMock = new Mock<IEmployeeRepo>();
                _validatorMock = new Mock<IValidator<EmployeeDTO>>();
                _controller = new EmployeesController(_employeeRepoMock.Object, _validatorMock.Object);
            }

        [Fact]
        public async Task AddEmployee_Success()
        {
           
            var employeeDto = new EmployeeDTO
            {
                EmployeeId = 103,
                FirstName = "Alexander",
                LastName = "James",
                Email = "AJAMES",
                PhoneNumber = "1.590.555.0103",
                HireDate = DateOnly.FromDateTime(DateTime.Now),
                JobId = "IT_PROG",
                Salary = 9000,
                DepartmentId = 60
            };
            _validatorMock
                .Setup(v => v.ValidateAsync(employeeDto, default))
                .ReturnsAsync(new ValidationResult());
            _employeeRepoMock
                .Setup(r => r.AddEmployee(It.IsAny<EmployeeDTO>()))
                .Returns(Task.CompletedTask);

        
            var result = await _controller.AddEmployee(employeeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Record Created Successfully", okResult.Value);
        }

        [Fact]
        public async Task AddEmployee_ValidationError()
        {
            
            var employeeDto = new EmployeeDTO { Email = "Invalid Email" };
            var validationFailure = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Invalid email format")
            };
            _validatorMock
                .Setup(v => v.ValidateAsync(employeeDto, default))
                .ReturnsAsync(new ValidationResult(validationFailure));

           
            var result = await _controller.AddEmployee(employeeDto);

           
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Validation failed", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task FindByEmail_Success()
        {
          
            var employeeDto = new EmployeeDTO
            {
                EmployeeId = 103,
                FirstName = "Alexander",
                Email = "AJAMES"
            };
            _employeeRepoMock
                .Setup(r => r.FindByEmail("AJAMES"))
                .ReturnsAsync(employeeDto);

           
            var result = await _controller.FindByEmail("AJAMES");

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEmployee = Assert.IsType<EmployeeDTO>(okResult.Value);
            Assert.Equal("Alexander", returnedEmployee.FirstName);
        }

        [Fact]
        public async Task AssignDep_EmployeeNotFound()
        {
            
            _employeeRepoMock
                .Setup(r => r.AssignDep(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new Exception("Employee with ID 103 not found."));

            
            var result = await _controller.AssignDep(103, 60);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Employee with ID 103 not found", badRequestResult.Value.ToString());
        }
        [Fact]
        public async Task FindByFirstName_ValidName_ReturnsOk()
        {
            
            var firstName = "Alexander";
            var employee = new EmployeeDTO
            {
                EmployeeId = 103,
                FirstName = "Alexander",
                LastName = "James",
                Email = "AJAMES",
                PhoneNumber = "1.590.555.0103",
                HireDate = new DateOnly(2016, 1, 3),
                JobId = "IT_PROG",
                Salary = 9000,
                CommissionPct = null,
                ManagerId = 102,
                DepartmentId = 60
            };

            _employeeRepoMock.Setup(repo => repo.FindByFirstName(firstName))
                .ReturnsAsync(employee);

           
            var result = await _controller.FindByFirstName(firstName);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<EmployeeDTO>(okResult.Value);
            Assert.Equal(firstName, returnedValue.FirstName);
        }

        [Fact]
        public async Task FindByFirstName_InvalidName_ValidationFailure_ReturnsBadRequest()
        {
            var invalidName = "NonExistent";
            var expectedMessage = $"No employee found with the first name: {invalidName}";
            _employeeRepoMock
                .Setup(repo => repo.FindByFirstName(invalidName))
                .ThrowsAsync(new Exception(expectedMessage));
            var result = await _controller.FindByFirstName(invalidName);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequestResult.Value;

            var valueType = value.GetType();
            var timeStampProperty = valueType.GetProperty("timeStamp");
            var messageProperty = valueType.GetProperty("message");

            Assert.NotNull(timeStampProperty);
            Assert.NotNull(messageProperty);

            var timeStampValue = timeStampProperty.GetValue(value);
            var messageValue = messageProperty.GetValue(value);

            Assert.IsType<DateOnly>(timeStampValue);
            Assert.Equal(expectedMessage, messageValue);
        }


        [Fact]
        public async Task FindByEmail_ValidEmail_ReturnsOk()
        {
            
            var email = "AJAMES";
            var employee = new EmployeeDTO
            {
                EmployeeId = 103,
                FirstName = "Alexander",
                LastName = "James",
                Email = email,
                PhoneNumber = "1.590.555.0103",
                HireDate = new DateOnly(2016, 1, 3),
                JobId = "IT_PROG",
                Salary = 9000,
                CommissionPct = null,
                ManagerId = 102,
                DepartmentId = 60
            };

            _employeeRepoMock.Setup(repo => repo.FindByEmail(email))
                .ReturnsAsync(employee);

            
            var result = await _controller.FindByEmail(email);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<EmployeeDTO>(okResult.Value);
            Assert.Equal(email, returnedValue.Email);
        }

        [Fact]
        public async Task FindByEmail_InvalidEmail_ReturnsBadRequest()
        {
            var invalidEmail = "INVALID_EMAIL";

            _employeeRepoMock.Setup(repo => repo.FindByEmail(invalidEmail))
                .ThrowsAsync(new Exception($"No employee found with the email: {invalidEmail}"));

            
            var result = await _controller.FindByEmail(invalidEmail);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);

            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(returnValue));

            Assert.Equal($"No employee found with the email: {invalidEmail}", response["message"]);
            Assert.NotNull(response["timeStamp"]);
        }



        [Fact]
        public async Task FindByPhoneNumber_ValidPhone_ReturnsOk()
        {
            
            var phone = "1.590.555.0103";
            var employee = new EmployeeDTO
            {
                EmployeeId = 103,
                FirstName = "Alexander",
                LastName = "James",
                Email = "AJAMES",
                PhoneNumber = phone,
                HireDate = new DateOnly(2016, 1, 3),
                JobId = "IT_PROG",
                Salary = 9000,
                CommissionPct = null,
                ManagerId = 102,
                DepartmentId = 60
            };

            _employeeRepoMock.Setup(repo => repo.FindByPhoneNumber(phone))
                .ReturnsAsync(employee);

            var result = await _controller.FindByPhoneNumber(phone);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<EmployeeDTO>(okResult.Value);
            Assert.Equal(phone, returnedValue.PhoneNumber);
        }


        [Fact]
        public async Task FindByPhoneNumber_InvalidPhone_ReturnsBadRequest()
        {
            var invalidPhone = "1234567890";

            _employeeRepoMock.Setup(repo => repo.FindByPhoneNumber(invalidPhone))
                .ThrowsAsync(new Exception($"No employee found with the phone number: {invalidPhone}"));

            var result = await _controller.FindByPhoneNumber(invalidPhone);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);

            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(returnValue));

            Assert.Equal($"No employee found with the phone number: {invalidPhone}", response["message"]);
            Assert.NotNull(response["timeStamp"]);
        }



        [Fact]
        public async Task ModifyEmployee_ReturnsOk()
        {
            var employeeid = 112;
            var employee = new EmployeeDTO
            {
                EmployeeId = 112,
                FirstName = "Alex",
                LastName = "James",
                Email = "AJAMES",
                PhoneNumber = "1.590.555.0103",
                HireDate = new DateOnly(2016, 1, 3),
                JobId = "IT_PROG",
                Salary = 9000,
                CommissionPct = null,
                ManagerId = 102,
                DepartmentId = 60
            };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<EmployeeDTO>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _employeeRepoMock.Setup(r => r.ModifyEmployee(It.IsAny<int>(), It.IsAny<EmployeeDTO>()))
                             .Returns(Task.CompletedTask);

            
            var result = await _controller.ModifyEmployee(employeeid, employee);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Record Modified Successfully", okResult.Value);
        }
        [Fact]
        public async Task ModifyEmployee_ValidationFails_ReturnsBadRequest()
        {
            var employeeid = 112;
            var employee = new EmployeeDTO
            {
                EmployeeId = 112,
                FirstName = "",
                LastName = "James",
                Email = "AJAMES",
                PhoneNumber = "1.590.555.0103",
                HireDate = new DateOnly(2016, 1, 3),
                JobId = "IT_PROG",
                Salary = 9000,
                CommissionPct = null,
                ManagerId = 102,
                DepartmentId = 60
            };

            var validationFailure = new ValidationFailure("FirstName", "Please provide a First Name.");
            var validationResult = new ValidationResult(new[] { validationFailure });

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<EmployeeDTO>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);
            
            var result = await _controller.ModifyEmployee(employeeid, employee);

           
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);

         
            var messageProperty = returnValue.GetType().GetProperty("message");
            var errorsProperty = returnValue.GetType().GetProperty("errors");
            var timeStampProperty = returnValue.GetType().GetProperty("timeStamp");

            Assert.NotNull(messageProperty);
            Assert.NotNull(errorsProperty);
            Assert.NotNull(timeStampProperty);

            var message = messageProperty.GetValue(returnValue)?.ToString();
            var errors = errorsProperty.GetValue(returnValue) as IEnumerable<string>;
            var timeStamp = timeStampProperty.GetValue(returnValue);

            Assert.Equal("Validation failed", message);
            Assert.Contains("Please provide a First Name.", errors);
            Assert.NotNull(timeStamp);
        }

        [Fact]
        public async Task AssignJob_EmployeeFound_ReturnsOk()
        {
            
            var currentJobId = "IT_PROG";
            var newJobId = "HR_MANAGER";

           
            _employeeRepoMock.Setup(repo => repo.AssignJob(currentJobId, newJobId))
                .Returns(Task.CompletedTask); 

            
            var result = await _controller.AssignJob(currentJobId, newJobId);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Record Modified Successfully", returnedValue);
        }
        [Fact]
        public async Task AssignJob_EmployeeNotFound_ReturnsBadRequest()
        {
           
            var currentJobId = "IT_PR";
            var newJobId = "HR_MANAGER";
            var exceptionMessage = $"No employee found with JobId '{currentJobId}'.";

            _employeeRepoMock.Setup(repo => repo.AssignJob(currentJobId, newJobId))
                             .ThrowsAsync(new Exception(exceptionMessage));

           
            var result = await _controller.AssignJob(currentJobId, newJobId);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);

           
            Console.WriteLine($"Returned object type: {returnValue.GetType()}");

           
            if (returnValue is ExpandoObject)
            {
                var dynamicReturnValue = (dynamic)returnValue;
                var message = dynamicReturnValue.message;
                var timeStamp = dynamicReturnValue.timeStamp;

                Assert.Equal(exceptionMessage, message);
                Assert.NotNull(timeStamp);
            }
            
        }
        [Fact]
        public async Task AssignMan_EmployeeFound_ReturnOk()
        {
            var employeeid = 101;
            var managerid = 100;
            _employeeRepoMock.Setup(repo => repo.AssignMan(employeeid, managerid))
                .Returns(Task.CompletedTask);

            var result= await _controller.AssignMan(employeeid,managerid);
            var resulttype= Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(resulttype.Value);
            Assert.Equal("Record Modified Successfully", returnValue);
        }
        [Fact]
        public async Task AssignMan_EmployeeNotFound_BadRequest()
        {
            var employeeid = 101;
            var managerid = 100;
            var excepmessage = $"Employee with ID {employeeid} not found.";
            _employeeRepoMock.Setup(repo=>repo.AssignMan(employeeid, managerid)).ThrowsAsync(new Exception(excepmessage));

            var result = await _controller.AssignMan(employeeid, managerid);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;
            
            Assert.NotNull(returnValue);

            if (returnValue is ExpandoObject)
            {
                var dynamicReturnValue = (dynamic)returnValue;
                var message = dynamicReturnValue.message;
                var timeStamp = dynamicReturnValue.timeStamp;

                Assert.Equal(excepmessage, message);
                Assert.NotNull(timeStamp);
            }
        }
        [Fact]
        public async Task AssignDep_EmployeeFound_ReturnOk()
        {
            var employeeid = 100;
            var departmentid = 30;
            _employeeRepoMock.Setup(repo=>repo.AssignDep(employeeid, departmentid)).Returns(Task.CompletedTask);

            var result= await _controller.AssignDep(employeeid,departmentid);
            var okresult= Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okresult.Value);
            Assert.Equal("Record Modified Successfully", returnValue);
        }
        [Fact]
        public async Task AssignDep_EmployeeNotFound_BadRequest()
        {
            var employeeid = 100;
            var departmentid = 30;
            var excepmessege = $"Employee with ID {employeeid} not found.";
            _employeeRepoMock.Setup(repo => repo.AssignDep(employeeid, departmentid)).ThrowsAsync(new Exception(excepmessege));

            var result = await _controller.AssignDep(employeeid, departmentid);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);

            if (returnValue is ExpandoObject)
            {
                var dynamicReturnValue = (dynamic)returnValue;
                var message = dynamicReturnValue.message;
                var timeStamp = dynamicReturnValue.timeStamp; 

                Assert.Equal(excepmessege, message);
                Assert.NotNull(timeStamp);
            }
        }
        [Fact]
        public async Task UpdateCommissionForDepartment_ReturnOk()
        {
            var departmentid = 30;
            var CommissionPct= 0.3m;
            _employeeRepoMock.Setup(repo=>repo.UpdateCommissionForDepartment(departmentid,CommissionPct)).Returns(Task.CompletedTask);
            var result= await _controller.UpdateCommissionForDepartment(departmentid,CommissionPct);
            var okresult= Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okresult.Value);
            Assert.Equal("Record Modified Successfully", returnValue);
        }
        [Fact]
        public async Task UpdateCommisionForDepartment_DepartmentNotFound_Badrequest()
        {
           
            var departmentid = 30;
            var CommissionPct = 0.3m;
            var exceptionMessage = $"Department with ID {departmentid} not found.";
            _employeeRepoMock.Setup(repo => repo.UpdateCommissionForDepartment(departmentid, CommissionPct))
                            .ThrowsAsync(new Exception(exceptionMessage));

            
            var result = await _controller.UpdateCommissionForDepartment(departmentid, CommissionPct);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = badRequestResult.Value;

            Assert.NotNull(returnValue);
            if (returnValue is ExpandoObject)
            {
                var dynamicReturnValue = (dynamic)returnValue;
                var message = dynamicReturnValue.message;
                var timeStamp = dynamicReturnValue.timeStamp;

                Assert.Equal(exceptionMessage, message);
                Assert.NotNull(timeStamp);
            }
        }

     }

}






