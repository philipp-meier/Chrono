import {TaskList, TaskListBrief} from "../Entities/TaskList";
import {TaskListOptions} from "../Entities/TaskListOptions";
import JSendApiClient, {API_ENDPOINTS} from "./JSendApiClient.ts";

export async function getTaskLists(): Promise<TaskListBrief[]> {
  return await JSendApiClient.get<TaskListBrief[]>(API_ENDPOINTS.TaskLists) ?? [];
}

export async function getTaskList(id: number): Promise<TaskList | null> {
  return await JSendApiClient.get<TaskList>(`${API_ENDPOINTS.TaskLists}/${id}`)
}

export async function getTaskListOptions(id: number): Promise<TaskListOptions | null> {
  return await JSendApiClient.get<TaskListOptions>(`${API_ENDPOINTS.TaskLists}/${id}/options`)
}

export async function createTaskList(title: string): Promise<number> {
  return await JSendApiClient.create(API_ENDPOINTS.TaskLists, {title: title});
}

export async function updateTaskList(
  id: number,
  title: string,
  options: TaskListOptions
): Promise<boolean> {
  return await JSendApiClient.update(`${API_ENDPOINTS.TaskLists}/${id}`, {
    taskListId: id,
    title: title,
    requireBusinessValue: options.requireBusinessValue,
    requireDescription: options.requireDescription
  });
}

export async function deleteTaskList(id: number): Promise<boolean> {
  return await JSendApiClient.delete(`${API_ENDPOINTS.TaskLists}/${id}`);
}
