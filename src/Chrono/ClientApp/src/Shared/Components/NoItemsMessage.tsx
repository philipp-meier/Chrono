import {Button, Header, Icon, Segment} from "semantic-ui-react";

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
              as="a"
              href={props.buttonOptions.href}>
            {props.buttonOptions.text}
          </Button>
      }
    </Segment>
  );
}

export default NoItemsMessage;
