import { Container } from "semantic-ui-react";
import {
  TaskControlMode,
  TaskControl,
} from "../components/TaskControl/TaskControl";
import { useParams } from "react-router-dom";

const TaskPage = (props: { mode: TaskControlMode }) => {
  const { listId, id } = useParams();
  const taskId = id ? parseInt(id || "-1") : -1;
  const taskListId = listId ? parseInt(listId || "-1") : -1;
  return (
    <Container style={{ marginTop: "1em" }}>
      <TaskControl mode={props.mode} id={taskId} listId={taskListId} />
    </Container>
  );
};

export default TaskPage;
