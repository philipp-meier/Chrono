import "./TaskListEditControl.less";
import TaskItemViewControl from "./TaskItemViewControl";
import {useEffect, useState} from "react";
import {useMediaQuery} from "react-responsive";
import {Button, Checkbox, Container, Dropdown, Icon} from "semantic-ui-react";
import type {Task} from "../../Entities/Task";
import type {TaskList, TaskListBrief} from "../../Entities/TaskList";
import type {Category} from "../../Entities/Category";
import {UserSettings} from "../../Entities/User";
import {Link} from "react-router-dom";

// Shared
import NoItemsMessage from "../../Shared/Components/NoItemsMessage";
import ApiClient, {API_ENDPOINTS} from "../../Shared/ApiClient.ts";

const TaskListEditControl = (props: { taskListId: number }) => {
  const [taskList, setTaskList] = useState<TaskList | null>(null);
  const [category, setCategory] = useState("");
  const [doneFilter, setDoneFilter] = useState(false);
  const [availableTaskLists, setAvailableTaskLists] = useState<TaskListBrief[]>([]);
  const [availableCategories, setAvailableCategories] = useState<Category[]>([]);
  const [isLoaded, setIsLoaded] = useState(false);
  const isMobileOptimized = useMediaQuery({query: "(max-width:682px)"});

  useEffect(() => {
    const dataFetch = async () => {
      const taskLists = (await ApiClient.get<TaskListBrief[]>(API_ENDPOINTS.TaskLists) ?? [])
        .sort((a, b) => a.title.localeCompare(b.title))

      const categories = (await ApiClient.get<Category[]>(API_ENDPOINTS.Categories) ?? [])
        .sort((a, b) => a.name.localeCompare(b.name))

      setAvailableTaskLists(taskLists ?? []);
      setAvailableCategories(categories ?? []);

      if (!taskLists || taskLists.length === 0) {
        setIsLoaded(true);
        return;
      }

      const userSettings = await ApiClient.get<UserSettings>(API_ENDPOINTS.UserSettings);

      const selectedTaskList = props.taskListId >= 0 ?
        props.taskListId : userSettings?.defaultTaskListId;

      const taskList = await ApiClient.get<TaskList>(`${API_ENDPOINTS.TaskLists}/${selectedTaskList ?? taskLists[0].id}`);
      setTaskList(taskList);
      setIsLoaded(true);
    };

    dataFetch()
  }, [props.taskListId]);

  const filteredTasks = (taskList?.tasks || [])
    .filter((x) => x.done === doneFilter)
    .filter(
      (x) => !category || x.categories.map((x) => x.name).indexOf(category) >= 0
    );

  const [, setTasks] = useState(filteredTasks);
  const handleTaskMove = (task: Task, direction: number) => {
    ApiClient.update(`${API_ENDPOINTS.Tasks}/${task.id}`, {
      id: task.id,
      position: task.position + direction,
      name: task.name,
      businessValue: task.businessValue,
      description: task.description,
      done: task.done,
      categories: task.categories,
    }).then((isUpdated) => {
      if (!isUpdated || Math.abs(direction) !== 1)
        return;

      const curIndex = filteredTasks.indexOf(task);
      const isFirstPosMovingUp = curIndex === 0 && direction < 0;
      const isLastPosMovingDown = curIndex === filteredTasks.length - 1 && direction > 0;

      if (isFirstPosMovingUp || isLastPosMovingDown)
        return;

      filteredTasks[curIndex].position += direction;
      filteredTasks[curIndex + direction].position -= direction;

      setTasks([...filteredTasks]);
    });
  };

  const taskListItems = filteredTasks
    .sort((a, b) => a.position - b.position)
    .map((task: Task) => (
      <TaskItemViewControl
        key={task.id}
        task={task}
        moveUp={
          filteredTasks.indexOf(task) !== 0
            ? () => handleTaskMove(task, -1)
            : undefined
        }
        moveDown={
          filteredTasks.indexOf(task) !== filteredTasks.length - 1
            ? () => handleTaskMove(task, +1)
            : undefined
        }
      />
    ));

  return (
    <Container className={isMobileOptimized ? "tasklist mobile" : "tasklist"}>
      <Container
        textAlign="right"
        style={{marginTop: "1em"}}
        className="tasklist-menu"
      >
        <div
          className={!isMobileOptimized ? "filters" : "filters ui form mini"}
        >
          {!isMobileOptimized && (
            <Checkbox
              label="Done"
              checked={doneFilter}
              toggle
              style={{marginRight: "0.5em"}}
              onClick={() => {
                setDoneFilter(!doneFilter);
              }}
            />
          )}
          <Dropdown
            placeholder="Categories"
            search={!isMobileOptimized}
            clearable
            selection
            options={availableCategories.map((x) => {
              return {key: x.name, text: x.name, value: x.name};
            })}
            style={{marginRight: "0.5em"}}
            onChange={(e: any) => {
              setCategory(e.target.innerText);
            }}
          />
          <Dropdown
            placeholder={taskList ? taskList.title : "List"}
            search={!isMobileOptimized}
            selection
            options={availableTaskLists.map((x) => {
              return {key: x.id, text: x.title, value: x.title};
            })}
            style={{marginRight: "0.5em"}}
            defaultValue={taskList ? taskList.title : undefined}
            onChange={async (e: any) => {
              const selectedId = availableTaskLists.find(
                (x) => x.title === e.target.innerText
              )?.id;
              if (selectedId && selectedId !== taskList?.id) {
                setTaskList(await ApiClient.get<TaskList>(`${API_ENDPOINTS.TaskLists}/${selectedId}`));
              }
            }}
          />
          {isMobileOptimized && (
            <Checkbox
              label="Done"
              checked={doneFilter}
              toggle
              style={{marginRight: "0.5em", marginTop: "0.5em"}}
              onClick={() => {
                setDoneFilter(!doneFilter);
              }}
              className="ui form mini"
            />
          )}
        </div>
        <div className="buttons">
          <Button
            as={Link}
            to={`/lists/${taskList?.id}/tasks/add`}
            circular={isMobileOptimized}
            size={isMobileOptimized ? "huge" : undefined}
            icon={isMobileOptimized}
            disabled={!taskList}
            primary
          >
            {isMobileOptimized ? <Icon name="add"/> : "Add Task"}
          </Button>
        </div>
      </Container>
      {taskListItems.length > 0 &&
          <Container className="ui divided tasklist-items items list">{taskListItems}</Container>}
      {isLoaded && !taskList &&
          <NoItemsMessage
              text={"Create your first task list in the \"Master Data\" menu."}
              buttonOptions={{text: "Open Master Data", href: "/masterdata"}}
          />
      }
      {isLoaded && taskList && taskListItems.length == 0 &&
          <NoItemsMessage
              text={`The list "${taskList.title}" does not contain any ${doneFilter ? 'completed' : 'open'} tasks at the moment.`}
              buttonOptions={!doneFilter ? {text: "Add a task", href: `/lists/${taskList?.id}/tasks/add`} : undefined}
          />
      }
    </Container>
  );
};

export default TaskListEditControl;
