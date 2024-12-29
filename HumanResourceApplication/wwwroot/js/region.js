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

    // Fetch all regions
    $('#getAllRegions').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/v1/Region/GetAllRegion', // API endpoint for all countries
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let list = data.map(
                    region => `<li>${region.regionId} ${region.regionName}</li>`
                ).join('');
                $('#regionList').html(list || '<li>No region available</li>');
            },
            error: handleError,
        });
    });


    // Fetch region by ID
    $('#getRegionById').click(function () {
        const regionId = $('#regionIdInput').val(); // Get input value from a field
        if (!regionId) {
            alert('Please enter a valid region ID.');
            return;
        }
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        $.ajax({
            url: `/api/v1/Region/${regionId}/id`, // API endpoint for GetRegionById
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                if (data) {
                    $('#regionDetail').html(
                        `<p>ID: ${data.regionId}</p><p>Name: ${data.regionName}</p>`
                    );
                } else {
                    $('#regionDetail').html('<p>No region found.</p>');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching region:', error);
                alert('Failed to fetch region.');
            },
        });
    });




    $('#addRegion').click(function () {
        const regionIdInput = $('#AddregionIdInput').val().trim(); // Fetch and trim input
        const regionId = parseFloat(regionIdInput); // Convert to decimal

        const regionName = $('#regionNameInput').val().trim();

        // Validate inputs
        if (!regionIdInput || isNaN(regionId) || !regionName) {
            alert('Please enter a valid Region ID and Region Name.');
            return;
        }

        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        const regionData = { regionId, regionName };

        $.ajax({
            url: '/api/v1/Region/AddRegion',
            type: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            contentType: 'application/json',
            data: JSON.stringify(regionData),
            success: function () {
                alert('Region added successfully!');
                $('#regionIdInput').val(''); // Clear inputs
                $('#regionNameInput').val('');
            },
            error: function (xhr, status, error) {
                console.error('Error details:', xhr.responseText);
                alert(`Failed to add region: ${xhr.responseText || error}`);
            },
        });
    });




});
