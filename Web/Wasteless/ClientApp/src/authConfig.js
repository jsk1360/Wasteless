import {LogLevel} from "@azure/msal-browser";
import React from 'react';
import {MsalAuthenticationTemplate} from "@azure/msal-react";
import {InteractionType} from "@azure/msal-browser";

export const b2cPolicies = {
    names: {
        signUpSignIn: "b2c_1_signin"
    },
    authorities: {
        signUpSignIn: {
            authority: "https://WastelessDemoOrg.b2clogin.com/WastelessDemoOrg.onmicrosoft.com/b2c_1_signin"
        }
    },
    authorityDomain: "WastelessDemoOrg.b2clogin.com"
}

export const msalConfig = {
    auth: {
        clientId: "8476fc31-8782-4304-b37d-a4553247d26e", // This is the ONLY mandatory field that you need to supply.
        authority: b2cPolicies.authorities.signUpSignIn.authority, 
        knownAuthorities: [b2cPolicies.authorityDomain], 
        redirectUri: "/", 
        postLogoutRedirectUri: "/", 
        navigateToLoginRequestUrl: false, 
    },
    cache: {
        cacheLocation: "sessionStorage", // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) {
                    return;
                }
                switch (level) {
                    case LogLevel.Error:
                        console.error(message);
                        return;
                    case LogLevel.Info:
                        console.info(message);
                        return;
                    case LogLevel.Verbose:
                        console.debug(message);
                        return;
                    case LogLevel.Warning:
                        console.warn(message);
                        return;
                    default:
                        break;
                }
            }
        }
    }
};

export const protectedResources = {
    api: {
        scopes: ["https://WastelessDemoOrg.onmicrosoft.com/wasteless-api/wasteless_read"],
    },
}

export const loginRequest = {
    scopes: [...protectedResources.api.scopes]
};


export const ProtectedContent = (props) => {
    const authRequest = {
        ...loginRequest
    };

    return (
        <MsalAuthenticationTemplate
            interactionType={InteractionType.Redirect}
            authenticationRequest={authRequest}
        >
            {props.children}
        </MsalAuthenticationTemplate>
    )
};
