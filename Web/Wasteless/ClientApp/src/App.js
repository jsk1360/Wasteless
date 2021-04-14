import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import {ApplicationPaths} from './components/api-authorization/ApiAuthorizationConstants';
import {hot} from 'react-hot-loader';

import './custom.css'
import Waste from "./components/Waste";
import Report from "./components/Report";

function App() {
    return <Layout>
        <AuthorizeRoute exact path='/' component={Waste}/>
        <AuthorizeRoute exact path='/report' component={Report}/>
        <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes}/>
    </Layout>
}

export default hot(module)(App)
