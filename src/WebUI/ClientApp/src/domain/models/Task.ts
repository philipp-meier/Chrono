import { Category } from "./Category";

export type Task = {
  id: number;
  listId: number;
  position: number;
  name: string;
  businessValue: string;
  description: string;
  done: boolean;
  categories: Category[];
  lastModifiedBy?: string;
  lastModified?: string;
};
