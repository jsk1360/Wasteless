import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import React from 'react';
import ReactDOM from 'react-dom';
import {BrowserRouter} from 'react-router-dom';
import App from './App';
import {PublicClientApplication} from "@azure/msal-browser";
import {msalConfig} from "./authConfig";
import {AccessTokenProvider} from "./accessTokenContext";
//import registerServiceWorker from './registerServiceWorker';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const msalInstance = new PublicClientApplication(msalConfig);

const render = Component => {
    return ReactDOM.render(
        <AccessTokenProvider>
            <BrowserRouter basename={baseUrl}>
                <Component instance={msalInstance}/>
            </BrowserRouter>
        </AccessTokenProvider>,

        rootElement);
}

render(App);

if (module.hot) {
    module.hot.accept('./App', () => {
        const NextApp = require('./App').default;
        render(NextApp);
    })
}

// Uncomment the line above that imports the registerServiceWorker function
// and the line below to register the generated service worker.
// By default create-react-app includes a service worker to improve the
// performance of the application by caching static assets. This service
// worker can interfere with the Identity UI, so it is
// disabled by default when Identity is being used.
//
//registerServiceWorker();

