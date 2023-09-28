import {Outlet} from "react-router-dom";
import MainMenu from "./Components/MainMenu/MainMenu";

const PageLayout = () => {
  return (
    <>
      <MainMenu/>
      <Outlet/>
    </>
  );
};

export default PageLayout;
