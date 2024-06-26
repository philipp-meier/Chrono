export type NotePreview = {
  id: number;
  title: string;
  preview: string;
  isPinned: boolean;
  created: string;
}

export type GetMyNotesResponse = {
  notes: NotePreview[];
};

export type Note = {
  id: number;
  title: string;
  text: string;
  lastModifiedBy?: string;
  lastModified?: string;
}