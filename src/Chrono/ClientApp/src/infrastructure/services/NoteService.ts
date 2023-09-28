import {GetMyNotesResponse, Note} from "../../domain/models/Note.ts";

export async function getMyNotes(): Promise<GetMyNotesResponse> {
  try {
    const response = await fetch(`/api/notes`);
    return response.ok ? await response.json() : [];
  } catch (error) {
    return {notes: []};
  }
}

export async function getNote(id: number): Promise<Note | null> {
  try {
    const response = await fetch(`/api/notes/${id}`);
    return response.ok ? await response.json() : null;
  } catch (error) {
    return null;
  }
}

export async function createNote(note: Note): Promise<boolean> {
  try {
    const response = await fetch("/api/notes", {
      method: "POST",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify({
        title: note.title,
        text: note.text,
      }),
    });

    return response.status === 200;
  } catch (error) {
    return false;
  }
}

export async function updateNote(
  updatedNote: Note,
): Promise<boolean> {
  try {
    const response = await fetch(`/api/notes/${updatedNote.id}`, {
      method: "PUT",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify({
        id: updatedNote.id,
        title: updatedNote.title,
        text: updatedNote.text,
      }),
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}

export async function deleteNote(id: number): Promise<boolean> {
  try {
    const response = await fetch(`/api/notes/${id}`, {
      method: "DELETE",
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}
