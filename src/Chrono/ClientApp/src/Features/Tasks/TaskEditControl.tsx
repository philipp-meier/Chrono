import "./TaskEditControl.less";
import {useEffect, useState} from "react";
import {Link, useNavigate} from "react-router-dom";
import {Button, Confirm, Container, Dropdown, Form, Icon, Input} from "semantic-ui-react";
import TaskCategoryEditControl from "./TaskCategoryEditControl";
import {Task} from "../../Entities/Task";
import {Category} from "../../Entities/Category";
import {TaskList} from "../../Entities/TaskList";
import {TaskListOptions} from "../../Entities/TaskListOptions";

// Shared
import {MarkdownEditor} from "../../Shared/Components/MarkdownEditor/MarkdownEditor";
import JSendApiClient, {API_ENDPOINTS} from "../../Shared/JSendApiClient";
import DateUtil from "../../Shared/DateUtil.ts";

enum TaskControlMode {
  Add,
  Edit,
}

const TaskEditControl = (props: {
  mode: TaskControlMode;
  id?: number;
  listId?: number;
}) => {
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [businessValue, setBusinessValue] = useState("");
  const [description, setDescription] = useState("");
  const [position, setPosition] = useState("1");
  const [categories, setCategories] = useState<Category[]>([]);
  const [task, setTask] = useState<Task | null>(null);
  const [taskListOptions, setTaskListOptions] = useState<TaskListOptions | null>(null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [lastModifiedText, setLastModifiedText] = useState(task?.lastModified);

  useEffect(() => {
    const dataFetch = async () => {
      const options = props.listId ?
        await JSendApiClient.get<TaskListOptions>(`${API_ENDPOINTS.TaskLists}/${props.listId}/options`) :
        null;

      setTaskListOptions(options);

      if (props.mode === TaskControlMode.Edit && props.id) {
        const task = await JSendApiClient.get<Task>(`${API_ENDPOINTS.Tasks}/${props.id}`);
        if (task) {
          setTask(task);
          setName(task.name);
          setBusinessValue(task.businessValue);
          setDescription(task.description);
          setPosition(task.position.toString());
          setCategories(task.categories);
          setLastModifiedText(task.lastModified);
        } else {
          // Task not found - redirect to add.
          navigate(`/lists/${props.listId}/tasks/add`);
        }
      } else if (props.mode === TaskControlMode.Add && props.listId) {
        const taskList = await JSendApiClient.get<TaskList>(`${API_ENDPOINTS.TaskLists}/${props.listId}`);
        if (taskList) {
          const maxPosition = Math.max(
            0,
            ...taskList.tasks.filter((x) => !x.done).map((x) => x.position)
          );
          setPosition((maxPosition + 1).toString());
        }
      }
    };
    dataFetch();
  }, [props, navigate]);

  if (props.mode === TaskControlMode.Edit && (!props.id || props.id < 0))
    return <Container>Not found</Container>;

  const buttonOptions = [
    {
      key: "saveAndClose",
      icon: "save",
      text: "Save & Close",
      value: "saveAndClose",
      onClick: () => saveTask(true),
    },
    {
      key: "delete",
      icon: "delete",
      text: "Delete task",
      value: "delete",
      onClick: () => setShowDeleteConfirm(true),
    },
  ];
  if (task && !task.done) {
    buttonOptions.push({
      key: "done",
      icon: "check",
      text: "Done",
      value: "done",
      onClick: () => {
        if (task && !task.done) {
          task.done = true;
          JSendApiClient.update(`${API_ENDPOINTS.Tasks}/${task.id}`, task).then((isDone) => {
            if (isDone) {
              navigate(`/lists/${task.listId}`);
            } else {
              task.done = false;
            }
          });
        }
      },
    });
  }

  const saveTask = (closeOnSave: boolean = false) => {
    const mode = props.mode;

    if (mode === TaskControlMode.Add) {
      const newTask: Task = {
        id: -1,
        listId: props.listId!,
        position: parseInt(position),
        name: name,
        businessValue: businessValue,
        description: description,
        done: false,
        categories: categories,
      };

      JSendApiClient.create(API_ENDPOINTS.Tasks, newTask).then((id) => {
        if (id !== -1) {
          if (closeOnSave)
            navigate(`/lists/${props.listId}`);
          else
            navigate(`/lists/${props.listId}/tasks/${id}`);
        }
      });
    } else if (task) {
      task.name = name;
      task.businessValue = businessValue;
      task.description = description;
      task.position = parseInt(position);
      task.categories = categories;

      JSendApiClient.update(`${API_ENDPOINTS.Tasks}/${task.id}`, task).then((isUpdated) => {
        if (isUpdated) {
          if (!closeOnSave) {
            setLastModifiedText(new Date().toISOString());
          } else {
            navigate(`/lists/${task?.listId}`);
          }
        }
      });
    }
  };

  return (
    <>
      <Form style={{marginTop: "1em"}}>
        <h1>{name ? name : 'New task'}</h1>
        <Form.Field
          control={Input}
          label="Name"
          placeholder="Name"
          value={name}
          onChange={(e: any) => {
            setName(e.target.value);
          }}
          required
          disabled={task?.done}
        ></Form.Field>
        <Form.Field
          control={Input}
          label="Business value"
          placeholder="Business value"
          value={businessValue}
          onChange={(e: any) => {
            setBusinessValue(e.target.value);
          }}
          required={taskListOptions?.requireBusinessValue}
          disabled={task?.done}
        ></Form.Field>
        <MarkdownEditor
          textLabel="Description"
          text={description}
          textAreaRows={10}
          onTextChanged={(e: any) => setDescription(e.target.value)}
          required={taskListOptions?.requireDescription}
          disabled={task?.done}/>
        <Form.Field
          label="Position"
          control={Input}
          type="number"
          min={1}
          value={position}
          onChange={(e: any) => {
            setPosition(e.target.value);
          }}
          required
          disabled={task?.done}
        />
        <Form.Field disabled={task?.done} className="categories">
          <TaskCategoryEditControl
            currentCategories={categories}
            onAdd={(category: Category) => {
              setCategories([...categories, category]);
            }}
            onDelete={(category: Category) => {
              const index = categories
                .map((x) => x.name)
                .indexOf(category.name);
              if (index >= 0) {
                categories.splice(index, 1);
                setCategories([...categories]);
              }
            }}
            disabled={task?.done}
          />
        </Form.Field>
        {task?.lastModifiedBy && (
          <div style={{color: "gray", marginBottom: "0.75em"}}>
            {`Last modified by ${task.lastModifiedBy} on ${DateUtil.formatDateFromString(lastModifiedText)}.`}
          </div>
        )}
        {!task?.done && (
          <Form.Field>
            <Button.Group primary>
              <Button onClick={() => saveTask(false)} disabled={task?.done}>
                <Icon name="save"/>
                Save
              </Button>
              {props.mode !== TaskControlMode.Add && (
                <Dropdown
                  className="button icon"
                  floating
                  options={buttonOptions}
                  trigger={<></>}
                  disabled={task?.done}
                />
              )}
            </Button.Group>
          </Form.Field>
        )}
        <Form.Field>
          <Button as={Link} to={`/lists/${props.listId}`}>
            <Icon name="list ul"/>
            Back to the list
          </Button>
        </Form.Field>
      </Form>
      <Confirm
        open={showDeleteConfirm}
        content={`Do you really want to delete the task "${name}"?`}
        onCancel={() => setShowDeleteConfirm(false)}
        onConfirm={() => {
          JSendApiClient.delete(`${API_ENDPOINTS.Tasks}/${props.id!}`).then((isDeleted) => {
            if (isDeleted) {
              setShowDeleteConfirm(false);
              navigate(`/lists/${props.listId}`);
            }
          });
        }}
      />
    </>
  );
};

export {TaskEditControl, TaskControlMode};
