import {
  Button,
  Confirm,
  Container,
  Form,
  Header,
  Input,
  List,
  Modal,
} from "semantic-ui-react";
import React, { useState } from "react";

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
  const [open, setOpen] = useState(false);
  const [newItemName, setNewItemName] = useState("");

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
        <Modal
          onClose={() => setOpen(false)}
          onOpen={() => setOpen(true)}
          open={open}
          trigger={<Button primary>Add {props.itemTitle}</Button>}
        >
          <Modal.Header>Add {props.itemTitle}</Modal.Header>
          <Modal.Content>
            <Modal.Description>
              <Form>
                <Form.Field
                  control={Input}
                  label="Name"
                  placeholder="Name"
                  value={newItemName}
                  onChange={(e: any) => {
                    setNewItemName(e.target.value);
                  }}
                  required
                ></Form.Field>
              </Form>
            </Modal.Description>
          </Modal.Content>
          <Modal.Actions>
            <Button
              onClick={() => {
                setOpen(false);
                setNewItemName("");
              }}
            >
              Cancel
            </Button>
            <Button
              primary
              content="Add"
              onClick={() => {
                props.onAdd(newItemName);
                setOpen(false);
                setNewItemName("");
              }}
            />
          </Modal.Actions>
        </Modal>
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
