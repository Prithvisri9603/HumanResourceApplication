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

    // Fetch all countries
    $('#getAllCountries').click(function () {
        if (!token) {
            alert('You are not authenticated. Please log in.');
            return;
        }
        $.ajax({
            url: '/api/Country', // API endpoint for all countries
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                let list = data.map(
                    country => `<li>${country.countryId} ${country.countryName} (Region: ${country.regionId})</li>`
                ).join('');
                $('#countryList').html(list || '<li>No country available</li>');
            },
            error: handleError,
        });
    });

    
    // Fetch country by ID
    $('#getCountryById').click(function () {
        const countryId = $('#countryIdInput').val(); // Get the input value
        if (!countryId) {
            alert('Please enter a country ID.');
            return;
        }
        $.ajax({
            url: `/api/Country/id?Countryid=${countryId}`, // Construct the query parameter URL
            type: 'GET',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function (data) {
                $('#countryDetails').html(`<p>Country Name: ${data.countryName}</p>`);
            },
            error: function (xhr) {
                console.error(xhr);
                if (xhr.status === 404) {
                    alert('Country not found.');
                } else {
                    alert('An error occurred.');
                }
            }
        });
    });

    $('#addCountry').click(function () {
        const newCountry = {
            countryId: $('#newCountryId').val(),
            countryName: $('#newCountryName').val(),
            regionId: $('#newRegionId').val(),
        };

        if (!newCountry.countryId || !newCountry.countryName || !newCountry.regionId) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: '/api/Country',
            type: 'POST',
            contentType: 'application/json',
            headers: { 'Authorization': `Bearer ${token}` },
            data: JSON.stringify(newCountry),
            success: function () {
                alert('Country added successfully!');
                $('#newCountryId').val('');
                $('#newCountryName').val('');
                $('#newRegionId').val('');
            },
            error: handleError,
        });
    });

    $('#updateCountry').click(function () {
        const countryId = $('#updateCountryId').val();
        const updatedCountry = {
            countryId: countryId,
            countryName: $('#updateCountryName').val(),
            regionId: $('#updateCountryCode').val(),
        };

        if (!countryId || !updatedCountry.countryName || !updatedCountry.regionId) {
            alert('Please fill in all fields.');
            return;
        }

        $.ajax({
            url: `/api/Country?Countryid=${countryId}`,
            type: 'PUT',
            contentType: 'application/json',
            headers: { 'Authorization': `Bearer ${token}` },
            data: JSON.stringify(updatedCountry),
            success: function () {
                alert('Country updated successfully!');
                $('#updateCountryId').val('');
                $('#updateCountryName').val('');
                $('#updateCountryCode').val('');
            },
            error: handleError,
        });
    });

    $('#deleteCountry').click(function () {
        const countryId = $('#deleteCountryId').val();

        if (!countryId) {
            alert('Please enter a country ID.');
            return;
        }

        $.ajax({
            url: `/api/Country/id?id=${countryId}`,
            type: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` },
            success: function () {
                alert('Country deleted successfully!');
                $('#deleteCountryId').val('');
            },
            error: handleError,
        });
    });
});
