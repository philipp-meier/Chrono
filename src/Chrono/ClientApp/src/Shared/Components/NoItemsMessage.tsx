import {Button, Header, Icon, Segment} from "semantic-ui-react";
import {Link} from "react-router-dom";

type NoItemsMessageButton = {
  text: string;
  href: string;
}

const NoItemsMessage = (props: {
  text: string;
  buttonOptions?: NoItemsMessageButton;
}) => {
  return (
    <Segment placeholder>
      <Header icon>
        <Icon name='lightbulb outline'/>
        {props.text}
      </Header>
      {props.buttonOptions &&
          <Button
              primary
              as={Link}
              to={props.buttonOptions.href}>
            {props.buttonOptions.text}
          </Button>
      }
    </Segment>
  );
}

export default NoItemsMessage;
