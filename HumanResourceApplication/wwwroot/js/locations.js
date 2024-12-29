
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
    $('#getLocations').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/Location/GetAllLocations',
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let list = data.map(location => `<li>${location.locationId} , ${location.streetAddress} , ${location.postalCode} , ${location.city} , ${location.stateProvince}, ${location.countryId} </li>`).join('');
                $('#locationList').html(list || '<li>No locations available</li>');
            },
            error: handleError,
        });
    });

    // Fetch locations
    $('#addLocation').click(function () {
        const countryId = $('#newCountryId').val().toUpperCase();
        console.log('Country ID:', countryId); 
        const location = {
            locationId: $('#newLocationId').val(),
            streetAddress: $('#newStreetAddress').val(),
            postalCode: $('#newPostalCode').val(),
            city: $('#newCity').val(),
            stateProvince: $('#newStateProvince').val(),
            countryId: countryId
        };
        $.ajax({
            url: '/api/Location',
            type: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            contentType: 'application/json',
            data: JSON.stringify(location),
            success: function () {
                alert('Location added successfully');
                $('#newLocationId, #newStreetAddress, #newPostalCode, #newCity, #newStateProvince, #newCountryId').val('');
            },
            error: handleError,
        });
    });

    $('#updateLocation').click(function () {
        const locationId = $('#updateLocationId').val();
        const updatedLocation = {
            locationId: locationId,
            streetAddress: $('#updateStreetAddress').val(),
            postalCode: $('#updatePostalCode').val(),
            city: $('#updateCity').val(),
            stateProvince: $('#updateStateProvince').val(),
            countryId: $('#updateCountryId').val().trim()
        };

        // Debug logs to verify inputs
        console.log('Updated Location:', updatedLocation);

        if (!locationId || !updatedLocation.streetAddress || !updatedLocation.postalCode || !updatedLocation.city || !updatedLocation.stateProvince || !updatedLocation.countryId) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: `/api/Location/id?=${locationId}`,
            type: 'PUT',
            contentType: 'application/json',
            headers: { 'Authorization': `Bearer ${token}` },
            data: JSON.stringify(updatedLocation),
            success: function () {
                alert('Location updated successfully!');
                $('#updateCountryId').val('');
                $('#updateCountryName').val('');
                $('#updateCountryCode').val('');
            },
            error: handleError,
        });
    });
});
