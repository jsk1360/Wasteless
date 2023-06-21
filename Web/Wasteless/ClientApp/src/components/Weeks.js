import React, {useEffect, useState} from "react";
import moment from "moment";
import {DayCard} from "./DayCard";
import {useAccessToken} from "../accessTokenContext";

export const Weeks = (props) => {
    const [wastes, setWaste] = useState([]);
    const [menus] = useState([]);
    const [isLoading, setLoading] = useState(true);
    const [accessToken] = useAccessToken()
    
    const [parameters, setParameters] = useState({
        dates: [],
        location: undefined
    })

    useEffect(() => {
        if (parameters.dates === props.dates && parameters.location === props.location) {
            return;
        }

        setParameters({
            dates: props.dates,
            location: props.location
        });

        let momentDays = props.dates.map(date => {
            return moment(date);
        }).filter(day => day.weekday() < 5);

        populateWasteData(momentDays[0], props.location);
        console.log("populate data");

        async function populateWasteData(date, location) {
            if (date === undefined) return;
            setLoading(true);
            const response = await fetch(`waste?date=${date.format("yyyy-MM-DD")}&location=${location}`, {
                headers: !accessToken ? {} : {'Authorization': `Bearer ${accessToken}`}
            });
            const data = await response.json();
            setWaste(data);
            setLoading(false);
        }
    }, [props.dates, props.location, parameters.dates, parameters.location, accessToken])

    // useEffect(() => {
    //     async function populateMenus(location) {
    //         setLoading(true);
    //         const response = await fetch(`menu?location=${location}`, {
    //             headers: !accessToken ? {} : {'Authorization': `Bearer ${accessToken}`}
    //         });
    //         const data = await response.json();
    //         setMenus(data);
    //         setLoading(false);
    //     }
    //
    //     populateMenus(props.location);
    // }, [props.location]);


    return (isLoading ? <p>Ladataan...</p> :
            wastes.length > 0 ? wastes.map(w =>
                <DayCard key={w.date} waste={w} menus={menus}/>) : <p>Sijainnille ja valitulle ajalle ei löytynyt hävikki tietoja</p>);
}