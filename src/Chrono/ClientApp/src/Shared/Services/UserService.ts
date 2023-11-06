import {User, UserSettings} from "../Entities/User";
import JSendApiClient, {API_ENDPOINTS} from "./JSendApiClient.ts";

export async function getCurrentUserInfo(): Promise<User> {
  return await JSendApiClient.get<User>(API_ENDPOINTS.User) ?? {isAuthenticated: false};
}

export async function getCurrentUserSettings(): Promise<UserSettings> {
  return await JSendApiClient.get<UserSettings>(API_ENDPOINTS.UserSettings) ?? {};
}

export async function updateCurrentUserSettings(settings: UserSettings): Promise<boolean> {
  return await JSendApiClient.update(API_ENDPOINTS.UserSettings, settings);
}
