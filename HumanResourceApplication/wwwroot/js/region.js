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

    // Fetch all regions and display in table format
    $('#getAllRegions').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/v1/Region/GetAllRegion', // API endpoint for all regions
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let rows = data.map(region => `
                    <tr>
                        <td>${region.regionId}</td>
                        <td>${region.regionName}</td>
                    </tr>
                `).join('');
                let table = `
                    <table border="1" style="width: 100%; border-collapse: collapse;">
                        <thead>
                            <tr>
                                <th>Region ID</th>
                                <th>Region Name</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${rows || '<tr><td colspan="2">No regions available</td></tr>'}
                        </tbody>
                    </table>
                `;
                $('#regionList').html(table);
            },
            error: handleError,
        });
    });

    // Fetch region by ID and display in table format
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
                    let table = `
                        <table border="1" style="width: 100%; border-collapse: collapse;">
                            <thead>
                                <tr>
                                    <th>Region ID</th>
                                    <th>Region Name</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>${data.regionId}</td>
                                    <td>${data.regionName}</td>
                                </tr>
                            </tbody>
                        </table>
                    `;
                    $('#regionDetail').html(table);
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

    $('#updateRegion').click(function () {
        const regionId = $('#updateRegionId').val();
        const updatedRegion = {
            regionId: regionId,
            regionName: $('#updateRegionName').val(),
        };

        if (!regionId || !updatedRegion.regionName) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: `/api/v1/Region/Update?regionId=${regionId}`,
            type: 'PUT',
            contentType: 'application/json',
            headers: { 'Authorization': `Bearer ${token}` },
            data: JSON.stringify(updatedRegion),
            success: function () {
                alert('Region updated successfully!');
                $('#updateRegionId').val('');
                $('#updateRegionName').val('');

            },
            error: handleError,
        });
    });

    $('#deleteRegion').click(function () {
        const regionId = $('#regionIdToDelete').val(); // Get the Region ID from input

        // Validate input
        if (!regionId) {
            alert('Please enter a valid Region ID to delete.');
            return;
        }

        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }

        // Construct the correct API endpoint
        const apiUrl = `/api/v1/Region/${regionId}/Delete`;

        $.ajax({
            url: apiUrl,
            type: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function () {
                alert(`Region with ID ${regionId} deleted successfully!`);
                $('#regionIdToDelete').val('');
            },
            error: function (xhr, status, error) {
                console.error('Error details:', xhr.responseText);
                alert(`Failed to delete region: ${xhr.responseText || error}`);
            },
        });
    });



});
