import {useEffect, useState} from "react";
import {MasterDataItem} from "./MasterDataItem";
import {Button, Checkbox, Form, Input, Modal} from "semantic-ui-react";
import {UserSettings} from "../../Entities/User";
import {TaskListOptions} from "../../Entities/TaskListOptions";

// Shared
import ApiClient, {API_ENDPOINTS} from "../../Shared/ApiClient.ts";

/** Does only support TaskList MasterDataItems at the moment. */
const MasterDataItemEditModal = (props: {
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
        const userSettings = await ApiClient.get<UserSettings>(API_ENDPOINTS.UserSettings);
        setDefaultList(props.context.id === userSettings?.defaultTaskListId);
      }

      const options = props.context ?
        await ApiClient.get<TaskListOptions>(`${API_ENDPOINTS.TaskLists}/${props.context.id}/options`) :
        null;

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
            ApiClient.update(API_ENDPOINTS.UserSettings, {defaultTaskListId})
              .then(isDone => {
                if (isDone)
                  props.onButtonClick!(true);
              });
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
            ApiClient.update(`${API_ENDPOINTS.TaskLists}/${props.context!.id}`, {
              taskListId: props.context!.id,
              title: title || 'Item',
              requireBusinessValue: requireBusinessValue,
              requireDescription: requireDescription
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

export default MasterDataItemEditModal;
