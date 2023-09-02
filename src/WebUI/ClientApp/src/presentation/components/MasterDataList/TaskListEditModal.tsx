import {MasterDataItem} from "./MasterDataListControl";
import {Button, Checkbox, Form, Input, Modal} from "semantic-ui-react";
import {useEffect, useState} from "react";
import {getTaskListOptions, updateTaskList} from "../../../infrastructure/services/TaskListService";
import {getCurrentUserSettings, updateCurrentUserSettings} from "../../../infrastructure/services/UserService.ts";

const TaskListEditModal = (props: {
  context?: MasterDataItem,
  onButtonClick?: (saved: boolean) => void;
}) => {
  const [title, setTitle] = useState(props.context?.name);
  const [requireDescription, setRequireDescription] = useState(true);
  const [requireBusinessValue, setRequireBusinessValue] = useState(true);
  const [dataLoaded, setDataLoaded] = useState(false);
  const [isDefaultList, setDefaultList] = useState(true);

  useEffect(() => {
    const dataFetch = async () => {

      if (props.context) {
        const userSettings = await getCurrentUserSettings();
        setDefaultList(props.context.id === userSettings.defaultTaskListId);
      }

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
        {<Button
          color="green"
          onClick={() => {
            const defaultTaskListId = isDefaultList ? undefined : props.context!.id;
            updateCurrentUserSettings({defaultTaskListId})
              .then(isDone => props.onButtonClick!(true));
          }}
        >
          {isDefaultList ? "Unset default" : "Set as default"}
        </Button>}
        <Button
          onClick={() => props.onButtonClick!(false)}
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
