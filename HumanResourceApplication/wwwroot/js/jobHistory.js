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
    $('#getAllJobHistories').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/JobHistory',
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let rows = data.map(
                    history => `
                <tr>
                    <td>${history.employeeId}</td>
                    <td>${history.startDate}</td>
                    <td>${history.endDate}</td>
                    <td>${history.jobId}</td>
                    <td>${history.departmentId}</td>
                </tr>`
                ).join('');

                let table = `
            <table>
                <thead>
                    <tr>
                        <th>Employee ID</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th>Job ID</th>
                        <th>Department ID</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows || '<tr><td colspan="5">No job histories available</td></tr>'}
                </tbody>
            </table>`;

                $('#jobHistoryList').html(table);
            },
            error: handleError,
        });
    });


    $('#getJobHistoryById').click(function () {
        const jobHistoryId = $('#jobHistoryIdInput').val().trim();

        if (!jobHistoryId) {
            alert('Please enter a Job History ID.');
            return;
        }
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        $.ajax({
            url: `/api/JobHistory/${jobHistoryId}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                alert(`Job History: ${JSON.stringify(data)}`);
            },
            error: handleError,
        });
    });



    $('#getJobHistoryByExperience').click(function () {
        const empId = $('#empIdInput').val().trim();

        if (!empId) {
            alert('Please enter an Employee ID.');
            return;
        }
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        $.ajax({
            url: `/api/JobHistory/lessthanoneyearexperience/${empId}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                alert(`Job Histories: ${JSON.stringify(data)}`);
            },
            error: handleError,
        });
    });




    $('#addJobHistory').click(function () {
        // Fetch input values
        const empId = $('#newempId').val().trim();
        const startDate = $('#newstartDate').val().trim();
        const jobId = $('#newjobId').val().trim();
        const deptId = $('#newdeptId').val().trim();

        // Validate input values
        if (!empId || !startDate || !jobId || !deptId) {
            alert('Please fill in all the fields: Employee ID, Start Date, Job ID, and Department ID.');
            return;
        }

        // Construct the API URL
        const url = `/api/JobHistory/${empId}/${startDate}/${jobId}/${deptId}`;

        // Make the POST request
        $.ajax({
            url: url,
            type: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`, // Replace with your actual token if needed
            },
            success: function () {
                alert('Job history added successfully!');
            },
            error: function (xhr) {
                console.error('Error:', xhr.responseText);
                alert(`Error adding job history: ${xhr.responseText}`);
            },
        });
    });

    $('#updateJobHistory').click(function () {
        // Fetch input values
        const empId = $('#empId').val(); // Employee ID
        const startDate = $('#startDate').val(); // Start Date
        const endDate = $('#endDate').val(); // End Date



        // Construct the API URL
        const url = `/api/JobHistory/${empId}/${startDate}?enddate=${endDate}`;

        // Make the PUT request
        $.ajax({
            url: url,
            type: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`, // Include the token if authentication is required
            },
            success: function () {
                alert('Job history updated successfully!');
            },
            error: function (xhr) {
                console.error('Error:', xhr.responseText);
                alert(`Error updating job history: ${xhr.responseText}`);
            },
        });
    });






});
