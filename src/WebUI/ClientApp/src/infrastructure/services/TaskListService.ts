import {TaskList} from "../../domain/models/TaskList";
import {TaskListBrief} from "../../domain/models/TaskListBrief";
import {TaskListOptions} from "../../domain/models/TaskListOptions";

export async function getTaskLists(): Promise<TaskListBrief[]> {
    try {
        const response = await fetch("/api/tasklists");
        return response.ok ? await response.json() : [];
    } catch (error) {
        return [];
    }
}

export async function getTaskList(id: number): Promise<TaskList | null> {
    try {
        const response = await fetch(`/api/tasklists/${id}`);
        return response.ok ? await response.json() : null;
    } catch (error) {
        return null;
    }
}

export async function getTaskListOptions(id: number): Promise<TaskListOptions | null> {
    try {
        const response = await fetch(`/api/tasklists/${id}/options`);
        return response.ok ? await response.json() : null;
    } catch (error) {
        return null;
    }
}

export async function createTaskList(title: string): Promise<number> {
    try {
        const response = await fetch("/api/tasklists", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({title: title}),
        });

        if (response.ok) return await response.json();

        return -1;
    } catch (error) {
        return -1;
    }
}

export async function deleteTaskList(id: number): Promise<boolean> {
    try {
        const response = await fetch(`/api/tasklists/${id}`, {
            method: "DELETE",
        });

        return response.status === 204;
    } catch (error) {
        return false;
    }
}
