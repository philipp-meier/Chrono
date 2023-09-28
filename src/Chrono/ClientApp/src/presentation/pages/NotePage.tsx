import {Container} from "semantic-ui-react";
import {useParams} from "react-router-dom";
import {NoteControl, NoteControlMode} from "../components/Notes/NoteControl.tsx";

const NotePage = (props: { mode: NoteControlMode }) => {
  const {id} = useParams();
  const noteId = id ? parseInt(id || "-1") : -1;
  return (
    <Container style={{marginTop: "1em"}}>
      <NoteControl mode={props.mode} id={noteId}/>
    </Container>
  );
};

export default NotePage;
