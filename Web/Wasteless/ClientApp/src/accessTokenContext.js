import React, {useContext, createContext, useState} from 'react'

const AccessTokenProvider = (props) => {
    const [accessToken, setAccessToken] = useState(null)
    return <AccessToken.Provider value={[accessToken, setAccessToken]} {...props} />
}

const AccessToken = createContext(null)

const useAccessToken = () => useContext(AccessToken)

export {AccessTokenProvider, useAccessToken}