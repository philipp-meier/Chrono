import {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {Button, Confirm, Container, Dropdown, Form, Icon, Input} from "semantic-ui-react";
import {Note} from "../../Entities/Note";

// Shared
import {MarkdownEditor} from "../../Shared/Components/MarkdownEditor/MarkdownEditor";
import JSendApiClient, {API_ENDPOINTS} from "../../Shared/JSendApiClient";
import DateUtil from "../../Shared/DateUtil.ts";

enum NoteEditControlMode {
  Add,
  Edit,
}

const NoteEditControl = (props: {
  mode: NoteEditControlMode;
  id?: number;
}) => {
  const navigate = useNavigate();
  const [title, setTitle] = useState("");
  const [text, setText] = useState("");
  const [note, setNote] = useState<Note | null>(null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [lastModifiedText, setLastModifiedText] = useState(note?.lastModified);

  useEffect(() => {
    const dataFetch = async () => {
      if (props.mode === NoteEditControlMode.Edit && props.id) {
        const context = await JSendApiClient.get<Note>(`${API_ENDPOINTS.Notes}/${props.id}`);
        if (context) {
          setNote(context);
          setTitle(context.title);
          setText(context.text);
          setLastModifiedText(context.lastModified);
        } else {
          // Note not found - redirect to add.
          navigate(`/notes/add`);
        }
      }
    };
    dataFetch();
  }, [props, navigate]);

  if (props.mode === NoteEditControlMode.Edit && (!props.id || props.id < 0))
    return <Container>Not found</Container>;

  const buttonOptions = [
    {
      key: "saveAndClose",
      icon: "save",
      text: "Save & Close",
      value: "saveAndClose",
      onClick: () => saveNote(true),
    },
    {
      key: "delete",
      icon: "delete",
      text: "Delete note",
      value: "delete",
      onClick: () => setShowDeleteConfirm(true),
    },
  ];

  const saveNote = (closeOnSave: boolean = false) => {
    const mode = props.mode;

    if (mode === NoteEditControlMode.Add) {
      const newNote: Note = {
        id: -1,
        title: title,
        text: text
      };

      JSendApiClient.create(API_ENDPOINTS.Notes, {
        title: newNote.title,
        text: newNote.text
      }).then((id) => {
        if (id !== -1) {
          if (closeOnSave)
            navigate("/notes");
          else
            navigate(`/notes/${id}`);
        }
      });
    } else if (note) {
      note.title = title;
      note.text = text;

      JSendApiClient.update(`${API_ENDPOINTS.Notes}/${note.id}`, {
        id: note.id,
        title: note.title,
        text: note.text,
      }).then((isUpdated) => {
        if (isUpdated) {
          if (!closeOnSave) {
            setLastModifiedText(new Date().toISOString());
          } else {
            navigate("/notes");
          }
        }
      });
    }
  };

  return (
    <>
      <Form style={{marginTop: "1em"}}>
        <h1>{title ? title : 'New note'}</h1>
        <Form.Field
          control={Input}
          label="Title"
          placeholder="Title"
          value={title}
          onChange={(e: any) => {
            setTitle(e.target.value);
          }}
          required
        ></Form.Field>
        <MarkdownEditor
          textLabel="Content"
          text={text}
          onTextChanged={(e: any) => setText(e.target.value)}
          textAreaRows={20}
          required={true}
        />
        {note?.lastModifiedBy && (
          <div style={{color: "gray", marginBottom: "0.75em"}}>
            {`Last modified by ${note.lastModifiedBy} on ${DateUtil.formatDateFromString(lastModifiedText)}.`}
          </div>
        )}
        <Form.Field>
          <Button.Group primary>
            <Button onClick={() => saveNote(false)}>
              <Icon name="save"/>
              Save
            </Button>
            {props.mode !== NoteEditControlMode.Add && (
              <Dropdown
                className="button icon"
                floating
                options={buttonOptions}
                trigger={<></>}
              />
            )}
          </Button.Group>
        </Form.Field>
        <Form.Field>
          <Button as="a" href="/notes">
            <Icon name="list ul"/>
            Back to the list
          </Button>
        </Form.Field>
      </Form>
      <Confirm
        open={showDeleteConfirm}
        content={`Do you really want to delete the note "${title}"?`}
        onCancel={() => setShowDeleteConfirm(false)}
        onConfirm={() => {
          JSendApiClient.delete(`${API_ENDPOINTS.Notes}/${props.id!}`).then((isDeleted) => {
            if (isDeleted) {
              setShowDeleteConfirm(false);
              navigate("/notes");
            }
          });
        }}
      />
    </>
  );
};

export {
  NoteEditControl, NoteEditControlMode
};
