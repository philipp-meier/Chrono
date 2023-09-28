import {Container} from "semantic-ui-react";
import {useParams} from "react-router-dom";
import {NoteEditControl, NoteEditControlMode} from "./NoteEditControl";

const NoteEditPage = (props: { mode: NoteEditControlMode }) => {
  const {id} = useParams();
  const noteId = id ? parseInt(id || "-1") : -1;
  return (
    <Container style={{marginTop: "1em"}}>
      <NoteEditControl mode={props.mode} id={noteId}/>
    </Container>
  );
};

export default NoteEditPage;
