import {Component} from "react";

interface ILoginProps {
  sign: string;
}

class LoginPage extends Component<ILoginProps, any> {
  private readonly _sign: string;

  constructor(props: ILoginProps) {
    super(props);
    this._sign = props.sign;
  }

  componentDidMount() {
    window.location.href = `/api/login?sign=${this._sign}`;
  }

  render() {
    return <>{this._sign === "in" ? "Logging in..." : "Logging out"}</>;
  }
}

export default LoginPage;
