import {Menu, Segment} from "semantic-ui-react";

const MainMenuLg = ({renderLinks}: any) => {
  return (
    <Segment inverted attached size="mini">
      <Menu inverted secondary>
        {renderLinks()}
      </Menu>
    </Segment>
  );
};
export default MainMenuLg;
