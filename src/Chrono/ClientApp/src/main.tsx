import ReactDOM from "react-dom/client";
import "./index.less";
import App from "./App";
import {ThemeService} from "./Shared/ThemeService.ts";

ThemeService.init();

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
root.render(<App/>);