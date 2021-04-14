import React, { Component, Fragment } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import authService from './AuthorizeService';
import { ApplicationPaths } from './ApiAuthorizationConstants';

export class LoginMenu extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isAuthenticated: false,
            userName: null
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    async populateState() {
        const [isAuthenticated, user, isAdmin] = await Promise.all([authService.isAuthenticated(), authService.getUser(), authService.isAdmin()])
        this.setState({
            isAuthenticated,
            userName: user && user.name,
            isAdmin
        });
    }

    render() {
        const { isAuthenticated, userName, isAdmin } = this.state;
        const registerPath = `${ApplicationPaths.Register}`;
        if (!isAuthenticated) {
            const loginPath = `${ApplicationPaths.Login}`;
            return this.anonymousView(registerPath, loginPath);
        } else {
            const profilePath = `${ApplicationPaths.Profile}`;
            const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
            return this.authenticatedView(userName, profilePath, logoutPath, registerPath, isAdmin);
        }
    }

    authenticatedView(userName, profilePath, logoutPath, registerPath, isAdmin) {
        return (<Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to={profilePath}>Hello {userName}</NavLink>
            </NavItem>
            {isAdmin ?
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to={registerPath}>Lisää käyttäjä</NavLink>
                </NavItem> : <></>
            }
            <NavItem>
                <NavLink tag={Link} className="text-dark" to={logoutPath}>Kirjaudu ulos</NavLink>
            </NavItem>
        </Fragment>);

    }

    anonymousView(registerPath, loginPath) {
        return (<Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to={loginPath}>Login</NavLink>
            </NavItem>
        </Fragment>);
    }
}
