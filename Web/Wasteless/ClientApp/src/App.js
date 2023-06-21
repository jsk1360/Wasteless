import React, {useEffect} from 'react';
import {Layout} from './components/Layout';
import {hot} from 'react-hot-loader';

import './custom.css'
import Waste from "./components/Waste";
import Report from "./components/Report";

import {Route} from "react-router";
import {BrowserRouter as Router, Switch} from "react-router-dom";

import {MsalProvider, useAccount, useMsal} from "@azure/msal-react";
import {EventType, InteractionRequiredAuthError, InteractionType} from "@azure/msal-browser";

import {b2cPolicies, ProtectedContent, protectedResources} from "./authConfig";
import {useAccessToken} from "./accessTokenContext";

const Pages = () => {
    const [accessToken, setAccessToken] = useAccessToken()
    /**
     * useMsal is hook that returns the PublicClientApplication instance,
     * an array of all accounts currently signed in and an inProgress value
     * that tells you what msal is currently doing. For more, visit:
     * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/hooks.md
     */
    const {instance, inProgress, accounts} = useMsal();
    const account = useAccount(accounts[0] || {});

    useEffect(() => {
        if (account && inProgress === "none" && !accessToken) {
            instance.acquireTokenSilent({
                scopes: protectedResources.api.scopes,
                account: account
            }).then((response) => {
                setAccessToken(response.accessToken);
            }).catch((error) => {
                // in case if silent token acquisition fails, fallback to an interactive method
                if (error instanceof InteractionRequiredAuthError) {
                    if (account && inProgress === "none") {
                        instance.acquireTokenPopup({
                            scopes: protectedResources.api.scopes,
                        }).then((response) => {
                            setAccessToken(response.accessToken);
                        }).catch(error => console.log(error));
                    }
                }
            });
        }
    }, [inProgress, accounts, instance, accessToken, setAccessToken, account]);


    /**
     * Using the event API, you can register an event callback that will do something when an event is emitted.
     * When registering an event callback in a react component you will need to make sure you do 2 things.
     * 1) The callback is registered only once
     * 2) The callback is unregistered before the component unmounts.
     * For more, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/events.md
     */
    useEffect(() => {
        const callbackId = instance.addEventCallback((event) => {
            if (event.eventType === EventType.LOGIN_FAILURE) {
                if (event.error && event.error.errorMessage.indexOf("AADB2C90118") > -1) {
                    if (event.interactionType === InteractionType.Redirect) {
                        instance.loginRedirect(b2cPolicies.authorities.forgotPassword);
                    } else if (event.interactionType === InteractionType.Popup) {
                        instance.loginPopup(b2cPolicies.authorities.forgotPassword)
                            .catch(e => {
                                return;
                            });
                    }
                }
            }

            if (event.eventType === EventType.LOGIN_SUCCESS || event.eventType === EventType.ACQUIRE_TOKEN_SUCCESS) {
                if (event?.payload) {
                    /**
                     * We need to reject id tokens that were not issued with the default sign-in policy.
                     * "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr").
                     * To learn more about B2C tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
                     */
                    if (event.payload.idTokenClaims["tfp"] === b2cPolicies.names.forgotPassword) {
                        window.alert("Password has been reset successfully. \nPlease sign-in with your new password.");
                        return instance.logout();
                    } else if (event.payload.idTokenClaims["tfp"] === b2cPolicies.names.editProfile) {
                        window.alert("Profile has been edited successfully. \nPlease sign-in again.");
                        return instance.logout();
                    }
                }
            }
        });

        return () => {
            if (callbackId) {
                instance.removeEventCallback(callbackId);
            }
        };
    }, [instance]);

    return (
        <Switch>
            <Route exact path="/">
                <ProtectedContent>
                    <Waste/>
                </ProtectedContent>
            </Route>
            <Route path="/report">
                <ProtectedContent>
                    <Report/>
                </ProtectedContent>
            </Route>
        </Switch>
    )
}

const App = ({instance}) => {
    return (
        <Router>
            <MsalProvider instance={instance}>
                <Layout>
                    <Pages/>
                </Layout>
            </MsalProvider>
        </Router>
    );
}

export default hot(module)(App)
