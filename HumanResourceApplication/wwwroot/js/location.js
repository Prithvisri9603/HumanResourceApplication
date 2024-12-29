
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

        $('#getLocations').click(function () {
            if (!token) {
                alert('You are not authenticated. Please log in.');
                return;
            }

            $.ajax({
                url: '/api/Location/GetAllLocations', // API endpoint for all locations
                type: 'GET',
                headers: { 'Authorization': `Bearer ${token}` },
                success: function (data) {
                    console.log('API Response:', data); // Log the API response

                    if (!data || data.length === 0) {
                        console.log('No locations available');
                        $('#locationList').html('<p>No locations available</p>');
                        return;
                    }

                    // Generate table rows
                    let rows = data.map(location => `
                <tr>
                    <td>${location.locationId}</td>
                    <td>${location.streetAddress || 'N/A'}</td>
                    <td>${location.postalCode || 'N/A'}</td>
                    <td>${location.city}</td>
                    <td>${location.stateProvince || 'N/A'}</td>
                    <td>${location.countryId}</td>
                </tr>
            `).join('');

                    // Generate the full table
                    let table = `
                <table border="1" style="width: 100%; border-collapse: collapse;">
                    <thead>
                        <tr>
                            <th>Location ID</th>
                            <th>Street Address</th>
                            <th>Postal Code</th>
                            <th>City</th>
                            <th>State Province</th>
                            <th>Country ID</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${rows}
                    </tbody>
                </table>
            `;

                    // Update the location list container with the table
                    $('#locationList').html(table);
                },
                error: function (xhr, status, error) {
                    console.error('Error occurred:', error); // Log any errors
                    alert('Failed to fetch locations. Please try again later.');
                },
            });
        });



    // Add Location
    $('#addLocation').click(function () {
        console.log('Add Location button clicked'); // Debugging
    const location = {
        locationId: $('#newLocationId').val(),
    streetAddress: $('#newStreetAddress').val(),
    postalCode: $('#newPostalCode').val(),
    city: $('#newCity').val(),
    stateProvince: $('#newStateProvince').val(),
    countryId: $('#newCountryId').val().toUpperCase(),
        };
    console.log('Location to add:', location); // Debugging
    $.ajax({
        url: '/api/Location',
    type: 'POST',
    headers: {'Authorization': `Bearer ${token}` },
    contentType: 'application/json',
    data: JSON.stringify(location),
    success: function () {
        alert('Location added successfully');
    $('#newLocationId, #newStreetAddress, #newPostalCode, #newCity, #newStateProvince, #newCountryId').val('');
            },
    error: handleError,
        });
    });

    // Update Location
    $('#updateLocation').click(function () {
        console.log('Update Location button clicked'); // Debugging
    const locationId = $('#updateLocationId').val();
    const updatedLocation = {
        locationId: locationId,
    streetAddress: $('#updateStreetAddress').val(),
    postalCode: $('#updatePostalCode').val(),
    city: $('#updateCity').val(),
    stateProvince: $('#updateStateProvince').val(),
    countryId: $('#updateCountryId').val().trim(),
        };
    console.log('Updated Location:', updatedLocation); // Debugging
    if (!locationId) {
        alert('Location ID is required.');
    return;
        }
    $.ajax({
        url: `/api/Location/${locationId}`,
    type: 'PUT',
    headers: {'Authorization': `Bearer ${token}` },
    contentType: 'application/json',
    data: JSON.stringify(updatedLocation),
    success: function () {
        alert('Location updated successfully!');
    $('#updateLocationId, #updateStreetAddress, #updatePostalCode, #updateCity, #updateStateProvince, #updateCountryId').val('');
            },
    error: handleError,
        });
    });
});

