import "./TaskControl.css";
import {Button, Confirm, Container, Dropdown, Form, Icon, Input, TextArea,} from "semantic-ui-react";
import {useNavigate} from "react-router-dom";
import {useEffect, useState} from "react";
import CategoryEditControl from "../CategoryEditControl";
import {Category} from "../../../domain/models/Category";
import {createTask, deleteTask, getTask, updateTask,} from "../../../infrastructure/services/TaskService";
import {Task} from "../../../domain/models/Task";
import {getTaskList, getTaskListOptions} from "../../../infrastructure/services/TaskListService";
import {TaskListOptions} from "../../../domain/models/TaskListOptions";

enum TaskControlMode {
    Add,
    Edit,
}

const TaskControl = (props: {
    mode: TaskControlMode;
    id?: number;
    listId?: number;
}) => {
    const navigate = useNavigate();
    const [name, setName] = useState("");
    const [businessValue, setBusinessValue] = useState("");
    const [description, setDescription] = useState("");
    const [position, setPosition] = useState("1");
    const [categories, setCategories] = useState([] as Category[]);
    const [task, setTask] = useState(null as Task | null);
    const [taskListOptions, setTaskListOptions] = useState(null as TaskListOptions | null);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

    useEffect(() => {
        const dataFetch = async () => {
            const options = props.listId ? await getTaskListOptions(props.listId) : null;
            setTaskListOptions(options);

            if (props.mode === TaskControlMode.Edit && props.id) {
                const task = await getTask(props.id);
                if (task) {
                    setTask(task);
                    setName(task.name);
                    setBusinessValue(task.businessValue);
                    setDescription(task.description);
                    setPosition(task.position.toString());
                    setCategories(task.categories);
                } else {
                    // Task not found - redirect to add.
                    navigate(`/lists/${props.listId}/tasks/add`);
                }
            } else if (props.mode === TaskControlMode.Add && props.listId) {
                const taskList = await getTaskList(props.listId);
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
                    updateTask(task).then((isDone) => {
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

    const saveTask = () => {
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

            createTask(newTask).then((isUpdated) => {
                if (isUpdated) navigate(`/lists/${props.listId}`);
            });
        } else if (task) {
            task.name = name;
            task.businessValue = businessValue;
            task.description = description;
            task.position = parseInt(position);
            task.categories = categories;

            updateTask(task).then((isUpdated) => {
                if (isUpdated) navigate(`/lists/${task?.listId}`);
            });
        }
    };

    return (
        <>
            <Form style={{marginTop: "1em"}} onSubmit={saveTask}>
                <h1>{name}</h1>
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
                <Form.Field
                    control={TextArea}
                    label="Description"
                    placeholder="Description"
                    value={description}
                    onChange={(e: any) => {
                        setDescription(e.target.value);
                    }}
                    required={taskListOptions?.requireDescription}
                    disabled={task?.done}
                ></Form.Field>
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
                    <CategoryEditControl
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
                        {`Last modified by ${task.lastModifiedBy} on ${new Date(
                            task.lastModified!
                        ).toLocaleDateString()} ${new Date(
                            task.lastModified!
                        ).toLocaleTimeString()}.`}
                    </div>
                )}
                {!task?.done && (
                    <Form.Field>
                        <Button.Group primary>
                            <Button type="submit" disabled={task?.done}>
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
                    <Button as="a" href={`/lists/${props.listId}`}>
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
                    deleteTask(props.id!).then((isDeleted) => {
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

export {TaskControl, TaskControlMode};
