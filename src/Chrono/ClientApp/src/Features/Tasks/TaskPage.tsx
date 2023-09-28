import {useParams} from "react-router-dom";
import {Container} from "semantic-ui-react";
import {TaskControlMode, TaskEditControl,} from "./TaskEditControl";

const TaskPage = (props: { mode: TaskControlMode }) => {
  const {listId, id} = useParams();
  const taskId = id ? parseInt(id || "-1") : -1;
  const taskListId = listId ? parseInt(listId || "-1") : -1;
  return (
    <Container style={{marginTop: "1em"}}>
      <TaskEditControl mode={props.mode} id={taskId} listId={taskListId}/>
    </Container>
  );
};

export default TaskPage;
