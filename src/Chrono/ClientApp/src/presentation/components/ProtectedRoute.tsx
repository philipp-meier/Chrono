import {Component} from "react";
import {Navigate} from "react-router-dom";
import {getCurrentUserInfo} from "../../infrastructure/services/UserService";

interface ProtectedRouteProps {
  children: React.ReactElement;
  redirectPath?: string;
}

interface ProtectedRouteState {
  ready: boolean;
  authenticated: boolean;
}

class ProtectedRoute extends Component<
  ProtectedRouteProps,
  ProtectedRouteState
> {
  constructor(props: ProtectedRouteProps) {
    super(props);
    this.state = {ready: false, authenticated: false};
  }

  componentDidMount(): void {
    this.populateAuthenticationState();
  }

  render() {
    const {ready, authenticated} = this.state;
    if (!ready) {
      return <div></div>;
    } else {
      const children = this.props.children;
      const redirectPath = this.props.redirectPath ?? "/";
      return authenticated ? children : <Navigate to={redirectPath} replace/>;
    }
  }

  async populateAuthenticationState() {
    const userInfo = await getCurrentUserInfo();
    this.setState({ready: true, authenticated: userInfo.isAuthenticated});
  }
}

export default ProtectedRoute;
