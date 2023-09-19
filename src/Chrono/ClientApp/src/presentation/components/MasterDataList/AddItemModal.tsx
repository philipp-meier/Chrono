import {Button, Form, Input, Modal} from "semantic-ui-react";
import {useState} from "react";

const AddItemModal = (props: {
  itemTitle: string;
  onAdd: (title: string) => void;
}) => {
  const [open, setOpen] = useState(false);
  const [newItemName, setNewItemName] = useState("");

  return (
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
              autoFocus
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
  );
};

export default AddItemModal;
