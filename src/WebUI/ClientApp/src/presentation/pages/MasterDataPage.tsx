import { Tab } from "semantic-ui-react";
import MasterDataListControl, {
  MasterDataItem,
} from "../components/MasterDataList/MasterDataListControl";
import {
  createTaskList,
  deleteTaskList,
  getTaskLists,
} from "../../infrastructure/services/TaskListService";
import { useEffect, useState } from "react";
import {
  createCategory,
  deleteCategory,
  getCategories,
} from "../../infrastructure/services/CategoryService";

const MasterDataPage = () => {
  const [taskLists, setTaskLists] = useState([] as MasterDataItem[]);
  const [categories, setCategories] = useState([] as MasterDataItem[]);

  useEffect(() => {
    const dataFetch = async () => {
      setTaskLists(
        (await getTaskLists()).map((x) => {
          return { id: x.id, name: x.title };
        })
      );
      setCategories(
        (await getCategories()).map((x) => {
          return { id: x.id, name: x.name };
        })
      );
    };

    dataFetch();
  }, []);

  const panes = [
    {
      menuItem: "Task lists",
      render: () => (
        <Tab.Pane>
          <MasterDataListControl
            itemTitle="Task List"
            items={taskLists.sort((a, b) => a.name.localeCompare(b.name))}
            onAdd={(title: string) => {
              createTaskList(title).then((id) => {
                if (id > 0) {
                  taskLists.push({ id: id, name: title });
                  setTaskLists([...taskLists]);
                }
              });
            }}
            onDelete={(item) => {
              const index = taskLists.indexOf(item);
              if (index >= 0) {
                deleteTaskList(item.id).then((isDeleted) => {
                  if (isDeleted) {
                    taskLists.splice(index, 1);
                    setTaskLists([...taskLists]);
                  }
                });
              }
            }}
          />
        </Tab.Pane>
      ),
    },
    {
      menuItem: "Categories",
      render: () => (
        <Tab.Pane>
          <MasterDataListControl
            itemTitle="Category"
            items={categories.sort((a, b) => a.name.localeCompare(b.name))}
            onAdd={(title: string) => {
              createCategory(title).then((id) => {
                if (id > 0) {
                  categories.push({ id: id, name: title });
                  setCategories([...categories]);
                }
              });
            }}
            onDelete={(item) => {
              const index = categories.indexOf(item);
              if (index >= 0) {
                deleteCategory(item.id).then((isDeleted) => {
                  if (isDeleted) {
                    categories.splice(index, 1);
                    setCategories([...categories]);
                  }
                });
              }
            }}
          />
        </Tab.Pane>
      ),
    },
  ];

  return (
    <Tab
      menu={{ attached: true, tabular: false }}
      panes={panes}
      className="masterData tabs"
    />
  );
};

export default MasterDataPage;
