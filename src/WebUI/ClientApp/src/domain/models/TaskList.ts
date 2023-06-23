import { Task } from "./Task";

export type TaskList = {
  id: number;
  title: string;
  tasks: Task[];
};
