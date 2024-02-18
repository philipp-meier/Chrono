import "./NotePage.less";
import {useEffect, useState} from "react";
import {Button, Card, Container, Icon} from "semantic-ui-react";
import {useMediaQuery} from "react-responsive";
import {GetMyNotesResponse, NotePreview} from "../../Entities/Note";

// Shared
import NoItemsMessage from "../../Shared/Components/NoItemsMessage";
import JSendApiClient, {API_ENDPOINTS} from "../../Shared/JSendApiClient";

const NotePage = () => {
  const isMobileOptimized = useMediaQuery({query: "(max-width:682px)"});
  const [notes, setNotes] = useState<NotePreview[]>([]);
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    const dataFetch = async () => {
      const response = await JSendApiClient.get<GetMyNotesResponse>(API_ENDPOINTS.Notes);
      setNotes(response?.notes ?? []);
      setIsLoaded(true);
    };
    dataFetch()
  }, []);

  const togglePinned = (note: NotePreview) => {
    JSendApiClient.update(`${API_ENDPOINTS.Notes}/${note.id}`, {
      id: note.id,
      title: note.title,
      isPinned: !note.isPinned
    }).then((isUpdated) => {
      if (isUpdated) {
        note.isPinned = !note.isPinned;
        setNotes([...notes]);
      }
    });
  };

  const orderedNotes = notes.sort((a, b) => {
    if (a.isPinned && !b.isPinned)
      return -1;
    else if (!a.isPinned && b.isPinned)
      return 1;

    const date1 = new Date(a.created);
    const date2 = new Date(b.created);
    return date2.getTime() - date1.getTime();
  });

  const noteItems = orderedNotes.map(n => {
    return (
      <Card key={n.id}>
        <Card.Content>
          <Card.Header className="note-header">
            <a href={`/notes/${n.id}`}>{n.title}</a>
            <Icon name="map pin" color={n.isPinned ? "blue" : "grey"} size="small" title={n.isPinned ? "Unpin" : "Pin"}
                  onClick={() => togglePinned(n)}/>
          </Card.Header>
          <Card.Meta>{new Date(n.created).toLocaleString(undefined, {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit"
          })}</Card.Meta>
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
      {noteItems.length > 0 && <Card.Group>{noteItems}</Card.Group>}
      {isLoaded && noteItems.length == 0 &&
          <NoItemsMessage
              text="You do not have any notes yet."
              buttonOptions={{text: "Add a note", href: "/notes/add"}}
          />
      }
    </Container>
  )
}

export default NotePage;
