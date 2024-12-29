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

    // Fetch all jobs
    $('#getJobs').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/Job',
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let list = data.map(job => `<li>${job.jobTitle} (Salary: ${job.minSalary}-${job.maxSalary})</li>`).join('');
                $('#jobList').html(list || '<li>No jobs available</li>');
            },
            error: handleError,
        });
    });

    // Fetch job by ID
    $('#getJobById').click(function () {
        const jobId = $('#jobIdInput').val();
        if (!jobId) {
            alert('Please enter a job ID.');
            return;
        }
        $.ajax({
            url: `/api/Job/${jobId}`,
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                $('#jobDetails').html(`<p>${data.jobTitle} (Salary: ${data.minSalary}-${data.maxSalary})</p>`);
            },
            error: handleError,
        });
    });


    // Add a new job
    $('#addJob').click(function () {
        const job = {
            jobId: $('#newJobId').val(),
            jobTitle: $('#newJobTitle').val(),
            minSalary: parseFloat($('#newMinSalary').val()),
            maxSalary: parseFloat($('#newMaxSalary').val()),
        };
        $.ajax({
            url: '/api/Job',
            type: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            contentType: 'application/json',
            data: JSON.stringify(job),
            success: function () {
                alert('Job added successfully');
                $('#newJobId, #newJobTitle, #newMinSalary, #newMaxSalary').val('');
            },
            error: handleError,
        });
    });

    // Update a job
    $('#updateJob').click(function () {
        const jobId = $('#updateJobId').val();
        const job = {
            jobTitle: $('#updateJobTitle').val(),
            minSalary: parseFloat($('#updateMinSalary').val()),
            maxSalary: parseFloat($('#updateMaxSalary').val()),
        };
        $.ajax({
            url: `/api/Job/${jobId}`,
            type: 'PUT',
            headers: { 'Authorization': `Bearer ${token}` },
            contentType: 'application/json',
            data: JSON.stringify(job),
            success: function () {
                alert('Job updated successfully');
                $('#updateJobId, #updateJobTitle, #updateMinSalary, #updateMaxSalary').val('');
            },
            error: handleError,
        });
    });

    // Update job salary
    $('#updateJobSalary').click(function () {
        const jobId = $('#updateSalaryJobId').val();
        const newMinSalary = parseFloat($('#updateNewMinSalary').val());
        const newMaxSalary = parseFloat($('#updateNewMaxSalary').val());
        $.ajax({
            url: `/api/Job/minsalary/maxsalary${jobId}?newMin=${newMinSalary}&newMax=${newMaxSalary}`,
            type: 'PUT',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function () {
                alert('Job salary updated successfully');
                $('#updateSalaryJobId, #updateNewMinSalary, #updateNewMaxSalary').val('');
            },
            error: handleError,
        });
    });
});
