import {Icon, Image} from "semantic-ui-react";
import {ThemeService} from "../../ThemeService.ts";

type MenuItemDefinition = {
  name: string;
  element?: JSX.Element;
  clickHandler?: () => void;
  disableClickHandler?: boolean;
  position?: "left" | "right";
  displayState: "always" | "loggedIn" | "loggedOut";
};

const MenuItems: MenuItemDefinition[] = [
  {
    name: "logo",
    element: (
      <Image
        size="tiny"
        src="/logo.png"
        style={{margin: "0 auto"}}
        alt=""
      />
    ),
    disableClickHandler: true,
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
    displayState: "loggedIn",
  },
  {
    name: "login",
    displayState: "loggedOut",
  },
  {
    name: "modeToggle",
    displayState: "always",
    position: "right",
    element: (
      <Icon id="modeIcon" name={ThemeService.isDarkModeEnabled() ? 'sun' : 'moon'} />
    ),
    clickHandler: () => {
      const modeIcon = document.getElementById('modeIcon')!;
      modeIcon.classList.toggle("sun");
      modeIcon.classList.toggle("moon");
      ThemeService.toggle()
    }
  },
];

export default MenuItems;
