import {Image} from "semantic-ui-react";
import {ThemeService} from "../../ThemeService.ts";

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
      <a onClick={() => ThemeService.toggle()}>
        <Image
          size="tiny"
          src="/logo.png"
          style={{margin: "0 auto"}}
          alt=""
        />
      </a>
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
