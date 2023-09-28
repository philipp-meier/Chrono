import {Image} from "semantic-ui-react";

type MenuItemDefinition = {
  name: string;
  element?: JSX.Element;
  disableNavigation?: boolean;
  position?: "left" | "right";
  displayState: "always" | "loggedIn" | "loggedOut";
};

const MenuItems: MenuItemDefinition[] = [
  {
    name: "logo",
    element: (
      <Image
        size="tiny"
        src="/chrono.png"
        style={{margin: "0 auto"}}
        alt=""
      />
    ),
    disableNavigation: true,
    displayState: "always",
  },
  {
    name: "home",
    displayState: "always",
  },
  {
    name: "lists",
    displayState: "loggedIn",
  },
  {
    name: "notes",
    displayState: "loggedIn",
  },
  {
    name: "masterData",
    displayState: "loggedIn",
  },
  {
    name: "logout",
    position: "right",
    displayState: "loggedIn",
  },
  {
    name: "login",
    position: "right",
    displayState: "loggedOut",
  },
];

export default MenuItems;
