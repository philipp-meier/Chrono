import {Button, Confirm, Container, List} from "semantic-ui-react";
import React, {ReactElement, useState} from "react";

export type MasterDataItem = {
  id: number;
  name: string;
};

const MasterDataListControl = (props: {
  items: MasterDataItem[];
  itemTitle: string;
  addModal: ReactElement;
  editModal?: ReactElement;
  onDelete: (item: MasterDataItem) => void;
  refreshDataCallback?: () => void;
}) => {
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [currentItem, setCurrentItem] = useState(null as MasterDataItem | null);

  const masterDataItems = props.items.map((x, idx) => (
    <List.Item key={idx}>
      <List.Content floated="right">
        {props.editModal && <Button
            onClick={() => {
              setCurrentItem(x);
              setShowEditModal(true);
            }}
        >
            Edit
        </Button>}
        <Button
          onClick={() => {
            setCurrentItem(x);
            setShowDeleteConfirm(true);
          }}
        >
          Delete
        </Button>
      </List.Content>
      <List.Content>{x.name}</List.Content>
    </List.Item>
  ));

  return (
    <>
      <Container textAlign="right" style={{marginBottom: "1em"}}>
        {props.addModal}
      </Container>

      <Container>
        <List divided verticalAlign="middle">
          {masterDataItems}
        </List>
      </Container>
      {/* Is there a better way to inject custom (edit) modals with properties? */}
      {showEditModal && React.cloneElement(props.editModal!, {
        context: currentItem,
        onButtonClick: (saved: boolean) => {
          if (saved)
            props.refreshDataCallback!();

          setShowEditModal(!showEditModal);
        }
      })}
      <Confirm
        open={showDeleteConfirm}
        content={`Do you really want to delete the item "${
          currentItem ? currentItem.name : "-"
        }"?`}
        onCancel={() => setShowDeleteConfirm(false)}
        onConfirm={() => {
          props.onDelete(currentItem!);
          setShowDeleteConfirm(false);
        }}
      />
    </>
  );
};

export default MasterDataListControl;
