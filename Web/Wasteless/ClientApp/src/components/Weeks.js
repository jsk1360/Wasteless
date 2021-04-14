import React, {useEffect, useState} from "react";
import moment from "moment";
import authService from "./api-authorization/AuthorizeService";
import {DayCard} from "./DayCard";

export const Weeks = (props) => {
    const [wastes, setWaste] = useState([]);
    const [menus, setMenus] = useState([]);
    const [isLoading, setLoading] = useState(true);
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
    }, [props.dates, props.location, parameters.dates, parameters.location])

    useEffect(() => {
        async function populateMenus(location) {
            setLoading(true);
            const token = await authService.getAccessToken();
            const response = await fetch(`menu?location=${location}`, {
                headers: !token ? {} : {'Authorization': `Bearer ${token}`}
            });
            const data = await response.json();
            setMenus(data);
            setLoading(false);
        }

        populateMenus(props.location);
    }, [props.location]);


    async function populateWasteData(date, location) {
        if (date === undefined) return;
        setLoading(true);
        const token = await authService.getAccessToken();
        const response = await fetch(`waste?date=${date.format("yyyy-MM-DD")}&location=${location}`, {
            headers: !token ? {} : {'Authorization': `Bearer ${token}`}
        });
        const data = await response.json();
        setWaste(data);
        setLoading(false);
    }

    return (isLoading ? <p>Ladataan...</p> :
            wastes.map(w =>
                <DayCard key={w.date} waste={w} menus={menus}/>
            )
    );
}