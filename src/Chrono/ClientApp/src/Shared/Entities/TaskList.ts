import {Task} from "./Task";

export type TaskListBrief = {
  id: number;
  title: string;
};

export type TaskList = {
  id: number;
  title: string;
  tasks: Task[];
};
