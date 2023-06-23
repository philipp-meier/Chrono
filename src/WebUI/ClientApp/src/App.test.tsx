import { render, screen } from "@testing-library/react";
import App from "./App";

test("renders learn react link", () => {
  render(<App />);
  const homeMenu = screen.getByText(/Home/i);
  expect(homeMenu).toBeInTheDocument();
});
