$(document).ready(function () {
    const token = localStorage.getItem('jwtToken'); // Assuming the JWT token is stored in localStorage

    // Handle Add Employee form submission
    $('#addEmployeeForm').submit(function (event) {
        event.preventDefault(); // Prevent the default form submission
        console.log("Form submitted!");

        // Get the phone number and format it
        let phone = $('#addPhone').val();
        

        // Create the employee object with all the data from the form
        const employee = {
            employeeId: $('#addEmployeeId').val(),
            firstName: $('#addFirstName').val(),
            lastName: $('#addLastName').val(),
            email: $('#addEmail').val(),
            phone: phone, // Formatted phone number
            jobId: $('#addJobId').val(),
            commissionPct: parseFloat($('#addCommissionPct').val()),
            departmentId: $('#addDepartmentId').val() ? parseFloat($('#addDepartmentId').val()) : null,
            salary: parseFloat($('#addSalary').val()), // Salary
            managerId: $('#addManagerId').val() ? parseInt($('#addManagerId').val()) : null // Manager ID
        };

        // Call the function to send employee data to the backend
        addEmployeeData(employee);
    });

    


    // Function to send the new employee data to the backend
    function addEmployeeData(employee) {
        console.log('Sending employee data:', employee);
        const token = localStorage.getItem('jwtToken'); // Ensure token is available

        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        $.ajax({
            url: '/api/Employees/Add new Employee', // Correct API endpoint for adding an employee
            type: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            contentType: 'application/json',
            data: JSON.stringify(employee),
            success: function () {
                alert('Employee added successfully.');
                $('#addEmployeeForm')[0].reset(); // Clear the form after success
            },
            error: function (xhr) {
                const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                alert(errorMessage);
            }
        });
    }




    // Search by First Name
    $('#searchEmployeeByFirstNameBtn').click(function () {
        const firstName = $('#searchByFirstName').val().trim();
        if (!firstName) {
            displayError('Please enter a first name.');
            return;
        }
        $('#errorMessages').hide();
        fetchEmployeeByFirstName(firstName);
    });

    // Search by Email
    $('#searchEmployeeByEmailBtn').click(function () {
        const email = $('#searchByEmail').val().trim();
        if (!email) {
            displayError('Please enter an email address.');
            return;
        }
        $('#errorMessages').hide();
        fetchEmployeeByEmail(email);
    });

    // Search by Phone Number
    $('#searchEmployeeByPhoneBtn').click(function () {
        const phone = $('#searchByPhone').val().trim();
        if (!phone) {
            displayError('Please enter a phone number.');
            return;
        }
        $('#errorMessages').hide();
        fetchEmployeeByPhone(phone);
    });

    // Fetch employee by First Name
    function fetchEmployeeByFirstName(firstName) {
        $.ajax({
            url: `/api/Employees/find by fisrt name?firstName=${firstName}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                displayEmployee(data);
            },
            error: function (xhr) {
                displayError(xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.');
            }
        });
    }

    // Fetch employee by Email
    function fetchEmployeeByEmail(email) {
        $.ajax({
            url: `/api/Employees/findemail?email=${email}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                displayEmployee(data);
            },
            error: function (xhr) {
                displayError(xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.');
            }
        });
    }

    // Fetch employee by Phone Number
    function fetchEmployeeByPhone(phone) {
        $.ajax({
            url: `/api/Employees/find phone?phone=${phone}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                displayEmployee(data);
            },
            error: function (xhr) {
                displayError(xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.');
            }
        });
    }

    // Display employee details
    function displayEmployee(employee) {
        $('#employeeDetails').show();
        $('#employeeId').text(employee.employeeId);
        $('#firstName').text(employee.firstName);
        $('#lastName').text(employee.lastName);
        $('#email').text(employee.email);
        $('#phoneNumber').text(employee.phoneNumber);
        $('#hireDate').text(employee.hireDate);
        $('#jobId').text(employee.jobId);
        $('#salary').text(employee.salary);
        $('#managerId').text(employee.managerId);
        $('#departmentId').text(employee.departmentId);
    }

    // Display error messages
    function displayError(message) {
        $('#errorMessages ul').empty().append(`<li>${message}</li>`);
        $('#errorMessages').show();
        $('#employeeDetails').hide();
    }

    $(document).ready(function () {
        // Fetch employee data by ID (simulate fetching from the backend)
        $('#employeeId').on('blur', function () {
            const employeeId = $('#employeeId').val();
            if (employeeId) {
                // In a real scenario, you would use an API call to fetch the employee details by ID
                fetchEmployeeData(employeeId);
            }
        });

        // Fetch employee data (simulate API call)
        function fetchEmployeeData(employeeId) {
            // Mock employee data (simulate a successful API response)
            const mockEmployeeData = {
                employeeId: 123,
                firstName: "John",
                lastName: "Doe",
                email: "john.doe@example.com",
                jobId: "IT_PROG",
                departmentId: 20,
                commissionPct: 0.1
            };

            // Pre-fill the form with fetched data
            $('#employeeId').val(mockEmployeeData.employeeId);
            $('#firstName').val(mockEmployeeData.firstName);
            $('#lastName').val(mockEmployeeData.lastName);
            $('#email').val(mockEmployeeData.email);
            $('#jobId').val(mockEmployeeData.jobId);
            $('#departmentId').val(mockEmployeeData.departmentId);
            $('#commissionPct').val(mockEmployeeData.commissionPct);
        }

    });


    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // Assuming JWT token is stored in localStorage

        // Function to load manager details when the button is clicked
        $('#loadManagersBtn').click(function () {
            $('#errorMessages').hide(); // Hide any previous error messages
            fetchManagers(); // Call function to fetch manager data
        });

        // Function to fetch manager details from the backend API
        function fetchManagers() {
            $.ajax({
                url: `/api/Employees/list All Manager Details`,  // Corrected URL to replace spaces with %20
                type: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }, // Send the JWT token in headers for authorization
                success: function (data) {
                    console.log(data); // Log the data to the console to see what is returned
                    displayManagers(data); // On success, call displayManagers function
                },
                error: function (xhr) {
                    const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                    displayError(errorMessage); // If error occurs, show error message
                }
            });
        }

        // Function to display manager data in the table
        function displayManagers(managers) {
            const tbody = $('#managerTable tbody');
            tbody.empty(); // Clear previous data in the table

            if (managers.length === 0) {
                tbody.append('<tr><td colspan="7">No managers found.</td></tr>'); // If no managers, show message
                return;
            }

            managers.forEach(function (manager) {
                tbody.append(`
<tr>
<td>${manager.employeeId}</td>
<td>${manager.firstName}</td>
<td>${manager.lastName}</td>
<td>${manager.email}</td>
<td>${manager.jobId}</td>
<td>${manager.departmentId}</td>
<td>${manager.commissionPct}</td>
</tr>
            `); // Insert manager data as rows in the table
            });

            $('#managerTable').show(); // Show the table after data is loaded
        }

        // Function to display error messages
        function displayError(message) {
            $('#errorMessages ul').empty(); // Clear any previous error messages
            $('#errorMessages ul').append(`<li>${message}</li>`); // Append the new error message
            $('#errorMessages').show(); // Show the error message box
        }
    });

    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // Assuming JWT token is stored in localStorage

        // Function to load employees by department when the button is clicked
        $('#getEmployeesByDepartmentButton').click(function () {
            const departmentId = $('#departmentIdInput').val(); // Get department ID from input field

            if (!departmentId) {
                alert('Please enter a department ID.');
                return;
            }

            $('#errorMessages').hide(); // Hide any previous error messages
            fetchEmployeesByDepartment(departmentId); // Call function to fetch employees by department
        });

        // Function to fetch employees by department from the backend API
        function fetchEmployeesByDepartment(departmentId) {
            $.ajax({
                url: `/api/employees/list All Employees by department?departmentId=${departmentId}`,  // API endpoint for employees by department
                type: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }, // Send the JWT token in headers for authorization
                success: function (data) {
                    console.log(data); // Log the data to the console to see what is returned
                    displayEmployees(data); // On success, call displayEmployees function
                },
                error: function (xhr) {
                    const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                    displayError(errorMessage); // If error occurs, show error message
                }
            });
        }

        // Function to display employee data in the table
        function displayEmployees(employees) {
            const tbody = $('#employeeTable tbody');
            tbody.empty(); // Clear previous data in the table

            if (employees.length === 0) {
                tbody.append('<tr><td colspan="7">No employees found in this department.</td></tr>'); // If no employees, show message
                return;
            }

            employees.forEach(function (employee) {
                tbody.append(`
<tr>
<td>${employee.employeeId}</td>
<td>${employee.firstName}</td>
<td>${employee.lastName}</td>
<td>${employee.email}</td>
<td>${employee.jobId}</td>
<td>${employee.departmentId}</td>
<td>${employee.salary || 'Not Available'}</td>
</tr>
            `); // Insert employee data as rows in the table
            });

            $('#employeeTable').show(); // Show the table after data is loaded
        }

        // Function to display error messages
        function displayError(message) {
            $('#errorMessages ul').empty(); // Clear any previous error messages
            $('#errorMessages ul').append(`<li>${message}</li>`); // Append the new error message
            $('#errorMessages').show(); // Show the error message box
        }
    });
    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // Assuming JWT token is stored in localStorage

        // Function to load employees without commission when the button is clicked
        $('#getEmployeesWithoutCommissionButton').click(function () {
            $('#errorMessages').hide(); // Hide any previous error messages
            fetchEmployeesWithoutCommission(); // Call function to fetch employees without commission
        });

        // Function to fetch employees without commission from the backend API
        function fetchEmployeesWithoutCommission() {
            $.ajax({
                url: `/api/employees/find All Employee With No Commission`,  // API endpoint for employees without commission
                type: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }, // Send the JWT token in headers for authorization
                success: function (data) {
                    console.log(data); // Log the data to the console to see what is returned
                    displayEmployees(data); // On success, call displayEmployees function
                },
                error: function (xhr) {
                    const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred.';
                    displayError(errorMessage); // If error occurs, show error message
                }
            });
        }

        // Function to display employee data in the new table
        function displayEmployees(employees) {
            const tbody = $('#employeesWithoutCommissionTable tbody');
            tbody.empty(); // Clear previous data in the table

            if (employees.length === 0) {
                tbody.append('<tr><td colspan="8">No employees found without commission.</td></tr>'); // If no employees found, show message
                return;
            }

            employees.forEach(function (employee) {
                tbody.append(`
<tr>
<td>${employee.employeeId}</td>
<td>${employee.firstName}</td>
<td>${employee.lastName}</td>
<td>${employee.email}</td>
<td>${employee.jobId}</td>
<td>${employee.departmentId}</td>
<td>${employee.salary || 'Not Available'}</td>
<td>${employee.commissionPct || 'No Commission'}</td>
</tr>
            `); // Insert employee data as rows in the new table
            });

            $('#employeesWithoutCommissionTable').show(); // Show the new table after data is loaded
        }

        // Function to display error messages
        function displayError(message) {
            $('#errorMessages ul').empty(); // Clear any previous error messages
            $('#errorMessages ul').append(`<li>${message}</li>`); // Append the new error message
            $('#errorMessages').show(); // Show the error message box
        }
    });
    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // Assuming JWT token is stored in localStorage

        // Populate the department dropdown with IDs between 10 and 120
        function populateDepartmentDropdown() {
            const departmentIds = Array.from({ length: 111 }, (_, i) => i + 10); // Generate IDs from 10 to 120

            const departmentSelect = $('#departmentIdSelect');
            departmentSelect.empty(); // Clear any previous options
            departmentSelect.append('<option value="">Select Department</option>'); // Add default option

            // Add department options to dropdown
            departmentIds.forEach(departmentId => {
                departmentSelect.append(`<option value="${departmentId}">${departmentId}</option>`);
            });
        }

        // Fetch Total Commission by Department when the button is clicked
        $('#getTotalCommissionButton').click(function () {
            const departmentId = $('#departmentIdSelect').val(); // Get department ID from the dropdown

            if (!departmentId) {
                alert('Please select a department.');
                return;
            }

            $('#errorMessages').hide(); // Hide any previous error messages
            fetchTotalCommission(departmentId); // Call function to fetch total commission by department
        });

        // Fetch total commission issued to employees by department from the backend API
        function fetchTotalCommission(departmentId) {
            $.ajax({
                url: `/api/Employees/find Total Commission Issued To Employee?departmentId=${departmentId}`, // The API URL with the departmentId
                type: 'GET',
                headers: { 'Authorization': `Bearer ${token}` },
                success: function (data) {
                    displayCommission(data); // Call function to display the commission value
                },
                error: function (xhr, status, error) {
                    const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred while fetching commission data.';
                    displayError(errorMessage); // Show error message if API request fails
                }
            });
        }

        // Display total commission in the result section
        function displayCommission(data) {
            if (data && data.sum !== undefined) {
                $('#commissionResult').show(); // Show the result section
                $('#totalCommissionValue').text(data.sum || '0'); // Set the total commission value
            } else {
                displayError('No commission data returned.');
            }
        }

        // Display error messages
        function displayError(message) {
            $('#errorMessages ul').empty(); // Clear any previous error messages
            $('#errorMessages ul').append(`<li>${message}</li>`); // Append the error message
            $('#errorMessages').show(); // Show the error message box
            $('#commissionResult').hide(); // Hide the commission result section
        }

        // Call the populateDepartmentDropdown function on page load to populate the departments
        populateDepartmentDropdown();
    });

    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // JWT token for authentication

        function handleError(xhr) {
            console.error(xhr);
            if (xhr.status === 401) {
                alert('Unauthorized. Please log in.');
                localStorage.removeItem('jwtToken'); // Clear token on unauthorized error
            } else {
                alert('An error occurred.');
            }
        }

        $(document).ready(function () {
            const token = localStorage.getItem('jwtToken'); // Get JWT token from localStorage

            // Button click event to fetch max salary
            $('#findMaxSalaryButton').click(function () {
                const employeeId = $('#MaxemployeeId').val();  // Get employee ID input

                // Validate if employeeId is provided
                if (!employeeId) {
                    alert("Please enter a valid Employee ID.");
                    return;
                }

                // AJAX request to fetch max salary of job by employee ID
                $.ajax({
                    url: `/api/Employees/find%20max%20salary%20of%20job?empid=${employeeId}`, // Controller endpoint with query parameter
                    type: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}` // JWT token for authentication
                    },
                    success: function (response) {
                        // Display the result (Job Title and Max Salary)
                        $('#resultContainer').html(`
                    <h3>Job Title: ${response.jobTitle}</h3>
                    <p><strong>Max Salary: $${response.maxSalary}</strong></p>
                `);
                    },
                    error: function (xhr, status, error) {
                        // Handle error if response fails
                        const errorMessage = xhr.responseJSON ? xhr.responseJSON.message : "An error occurred";
                        $('#resultContainer').html(`
                    <p style="color: red;">Error: ${errorMessage}</p>
                `);
                    }
                });
            });
        });

    });

    $(document).ready(function () {
        const token = localStorage.getItem('jwtToken'); // JWT token for authentication

        function handleError(xhr) {
            console.error(xhr);
            if (xhr.status === 401) {
                alert('Unauthorized. Please log in.');
                localStorage.removeItem('jwtToken'); // Clear token on unauthorized error
            } else {
                alert('An error occurred while updating the email.');
            }
        }

        // JavaScript to handle the form submission and update employee email using AJAX
        $('#updateEmailForm').submit(function (event) {
            event.preventDefault();  // Prevent the default form submission

            var currentemail = $('#currentemail').val();  // Get the current email from the form
            var newemail = $('#newemail').val();  // Get the new email from the form

            // Make the AJAX PUT request to the backend API
            $.ajax({
                url: `/api/Employees/Update Email?currentemail=${currentemail}&newemail=${newemail}`,
                type: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}` // JWT token for authentication
                },
                success: function (response) {
                    // On success, display the success message in the #response div
                    $('#response').html(`<p style="color:green;">${response}</p>`);
                },
                error: function (error) {
                    // On error, display the error message in the #response div
                    $('#response').html(`<p style="color:red;">Error: ${error}</p>`);
                }
            });
        });

    });
    // Assign Manager (AJAX)
    $('#assignManagerForm').submit(function (event) {
        event.preventDefault();
        const token = localStorage.getItem('jwtToken');
        var employeeId = $('#AssignemployeeId').val();  // Get Employee ID from input field
        var managerId = $('#AssignmanagerId').val();  // Get Manager ID from input field

        // Make the AJAX PUT request to assign the manager
        $.ajax({
            url: `/api/Employees/Assign Manager?employeeId=${employeeId}&managerId=${managerId}`,
            type: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}` // JWT token for authentication
            },
            success: function (response) {
                $('#AssignresultContainer').html(`<p>${response}</p>`);  // Show success message
            },
            error: function (error) {
                $('#response').html(`<p>Error: ${error.responseJSON.message}</p>`);  // Show error message
            }
        });
    });

    $(document).ready(function () {
        // Handle form submission for modifying employee details
        $('#modifyEmployeeForm').submit(function (event) {
            event.preventDefault();  // Prevent the default form submission
            const token = localStorage.getItem('jwtToken'); // Get JWT token for authorization

            // Get form data
            var employeeId = $('#modemployeeId').val(); // Employee ID will be passed as query parameter
            var email = $('#modemail').val();
            console.log('Employee ID:', employeeId); // Check if this is undefined



            var employeeData = {
                employeeId: employeeId,
                firstName: $('#modfirstName').val(),
                lastName: $('#modlastName').val(),
                email: email,
                jobId: $('#modjobId').val(),
                commissionPercentage: $('#modcommissionPct').val(),
                departmentId: $('#moddepartmentId').val()
            };
            console.log('Employee Data:', employeeData);
            // Make the AJAX PUT request to modify the employee
            $.ajax({
                url: `/api/Employees/Modify?employeeId=${employeeId}`,  // Pass employeeId as query parameter
                type: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}` // JWT token for authentication
                },
                contentType: 'application/json',
                data: JSON.stringify(employeeData),  // Send the modified employee data as JSON
                success: function (response) {
                    // On success, display the success message
                    $('#modresponse').html(`<p style="color:green;">${response}</p>`);
                },
                error: function (error) {
                    // On error, display the error message
                    $('#modresponse').html(`<p style="color:red;">Error: ${error}</p>`);
                }
            });
        });


    });



    $(document).ready(function () {
        // Handle form submission
        $('#assignDepartmentForm').submit(function (event) {
            event.preventDefault();
            const token = localStorage.getItem('jwtToken');// Prevent default form submission

            // Get employee ID and department ID from the form
            var employeeId = $('#assemployeeId').val();
            var departmentId = $('#assdepartmentId').val();

            // Validate inputs (optional but recommended)
            if (!employeeId || !departmentId) {
                alert("Please provide both Employee ID and Department ID.");
                return;
            }

            // Send the PUT request to assign the department
            $.ajax({
                url: `/api/Employees/Assign Department?employeeId=${employeeId}&departmentId=${departmentId}`,  // Your API endpoint URL
                type: 'PUT',

                contentType: 'application/json',
                data: JSON.stringify({  // Send the data as a JSON object
                    employeeId: employeeId,
                    departmentId: departmentId
                }),
                headers: {
                    'Authorization': `Bearer ${token}` // JWT token for authentication
                },
                success: function (response) {
                    // On success, display the success message
                    $('#assresponse').html(`<p style="color:green;">${response}</p>`);
                },
                error: function (error) {
                    // On error, display the error message
                    $('#assresponse').html(`<p style="color:red;">Error: ${error.responseJSON.message}</p>`);
                }
            });
        });
    });
    $(document).ready(function () {
        // Handle form submission
        $('#updateCommissionForm').submit(function (event) {
            event.preventDefault();  // Prevent default form submission
            const token = localStorage.getItem('jwtToken'); // Get JWT token for authorization

            // Get department ID and commission percentage from the form
            var departmentId = $('#updepartmentId').val();
            var commissionPercentage = $('#upcommissionPercentage').val();

            // Validate inputs (commission percentage must be between 0 and 1)
            if (!departmentId || !commissionPercentage) {
                alert("Please provide both Department ID and Commission Percentage.");
                return;
            }

            if (commissionPercentage < 0 || commissionPercentage > 1) {
                alert("Commission Percentage must be between 0 and 1.");
                return;
            }

            // Send the PUT request to update the commission
            $.ajax({
                url: `/api/Employees/update Commission For Sales Department?commissionPercentage=${commissionPercentage}&departmentId=${departmentId}`,  // Your API endpoint URL
                type: 'PUT',
                data: {
                    departmentId: departmentId,
                    commissionPercentage: commissionPercentage
                },
                headers: {
                    'Authorization': `Bearer ${token}` // JWT token for authentication
                },
                success: function (response) {
                    // On success, display the success message
                    $('#upresponse').html(`<p style="color:green;">${response}</p>`);
                },
                error: function (error) {
                    // On error, display the error message
                    $('#upresponse').html(`<p style="color:red;">Error: ${error}</p>`);
                }
            });
        });
    });

    $(document).ready(function () {
        // Handle form submission for assigning a new job to an employee
        $('#assignJobForm').submit(function (event) {
            event.preventDefault();  // Prevent the default form submission

            const token = localStorage.getItem('jwtToken'); // Get JWT token for authorization

            // Get form data

            var currentJobId = $('#assjobidcurrentJobId').val();
            var newJobId = $('#assjobidnewJobId').val();



            // Make the AJAX PUT request to assign the new job to the employee
            $.ajax({
                url: `/api/Employees/Assign%20Job?currentJobId=${currentJobId}&newJobId=${newJobId}`,  // currentJobId, and newJobId as query parameters
                type: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}` // JWT token for authentication
                },
                success: function (response) {
                    // On success, display the success message
                    $('#assjobidresponse').html(`<p style="color:green;">${response}</p>`);
                },
                error: function (error) {
                    // On error, display the error message
                    $('#assjobidresponse').html(`<p style="color:red;">Error: ${error.responseJSON.message}</p>`);
                }
            });
        });

        $(document).ready(function () {
            // Handle the button click to fetch employee count by location
            $('#fetchEmployeeCount').click(function () {
                const token = localStorage.getItem('jwtToken'); // Get JWT token from local storage

                // AJAX request to fetch employee counts grouped by location
                $.ajax({
                    url: `/api/Employees/location wise count of employees`,  // Adjust URL as necessary
                    type: 'GET',  // Make sure the backend is expecting GET request
                    headers: {
                        'Authorization': `Bearer ${token}` // Add Authorization header with JWT token
                    },
                    success: function (response) {
                        let resultHTML = "<h4>Employee Count by Location:</h4><ul>";

                        // Loop through the result and build the list of locations and their employee counts
                        $.each(response, function (location, count) {
                            resultHTML += `<li><strong>${location}</strong>: ${count} employees</li>`;
                        });

                        resultHTML += "</ul>";

                        // Display the results in the div
                        $('#employeeCountResult').html(resultHTML);
                    },
                    error: function (error) {
                        // Display error message if the request fails
                        const errorMessage = error.responseJSON ? error.responseJSON.message : "An error occurred";
                        $('#employeeCountResult').html(`<p style="color:red;">Error: ${errorMessage}</p>`);
                    }
                });
            });
        });

    });


});
