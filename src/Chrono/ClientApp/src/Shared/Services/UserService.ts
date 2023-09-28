import {User, UserSettings} from "../Entities/User";

export async function getCurrentUserInfo(): Promise<User> {
  try {
    const response = await fetch("/api/user");
    return await response.json();
  } catch (error) {
    return {isAuthenticated: false};
  }
}

export async function getCurrentUserSettings(): Promise<UserSettings> {
  try {
    const response = await fetch("/api/user/settings");
    return await response.json();
  } catch (error) {
    return {};
  }
}

export async function updateCurrentUserSettings(settings: UserSettings): Promise<boolean> {
  try {
    const response = await fetch(`/api/user/settings`, {
      method: "PUT",
      headers: {"Content-Type": "application/json"},
      body: JSON.stringify(settings),
    });

    return response.status === 204;
  } catch (error) {
    return false;
  }
}
