import {useState} from "react";
import {Image, Menu, Sidebar} from "semantic-ui-react";

const MainMenuMd = ({renderLinks}: any) => {
  const [visible, setVisible] = useState(false);
  const [icon, setIcon] = useState(HamIcon);

  const hideSidebar = () => {
    setIcon(HamIcon);
    setVisible(false);
  };
  const showSidebar = () => {
    setIcon(CloseIcon);
    setVisible(true);
  };
  const toggleSidebar = () => {
    visible ? hideSidebar() : showSidebar();
  };

  return (
    <>
      {visible && (
        <div
          style={{
            backgroundColor: "rgba(0, 0, 0, 0.795)",
            position: "fixed",
            height: "110vh",
            width: "100%",
            zIndex: 3,
          }}
          onClick={toggleSidebar}
        />
      )}
      <Menu inverted size="tiny" borderless attached>
        <Menu.Item>
          <Image
            size="tiny"
            src="/chrono.png"
            style={{margin: "0 auto"}}
            alt=""
          />
        </Menu.Item>
        <Menu.Menu position="right">
          <Menu.Item onClick={toggleSidebar}>{icon}</Menu.Item>
        </Menu.Menu>
      </Menu>
      <Sidebar
        as={Menu}
        animation="overlay"
        icon="labeled"
        inverted
        vertical
        visible={visible}
        width="thin"
        style={{zIndex: 4}}
      >
        {renderLinks()}
      </Sidebar>
    </>
  );
};

const HamIcon = () => {
  return <i className="big bars icon inverted" style={{zIndex: 4}}/>;
};
const CloseIcon = () => {
  return <i className="big close icon" style={{zIndex: 4}}/>;
};

export default MainMenuMd;
