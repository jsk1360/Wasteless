import React, {useEffect, useState} from "react";
import authService from "./api-authorization/AuthorizeService";

export const LocationPicker = (props) => {
    const [locations, setLocations] = useState([]);
    const [selectedLocation, setSelectedLocation] = useState(undefined);

    useEffect(() => {
        PopulateLocations();
    }, [])

    async function PopulateLocations() {
        var token = await authService.getAccessToken();
        const response = await fetch('waste/locations', {
            headers: !token ? {} : {'Authorization': `Bearer ${token}`}
        });
        const data = await response.json();

        setLocations(data);
        setSelectedLocation(data[0].id);
        props.onChange(data[0].id);
    }

    function handleChange(event) {
        setSelectedLocation(event.target.value);
        props.onChange(event.target.value);
    }

    return (
        <form>
            <select className="form-control" value={selectedLocation} onChange={handleChange}>
                {locations.map(location =>
                    <option key={location.id} value={location.id}>{location.city} - {location.name}</option>
                )}
            </select>
        </form>
    )
}