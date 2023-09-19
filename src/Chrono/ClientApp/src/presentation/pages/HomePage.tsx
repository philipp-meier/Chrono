import {Container, Image} from "semantic-ui-react";

const HomePage = () => {
  return (
    <Container text style={{marginTop: "7em"}} textAlign="center">
      <Image
        size="small"
        src="/chrono.png"
        style={{marginRight: "1.5em", marginBottom: "0.5em"}}
        verticalAlign="middle"
      />
      <p>Work on what matters.</p>
    </Container>
  );
};

export default HomePage;
