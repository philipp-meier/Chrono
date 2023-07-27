import {Tab} from "semantic-ui-react";
import MasterDataListControl, {MasterDataItem,} from "../components/MasterDataList/MasterDataListControl";
import {createTaskList, deleteTaskList, getTaskLists,} from "../../infrastructure/services/TaskListService";
import React from "react";
import {createCategory, deleteCategory, getCategories,} from "../../infrastructure/services/CategoryService";
import AddItemModal from "../components/MasterDataList/AddItemModal";
import TaskListEditModal from "../components/MasterDataList/TaskListEditModal";

interface MasterDataPageState {
  taskLists: MasterDataItem[];
  categories: MasterDataItem[];
}

export default class MasterDataPage extends React.Component<any, MasterDataPageState> {
  constructor(props: any) {
    super(props);
    this.state = {taskLists: [], categories: []};
  }

  componentDidMount(): void {
    this.refreshDataAsync();
  }

  async refreshDataAsync() {
    const taskLists = (await getTaskLists()).map((x) => {
      return {id: x.id, name: x.title};
    });
    const categories = (await getCategories()).map((x) => {
      return {id: x.id, name: x.name};
    });

    this.setState({taskLists, categories});
  }

  render() {
    const panes = [
      {
        menuItem: "Task lists",
        render: () => (
          <Tab.Pane>
            <MasterDataListControl
              itemTitle="Task List"
              items={this.state.taskLists.sort((a, b) => a.name.localeCompare(b.name))}
              addModal={<AddItemModal itemTitle="Task List" onAdd={(title: string) => {
                createTaskList(title).then((id) => {
                  if (id > 0) {
                    this.state.taskLists.push({id: id, name: title});
                    this.setState({taskLists: this.state.taskLists, categories: this.state.categories});
                  }
                });
              }}/>}
              editModal={<TaskListEditModal/>}
              onDelete={(item) => {
                const index = this.state.taskLists.indexOf(item);
                if (index >= 0) {
                  deleteTaskList(item.id).then((isDeleted) => {
                    if (isDeleted) {
                      this.state.taskLists.splice(index, 1);
                      this.setState({taskLists: this.state.taskLists, categories: this.state.categories});
                    }
                  });
                }
              }}
              refreshDataCallback={() => this.refreshDataAsync()}
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
              items={this.state.categories.sort((a, b) => a.name.localeCompare(b.name))}
              addModal={<AddItemModal itemTitle="Category" onAdd={(title: string) => {
                createCategory(title).then((id) => {
                  if (id > 0) {
                    this.state.categories.push({id: id, name: title});
                    this.setState({taskLists: this.state.taskLists, categories: this.state.categories});
                  }
                });
              }}/>}
              onDelete={(item) => {
                const index = this.state.categories.indexOf(item);
                if (index >= 0) {
                  deleteCategory(item.id).then((isDeleted) => {
                    if (isDeleted) {
                      this.state.categories.splice(index, 1);
                      this.setState({taskLists: this.state.taskLists, categories: this.state.categories});
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
        menu={{attached: true, tabular: false}} panes={panes}
        className="masterData tabs"
      />
    );
  }
}
