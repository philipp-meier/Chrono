import {useParams} from "react-router-dom";
import NoteOverview from "../components/Notes/NoteOverview.tsx";

const NotesPage = () => {
  const {noteId} = useParams();
  const parsedNoteId = noteId ? parseInt(noteId || "-1") : -1;
  return <NoteOverview/>;
};

export default NotesPage;
