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

    const noteItems = notes.map(n => {
        return (
            <Card key={n.id}>
                <Card.Content>
                    <Card.Header>
                        <a href={`/notes/${n.id}`}>{n.title}</a>
                    </Card.Header>
                    <Card.Meta>{n.created}</Card.Meta>
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
