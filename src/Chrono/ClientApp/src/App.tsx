import "./App.css";
import "semantic-ui-css/semantic.min.css";
import PageLayout from "./presentation/components/PageLayout";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import ProtectedRoute from "./presentation/components/ProtectedRoute";
import AppRoutes from "./AppRoutes";

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<PageLayout/>}>
          {AppRoutes.map((route, index) => {
            const {element, requireAuth, ...rest} = route;
            return (
              <Route
                key={index}
                {...rest}
                element={
                  requireAuth ? (
                    <ProtectedRoute>{element}</ProtectedRoute>
                  ) : (
                    element
                  )
                }
              />
            );
          })}
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
