import React, {useLayoutEffect, useState} from "react";
import {useMediaQuery} from "react-responsive";
import {Menu} from "semantic-ui-react";
import {useLocation, useNavigate} from "react-router-dom";
import MainMenuLg from "./MainMenuLg";
import MainMenuMd from "./MainMenuMd";
import {User} from "../../../Entities/User";
import MenuItems from "./MenuItems";
import JSendApiClient, {API_ENDPOINTS} from "../../JSendApiClient";

const MainMenu = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userInfo, setUserInfo] = useState<User | null>(null);

    useLayoutEffect(() => {
        const dataFetch = async () => {
            const userInfo = await JSendApiClient.get<User>(API_ENDPOINTS.User) ?? {isAuthenticated: false};
            setUserInfo(userInfo);
        };
        dataFetch();
    }, []);

    const pathname = location.pathname;
    const delimiterIndex = pathname.substring(1).indexOf("/");
    const currentLocation =
        pathname &&
        pathname.substring(
            1,
            delimiterIndex >= 0
                ? pathname.substring(1).indexOf("/") + 1
                : pathname.length
        );

    const [activeItem, setActiveItem] = useState(
        !currentLocation ? "home" : currentLocation
    );
    const handleItemClick = (
        _e: React.MouseEvent<HTMLAnchorElement, MouseEvent>,
        {name}: any
    ) => {
        setActiveItem(name);

        // Use lowercase paths
        const path = typeof name == "string" ? name.toLowerCase() : name;
        navigate(path === "home" ? "" : path);
    };

    const isActiveItem = (item: string) => {
        return item.toLowerCase() === activeItem.toLowerCase();
    };
    
    const renderLinks = () => {
        const isLoggedIn = userInfo?.isAuthenticated ?? false;
        return MenuItems.filter(
            (x) =>
                x.displayState === "always" ||
                (x.displayState === "loggedIn" && isLoggedIn) ||
                (x.displayState === "loggedOut" && !isLoggedIn)
        ).map((menuItem, index) => {
            const {name, disableClickHandler, position, element, clickHandler} = menuItem;
            return (
                <Menu.Item
                    key={index}
                    name={name}
                    active={isActiveItem(name)}
                    onClick={!disableClickHandler ? clickHandler || handleItemClick : undefined}
                    position={position}
                >
                    {element}
                </Menu.Item>
            );
        });
    };

    return (
        <div>
            {useMediaQuery({query: "(min-width:576px)"}) ? (
                <MainMenuLg renderLinks={renderLinks}/>
            ) : (
                <MainMenuMd renderLinks={renderLinks}/>
            )}
        </div>
    );
};

export default MainMenu;
