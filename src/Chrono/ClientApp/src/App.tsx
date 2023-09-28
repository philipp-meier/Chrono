import "./App.css";
import AppRoutes from "./AppRoutes";
import "semantic-ui-css/semantic.min.css";
import {BrowserRouter, Route, Routes} from "react-router-dom";

// Shared
import ProtectedRoute from "./Shared/Components/ProtectedRoute";
import PageLayout from "./Shared/PageLayout";

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
