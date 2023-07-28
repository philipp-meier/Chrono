import {MasterDataItem} from "./MasterDataListControl";
import {Button, Checkbox, Form, Input, Modal} from "semantic-ui-react";
import {useEffect, useState} from "react";
import {getTaskListOptions, updateTaskList} from "../../../infrastructure/services/TaskListService";

const TaskListEditModal = (props: {
  context?: MasterDataItem,
  onButtonClick?: (saved: boolean) => void;
}) => {
  const [title, setTitle] = useState(props.context?.name);
  const [requireDescription, setRequireDescription] = useState(true);
  const [requireBusinessValue, setRequireBusinessValue] = useState(true);
  const [dataLoaded, setDataLoaded] = useState(false);

  useEffect(() => {
    const dataFetch = async () => {
      const options = props.context ? await getTaskListOptions(props.context.id) : null;
      if (options) {
        setRequireBusinessValue(options.requireBusinessValue);
        setRequireDescription(options.requireDescription);
      }
      setDataLoaded(true);
    };
    dataFetch();
  }, [props]);

  return (
    <Modal open={dataLoaded}>
      <Modal.Header>Edit {title || 'item'}</Modal.Header>
      <Modal.Content>
        <Modal.Description>
          <Form>
            <Form.Field
              autoFocus
              control={Input}
              label="Name"
              placeholder="Name"
              value={title}
              onChange={(e: any) => {
                setTitle(e.target.value);
              }}
              required
            ></Form.Field>
            <Form.Field
              control={Checkbox}
              label="Require Business Value"
              checked={requireBusinessValue}
              onClick={() => {
                setRequireBusinessValue(!requireBusinessValue);
              }}
            />
            <Form.Field
              control={Checkbox}
              label="Require Description"
              checked={requireDescription}
              onClick={() => {
                setRequireDescription(!requireDescription);
              }}
            />
          </Form>
        </Modal.Description>
      </Modal.Content>
      <Modal.Actions>
        <Button
          onClick={() => {
            props.onButtonClick!(false);
          }}
        >
          Cancel
        </Button>
        <Button
          primary
          content="Save"
          onClick={() => {
            updateTaskList(props.context!.id, title || 'Item', {
              requireDescription,
              requireBusinessValue
            }).then((isDone) => {
              if (isDone) {
                props.onButtonClick!(true);
              }
            });
          }}
        />
      </Modal.Actions>
    </Modal>
  );
};

export default TaskListEditModal;
