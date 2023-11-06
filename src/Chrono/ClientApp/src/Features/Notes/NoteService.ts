// Shared
import {GetMyNotesResponse, Note} from "../../Shared/Entities/Note";
import JSendApiClient, {API_ENDPOINTS} from "../../Shared/Services/JSendApiClient.ts";

export async function getMyNotes(): Promise<GetMyNotesResponse> {
  return await JSendApiClient.get<GetMyNotesResponse>(API_ENDPOINTS.Notes) ?? {notes: []};
}

export async function getNote(id: number): Promise<Note | null> {
  return await JSendApiClient.get<Note>(`${API_ENDPOINTS.Notes}/${id}`);
}

export async function createNote(note: Note): Promise<boolean> {
  const result = await JSendApiClient.create(API_ENDPOINTS.Notes, {
    title: note.title,
    text: note.text,
  });

  return result !== -1;
}

export async function updateNote(
  updatedNote: Note,
): Promise<boolean> {
  return await JSendApiClient.update(`${API_ENDPOINTS.Notes}/${updatedNote.id}`, {
    id: updatedNote.id,
    title: updatedNote.title,
    text: updatedNote.text,
  });
}

export async function deleteNote(id: number): Promise<boolean> {
  return await JSendApiClient.delete(`${API_ENDPOINTS.Notes}/${id}`);
}
