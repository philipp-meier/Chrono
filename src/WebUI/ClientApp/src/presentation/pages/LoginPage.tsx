import { Component } from "react";

interface ILoginProps {
  sign: string;
}

class LoginPage extends Component<ILoginProps> {
  private readonly _sign: string;

  constructor(props: ILoginProps) {
    super(props);
    this._sign = props.sign;
  }

  componentDidMount() {
    window.location.href = `/api/login?redirectUrl=${encodeURI(
      window.location.protocol + "//" + window.location.host
    )}&sign=${this._sign}`;
  }

  render() {
    return <>{this._sign === "in" ? "Logging in..." : "Logging out"}</>;
  }
}

export default LoginPage;
