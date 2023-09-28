import "./NotePage.css";
import {useEffect, useState} from "react";
import {Button, Card, Container, Icon} from "semantic-ui-react";
import {useMediaQuery} from "react-responsive";
import {getMyNotes} from "./NoteService";

// Shared
import {NotePreview} from "../../Shared/Entities/Note";

const NotePage = () => {
  const isMobileOptimized = useMediaQuery({query: "(max-width:682px)"});
  const [notes, setNotes] = useState([] as NotePreview[]);

  useEffect(() => {
    const dataFetch = async () => {
      const response = await getMyNotes();
      setNotes(response.notes);
    };
    dataFetch()
  }, []);

  const noteItems = notes.map(n => {
    return (
      <Card key={n.id}>
        <Card.Content>
          <Card.Header>
            <a href={`/notes/${n.id}`}>{n.title}</a>
          </Card.Header>
          <Card.Meta>{n.lastModified + ""}</Card.Meta>
          <Card.Description>
            {n.preview}
          </Card.Description>
        </Card.Content>
      </Card>
    );
  })

  return (
    <Container className={isMobileOptimized ? "note-container mobile" : "note-container"}>
      <Container
        textAlign="right"
        style={{marginBottom: "1em"}}
        className="note-menu"
      >
        <div className="buttons">
          <Button
            as="a"
            href={`/notes/add`}
            circular={isMobileOptimized}
            size={isMobileOptimized ? "huge" : undefined}
            icon={isMobileOptimized}
            primary
          >
            {isMobileOptimized ? <Icon name="add"/> : "Add Note"}
          </Button>
        </div>
      </Container>
      <Card.Group>{noteItems}</Card.Group>
    </Container>
  );
};

export default NotePage;
