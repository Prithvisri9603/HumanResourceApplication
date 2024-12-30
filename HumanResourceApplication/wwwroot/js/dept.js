$(document).ready(function () {
    const token = localStorage.getItem('jwtToken');

    function handleError(xhr) {
        console.error(xhr);
        if (xhr.status === 401) {
            alert('Unauthorized. Please log in.');
            localStorage.removeItem('jwtToken');
        } else if (xhr.status === 404) {
            alert('Data not found.');
        } else {
            alert('An error occurred.');
        }
    }

    // Get all departments
    $('#getAllDepartments').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/v1/Department/GetAllDepartment', // API endpoint for all departments
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let rows = data.map(department => `
                <tr>
                    <td>${department.departmentId}</td>
                    <td>${department.departmentName}</td>
                    <td>${department.managerId || 'N/A'}</td>
                    <td>${department.locationId || 'N/A'}</td>
                </tr>
            `).join('');
                let table = `
                <table border="1" style="width: 100%; border-collapse: collapse;">
                    <thead>
                        <tr>
                            <th>Department ID</th>
                            <th>Department Name</th>
                            <th>Manager ID</th>
                            <th>Location ID</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${rows || '<tr><td colspan="4">No departments available</td></tr>'}
                    </tbody>
                </table>
            `;
                $('#allDepartmentsList').html(table);
            },
            error: handleError,
        });
    });



    // Get departments by Employee ID and display in a table
    $('#getDepartmentsByEmp').click(function () {
        const empId = $('#empId').val().trim();

        if (!empId) {
            alert('Please enter an Employee ID.');
            return;
        }

        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        $.ajax({
            url: `/api/v1/Department/${empId}`, // API endpoint
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                if (data.length > 0) {
                    let rows = data.map(dept => `
                    <tr>
                        <td>${dept.departmentId}</td>
                        <td>${dept.departmentName}</td>
                        <td>${dept.managerId || 'N/A'}</td>
                        <td>${dept.locationId || 'N/A'}</td>
                    </tr>
                `).join('');
                    let table = `
                    <table border="1" style="width: 100%; border-collapse: collapse;">
                        <thead>
                            <tr>
                                <th>Department ID</th>
                                <th>Department Name</th>
                                <th>Manager ID</th>
                                <th>Location ID</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${rows}
                        </tbody>
                    </table>
                `;
                    $('#departmentsList').html(table);
                } else {
                    $('#departmentsList').html('<p>No departments found for this Employee ID.</p>');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching departments:', xhr.responseText);
                alert(`Failed to fetch departments: ${xhr.responseText || error}`);
            },
        });
    });


    // Add new department
    $('#addDepartment').click(function () {
        const newDepartment = {
            departmentId: $('#newDepartmentId').val(),
            departmentName: $('#newDepartmentName').val(),
            managerId: $('#newDepartmentManagerId').val(),
            locationId: $('#newDepartmentLocationId').val()
        };

        if (!newDepartment.departmentId || !newDepartment.departmentName ||
            !newDepartment.managerId || !newDepartment.locationId) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: '/api/v1/Department/AddDepartment',
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(newDepartment),
            success: function (response) {
                alert('Department added successfully!');
                // Clear the form
                $('#newDepartmentId').val('');
                $('#newDepartmentName').val('');
                $('#newDepartmentManagerId').val('');
                $('#newDepartmentLocationId').val('');

                // Refresh the departments list if it exists
                if (typeof $('#getAllDepartments').click === 'function') {
                    $('#getAllDepartments').click();
                }
            },
            error: function (xhr) {
                if (xhr.status === 400) {
                    $('#addResult').html('<p class="error">Validation failed. Please check your input.</p>');
                } else {
                    handleError(xhr);
                }
            }
        });
    });


    // Update department
    $('#updateDepartment').click(function () {
        const departmentId = $('#updateDepartmentId').val();
        const updatedDepartment = {
            departmentId: departmentId,
            departmentName: $('#updateDepartmentName').val(),
            managerId: $('#updateDepartmentManagerId').val(),
            locationId: $('#updateDepartmentLocationId').val()
        };

        if (!departmentId || !updatedDepartment.departmentName ||
            !updatedDepartment.managerId || !updatedDepartment.locationId) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: `/api/v1/Department/Update?departmentId=${departmentId}`,
            type: 'PUT',
            contentType: 'application/json',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(updatedDepartment),
            success: function () {
                alert('Department updated successfully!');
                $('#updateDepartmentId, #updateDepartmentName, #updateDepartmentManagerId, #updateDepartmentLocationId').val('');
            },
            error: handleError
        });
    });

    // Find Max Salary in Department
    $('#findMaxSalary').click(function () {
        const departmentId = $('#maxSalaryDeptId').val();
        if (!departmentId) {
            alert('Please enter a department ID.');
            return;
        }

        $.ajax({
            url: `/api/v1/Department/findmaxsalary/${departmentId}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let html = '<h4>Maximum Salary Details:</h4>';
                for (const [deptName, maxSalary] of Object.entries(data)) {
                    html += `
<p>Department: ${deptName}</p>
<p>Maximum Salary: $${maxSalary?.toLocaleString() || 'N/A'}</p>
                    `;
                }
                $('#maxSalaryResult').html(html);
            },
            error: handleError
        });
    });

    // Find Min Salary in Department
    $('#findMinSalary').click(function () {
        const departmentId = $('#minSalaryDeptId').val();
        if (!departmentId) {
            alert('Please enter a department ID.');
            return;
        }

        $.ajax({
            url: `/api/v1/Department/findminsalary/${departmentId}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let html = '<h4>Minimum Salary Details:</h4>';
                for (const [deptName, minSalary] of Object.entries(data)) {
                    html += `
<p>Department: ${deptName}</p>
<p>Minimum Salary: $${minSalary?.toLocaleString() || 'N/A'}</p>
                    `;
                }
                $('#minSalaryResult').html(html);
            },
            error: handleError
        });
    });

    // Delete department
    $('#deleteDepartment').click(function () {
        const departmentId = $('#deleteDepartmentId').val();

        if (!departmentId) {
            alert('Please enter a department ID.');
            return;
        }

        // Confirm before deletion
        if (!confirm('Are you sure you want to delete this department?')) {
            return;
        }

        $.ajax({
            url: `/api/v1/Department/${departmentId}/Delete`,
            type: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            success: function (response) {
                $('#deleteResult').html('<p class="success">Department deleted successfully!</p>');
                $('#deleteDepartmentId').val('');

                // Refresh the departments list if it exists
                if (typeof $('#getAllDepartments').click === 'function') {
                    $('#getAllDepartments').click();
                }
            },
            error: function (xhr) {
                if (xhr.status === 404) {
                    $('#deleteResult').html('<p class="error">Department not found.</p>');
                } else {
                    handleError(xhr);
                }
            }
        });
    });

});