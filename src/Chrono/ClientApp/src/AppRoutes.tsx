import {TaskControlMode} from "./presentation/components/TaskControl/TaskControl";
import HomePage from "./presentation/pages/HomePage";
import LoginPage from "./presentation/pages/LoginPage";
import MasterDataPage from "./presentation/pages/MasterDataPage";
import TaskPage from "./presentation/pages/TaskPage";
import TaskListPage from "./presentation/pages/TaskListPage";
import NotesPage from "./presentation/pages/NotesPage.tsx";
import NotePage from "./presentation/pages/NotePage.tsx";
import {NoteControlMode} from "./presentation/components/Notes/NoteControl.tsx";

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
    element: <NotesPage/>,
    requireAuth: true,
  },
  {
    path: "notes/:id",
    element: <NotePage mode={NoteControlMode.Edit}/>,
    requireAuth: true,
  },
  {
    path: "notes/add",
    element: <NotePage mode={NoteControlMode.Add}/>,
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
