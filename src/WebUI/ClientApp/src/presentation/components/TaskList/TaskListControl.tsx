import "./TaskListControl.css";
import TaskListItem from "./TaskListItem";
import type {Task} from "../../../domain/models/Task";
import type {TaskListBrief} from "../../../domain/models/TaskListBrief";
import type {TaskList} from "../../../domain/models/TaskList";
import type {Category} from "../../../domain/models/Category";
import {useEffect, useState} from "react";
import {Button, Checkbox, Container, Dropdown, Icon} from "semantic-ui-react";
import {getTaskList, getTaskLists,} from "../../../infrastructure/services/TaskListService";
import {getCategories} from "../../../infrastructure/services/CategoryService";
import {updateTask} from "../../../infrastructure/services/TaskService";
import {useMediaQuery} from "react-responsive";
import {getCurrentUserSettings} from "../../../infrastructure/services/UserService.ts";

const TaskListControl = (props: { taskListId?: number }) => {
  const [taskList, setTaskList] = useState(null as TaskList | null);
  const [category, setCategory] = useState("");
  const [doneFilter, setDoneFilter] = useState(false);
  const [availableTaskLists, setAvailableTaskLists] = useState(
    [] as TaskListBrief[]
  );
  const [availableCategories, setAvailableCategories] = useState(
    [] as Category[]
  );
  const isMobileOptimized = useMediaQuery({query: "(max-width:682px)"});

  useEffect(() => {
    const dataFetch = async () => {
      const taskLists = await getTaskLists();
      setAvailableTaskLists(taskLists);

      const categories = await getCategories();
      setAvailableCategories(categories);

      if (!taskLists || taskLists.length === 0)
        return;

      const userSettings = await getCurrentUserSettings();
      const selectedTaskList = props.taskListId >= 0 ?
        props.taskListId : userSettings.defaultTaskListId;

      const taskList = await getTaskList(selectedTaskList ?? taskLists[0].id);
      setTaskList(taskList);
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
    updateTask(task, task.position + direction).then((isUpdated) => {
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
      <TaskListItem
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
                setTaskList(await getTaskList(selectedId));
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
            as="a"
            href={`/lists/${taskList?.id}/tasks/add`}
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
      <Container className="ui divided items list">{taskListItems}</Container>
    </Container>
  );
};

export default TaskListControl;
