import React, {useEffect, useState} from "react";

import {callApiWithToken} from "../fetch";
import {useAccessToken} from "../accessTokenContext";

export const LocationPicker = (props) => {
    const [locations, setLocations] = useState([]);
    const [accessToken] = useAccessToken()
    const [selectedLocation, setSelectedLocation] = useState(undefined);

    useEffect(() => {
        if (accessToken)
            PopulateLocations(accessToken);

        async function PopulateLocations(accessToken) {
            const data = await callApiWithToken(accessToken, "waste/Locations");

            if (!data) return;
            setLocations(data);
            setSelectedLocation(data[0].id);
            props.onChange(data[0].id);
        }

    }, [accessToken, props])

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