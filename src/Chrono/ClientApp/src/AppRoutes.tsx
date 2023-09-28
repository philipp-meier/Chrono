import {TaskControlMode} from "./Features/Tasks/TaskEditControl";
import HomePage from "./Features/HomePage/HomePage";
import LoginPage from "./Features/Login/LoginPage";
import MasterDataPage from "./Features/MasterData/MasterDataPage";
import TaskPage from "./Features/Tasks/TaskPage";
import TaskListPage from "./Features/TaskLists/TaskListPage";
import NotePage from "./Features/Notes/NotePage";
import NoteEditPage from "./Features/Notes/NoteEditPage";
import {NoteEditControlMode} from "./Features/Notes/NoteEditControl";

const AppRoutes = [
  {
    index: true,
    element: <HomePage/>,
  },
  {
    path: "lists/:listId?",
    element: <TaskListPage/>,
    requireAuth: true,
  },
  {
    path: "lists/:listId/tasks/:id",
    element: <TaskPage mode={TaskControlMode.Edit}/>,
    requireAuth: true,
  },
  {
    path: "lists/:listId/tasks/add",
    element: <TaskPage mode={TaskControlMode.Add}/>,
    requireAuth: true,
  },
  {
    path: "notes",
    element: <NotePage/>,
    requireAuth: true,
  },
  {
    path: "notes/:id",
    element: <NoteEditPage mode={NoteEditControlMode.Edit}/>,
    requireAuth: true,
  },
  {
    path: "notes/add",
    element: <NoteEditPage mode={NoteEditControlMode.Add}/>,
    requireAuth: true,
  },
  {
    path: "masterdata",
    element: <MasterDataPage/>,
    requireAuth: true,
  },
  {
    path: "login",
    element: <LoginPage sign="in"/>,
  },
  {
    path: "logout",
    element: <LoginPage sign="out"/>,
  },
];

export default AppRoutes;
