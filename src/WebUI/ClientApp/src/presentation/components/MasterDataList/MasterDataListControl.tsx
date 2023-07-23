import { Button, Confirm, Container, List } from "semantic-ui-react";
import React, { useState } from "react";
import AddItemModal from "./AddItemModal";

export type MasterDataItem = {
  id: number;
  name: string;
};

const MasterDataListControl = (props: {
  items: MasterDataItem[];
  itemTitle: string;
  onAdd: (title: string) => void;
  onDelete: (item: MasterDataItem) => void;
}) => {
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [currentItem, setCurrentItem] = useState(null as MasterDataItem | null);

  const masterDataItems = props.items.map((x, idx) => (
    <List.Item key={idx}>
      <List.Content floated="right">
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
      <Container textAlign="right" style={{ marginBottom: "1em" }}>
        <AddItemModal itemTitle={props.itemTitle} onAdd={props.onAdd} />
      </Container>

      <Container>
        <List divided verticalAlign="middle">
          {masterDataItems}
        </List>
      </Container>
      <Confirm
        open={showDeleteConfirm}
        content={`Do you really want to delete the item \"${
          currentItem ? currentItem.name : "-"
        }\"?`}
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
