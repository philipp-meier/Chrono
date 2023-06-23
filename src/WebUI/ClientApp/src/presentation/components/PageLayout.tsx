import { Outlet } from "react-router-dom";
import MainMenu from "./MainMenu/MainMenu";

const PageLayout = () => {
  return (
    <>
      <MainMenu />
      <Outlet />
    </>
  );
};

export default PageLayout;
