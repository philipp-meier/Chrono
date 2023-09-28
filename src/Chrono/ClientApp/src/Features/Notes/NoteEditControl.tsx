import {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {Button, Confirm, Container, Dropdown, Form, Icon, Input, TextArea,} from "semantic-ui-react";
import {createNote, deleteNote, getNote, updateNote} from "./NoteService";

// Shared
import {Note} from "../../Shared/Entities/Note";

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
  const [note, setNote] = useState(null as Note | null);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  useEffect(() => {
    const dataFetch = async () => {
      if (props.mode === NoteEditControlMode.Edit && props.id) {
        const context = await getNote(props.id);
        if (context) {
          setNote(context);
          setTitle(context.title);
          setText(context.text);
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
      key: "delete",
      icon: "delete",
      text: "Delete Note",
      value: "delete",
      onClick: () => setShowDeleteConfirm(true),
    },
  ];

  const saveNote = () => {
    const mode = props.mode;

    if (mode === NoteEditControlMode.Add) {
      const newNote: Note = {
        id: -1,
        title: title,
        text: text
      };

      createNote(newNote).then((isUpdated) => {
        if (isUpdated) navigate("/notes");
      });
    } else if (note) {
      note.title = title;
      note.text = text;

      updateNote(note).then((isUpdated) => {
        if (isUpdated) navigate("/notes");
      });
    }
  };

  return (
    <>
      <Form style={{marginTop: "1em"}} onSubmit={saveNote}>
        <h1>{title}</h1>
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
        <Form.Field
          control={TextArea}
          label="Content"
          placeholder="Content"
          value={text}
          onChange={(e: any) => {
            setText(e.target.value);
          }}
          required
        ></Form.Field>
        {note?.lastModifiedBy && (
          <div style={{color: "gray", marginBottom: "0.75em"}}>
            {`Last modified by ${note.lastModifiedBy} on ${new Date(
              note.lastModified!
            ).toLocaleDateString()} ${new Date(
              note.lastModified!
            ).toLocaleTimeString()}.`}
          </div>
        )}
        <Form.Field>
          <Button.Group primary>
            <Button type="submit">
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
          deleteNote(props.id!).then((isDeleted) => {
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

export {NoteEditControl, NoteEditControlMode};
