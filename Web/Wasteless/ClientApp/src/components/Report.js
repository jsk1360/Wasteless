import React, {useEffect, useState} from 'react';
import authService from './api-authorization/AuthorizeService'

function Iframe(props) {
    const iframe = '<iframe height="800px" style="width: 100%;" title="fx." src="' + props.src + '" allowtransparency="true" allowfullscreen="true"></iframe>';
    return (<div dangerouslySetInnerHTML={{__html: iframe}}/>);
}

const ReportIframe = props =>
    <Iframe src={props.reportUrl}/>

const Report = props => {
    const [loading, setLoading] = useState(true);
    const [reportUrl, setReportUrl] = useState("");

    useEffect(() => {
        fetchReportUrl();
    }, [])

    async function fetchReportUrl() {
        setLoading(true);
        const token = await authService.getAccessToken();
        const response = await fetch(`waste/report`, {
            headers: !token ? {} : {'Authorization': `Bearer ${token}`}
        });
        let data = await response.text();
        if (response.ok) {
            var user = await authService.userManager.getUser();
            console.log(user.profile);
            if (user.profile.location) {
                let url = new URL(data);
                url.searchParams.append('filter', `Locations/LocationSid eq ${user.profile.location}`);
                data = url.toString();
            }
            setReportUrl(data);
        }
        setLoading(false);
    }

    return (
        <div>
            {loading ? <span>Ladataan...</span> : <ReportIframe reportUrl={reportUrl}/>}
        </div>
    )
}

export default Report;
