import React, {useEffect, useState} from 'react';
import {useAccessToken} from "../accessTokenContext";
import {useMsal} from "@azure/msal-react";

function Iframe(props) {
    const iframe = '<iframe height="800px" style="width: 100%;" title="fx." src="' + props.src + '" allowtransparency="true" allowfullscreen="true"></iframe>';
    return (<div dangerouslySetInnerHTML={{__html: iframe}}/>);
}

const ReportIframe = props =>
    <Iframe src={props.reportUrl}/>

const Report = props => {
    const [loading, setLoading] = useState(true);
    const [reportUrl, setReportUrl] = useState("");
    const [accessToken] = useAccessToken()
    const {accounts} = useMsal();

    useEffect(() => {
        fetchReportUrl(accessToken);

        async function fetchReportUrl(token) {
            setLoading(true);
            const response = await fetch(`waste/report`, {
                headers: !token ? {} : {'Authorization': `Bearer ${token}`}
            });
            let data = await response.text();
            if (response.ok) {
                const user = accounts[0];
                if (user.idTokenClaims.city) {
                    let url = new URL(data);
                    url.searchParams.append('filter', `Locations/LocationSid eq ${user.idTokenClaims.city}`);
                    data = url.toString();
                }
                setReportUrl(data);
            }
            setLoading(false);
        }
    }, [accessToken, accounts])


    return (
        <div>
            {loading ? <span>Ladataan...</span> : <ReportIframe reportUrl={reportUrl}/>}
        </div>
    )
}

export default Report;
