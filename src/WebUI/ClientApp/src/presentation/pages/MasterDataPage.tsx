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

interface TabPaneRenderOptions {
  title: string;
  items: MasterDataItem[];
  createCallback: (title: string) => Promise<number>;
  deleteCallback: (id: number) => Promise<boolean>;
  editModal?: any;
}

export default class MasterDataPage extends React.Component<any, MasterDataPageState> {
  constructor(props: any) {
    super(props);
    this.state = {taskLists: [], categories: []};
  }

  async componentDidMount(): Promise<void> {
    await this.refreshMasterDataAsync();
  }

  async refreshMasterDataAsync(): Promise<void> {
    const taskLists = (await getTaskLists()).map((x) => {
      return {id: x.id, name: x.title};
    });
    const categories = (await getCategories()).map((x) => {
      return {id: x.id, name: x.name};
    });

    this.setState({taskLists, categories});
  }

  render() {
    return (
      <Tab
        menu={{attached: true, tabular: false}}
        panes={this.buildTabPanes()}
        className="masterData tabs"
      />
    );
  }

  buildTabPanes() {
    return [
      {
        menuItem: "Task lists",
        render: this.renderTabPane({
          title: "Task List",
          items: this.state.taskLists,
          createCallback: createTaskList,
          deleteCallback: deleteTaskList,
          editModal: <TaskListEditModal/>
        }),
      },
      {
        menuItem: "Categories",
        render: this.renderTabPane({
          title: "Category",
          items: this.state.categories,
          createCallback: createCategory,
          deleteCallback: deleteCategory
        }),
      },
    ]
  }

  renderTabPane(options: TabPaneRenderOptions) {
    return () => (
      <Tab.Pane>
        <MasterDataListControl
          itemTitle={options.title}
          items={this.sortMasterDataItems(options.items)}
          addModal={
            <AddItemModal
              itemTitle={options.title}
              onAdd={(title: string) => this.createMasterDataItem(options.items, title, options.createCallback)}
            />
          }
          editModal={options.editModal}
          onDelete={(item: MasterDataItem) => this.deleteMasterDataItem(options.items, item, options.deleteCallback)}
          refreshDataCallback={() => this.refreshMasterDataAsync()}
        />
      </Tab.Pane>
    );
  }

  sortMasterDataItems(items: MasterDataItem[]) {
    return items.sort((a, b) => a.name.localeCompare(b.name));
  }

  updateState() {
    this.setState({taskLists: this.state.taskLists, categories: this.state.categories});
  }

  createMasterDataItem(items: MasterDataItem[], title: string, createCallback: (title: string) => Promise<number>) {
    createCallback(title).then((id) => {
        if (id <= 0)
          return;

        items.push({id: id, name: title});
        this.updateState();
      }
    );
  }

  deleteMasterDataItem(items: MasterDataItem[], item: MasterDataItem, deleteCallback: (id: number) => Promise<boolean>) {
    const index = items.indexOf(item);
    if (index < 0)
      return;

    deleteCallback(item.id).then((isDeleted) => {
        if (isDeleted) {
          items.splice(index, 1);
          this.updateState();
        }
      }
    );
  }
}
