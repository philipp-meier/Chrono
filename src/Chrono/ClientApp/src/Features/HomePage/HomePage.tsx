import "./HomePage.less"
import {Container, Image} from "semantic-ui-react";

const HomePage = () => {
  return (
    <Container className="chrono-container" text textAlign="center">
      <div className="headline">
        <Image
          size="small"
          src="/chrono.png"
          verticalAlign="middle"
        />
        <span className="version-number">v1.4.0</span>
      </div>
      <p>Work on what matters.</p>
    </Container>
  );
};

export default HomePage;
