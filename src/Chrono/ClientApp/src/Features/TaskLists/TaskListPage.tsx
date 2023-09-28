import {useParams} from "react-router-dom";
import {default as TaskListControl} from "./TaskListEditControl";

const TaskListPage = () => {
  const {listId} = useParams();
  const taskListId = listId ? parseInt(listId || "-1") : -1;
  return <TaskListControl taskListId={taskListId}/>;
};

export default TaskListPage;
