import React from "react";
import {Tab} from "semantic-ui-react";
import MasterDataItemAddModal from "./MasterDataItemAddModal";
import MasterDataItemEditModal from "./MasterDataItemEditModal";
import MasterDataItemList from "./MasterDataItemList";
import {MasterDataItem} from "./MasterDataItem";
import {UserSettings} from "../../Entities/User";
import {Category} from "../../Entities/Category";
import {TaskListBrief} from "../../Entities/TaskList";

// Shared
import JSendApiClient, {API_ENDPOINTS} from "../../Shared/JSendApiClient";

interface MasterDataPageState {
    taskLists: MasterDataItem[];
    categories: MasterDataItem[];
    favoriteTaskListId?: number;
    isLoaded: boolean;
}

interface TabPaneRenderOptions {
    title: string;
    titlePlural: string;
    items: MasterDataItem[];
    createCallback: (title: string) => Promise<number>;
    deleteCallback: (id: number) => Promise<boolean>;
    editModal?: any;
    favoriteItemId?: number;
}

export default class MasterDataPage extends React.Component<any, MasterDataPageState> {
    constructor(props: any) {
        super(props);
        this.state = {taskLists: [], categories: [], isLoaded: false};
    }

    async componentDidMount(): Promise<void> {
        await this.refreshMasterDataAsync();
    }

    async refreshMasterDataAsync(): Promise<void> {
        const taskLists = (await JSendApiClient.get<TaskListBrief[]>(API_ENDPOINTS.TaskLists) ?? []).map((x) => {
            return {id: x.id, name: x.title};
        });
        const categories = (await JSendApiClient.get<Category[]>(API_ENDPOINTS.Categories) ?? []).map((x) => {
            return {id: x.id, name: x.name};
        });

        const userSettings = await JSendApiClient.get<UserSettings>(API_ENDPOINTS.UserSettings);
        this.setState({taskLists, categories, favoriteTaskListId: userSettings?.defaultTaskListId, isLoaded: true});
    }

    render() {
        return this.state.isLoaded ?
            <Tab
                menu={{attached: true, tabular: false}}
                panes={this.buildTabPanes()}
                className="masterData tabs"/>
            : <></>;
    }

    buildTabPanes() {
        return [
            {
                menuItem: "Task lists",
                render: this.renderTabPane({
                    title: "Task List",
                    titlePlural: "Task Lists",
                    items: this.state.taskLists,
                    createCallback: (title: string) => JSendApiClient.create(API_ENDPOINTS.TaskLists, {title: title}),
                    deleteCallback: (id: number) => JSendApiClient.delete(`${API_ENDPOINTS.TaskLists}/${id}`),
                    editModal: <MasterDataItemEditModal/>,
                    favoriteItemId: this.state.favoriteTaskListId
                }),
            },
            {
                menuItem: "Categories",
                render: this.renderTabPane({
                    title: "Category",
                    titlePlural: "Categories",
                    items: this.state.categories,
                    createCallback: (name: string) => JSendApiClient.create(API_ENDPOINTS.Categories, {name: name}),
                    deleteCallback: (id: number) => JSendApiClient.delete(`${API_ENDPOINTS.Categories}/${id}`)
                }),
            },
        ]
    }

    renderTabPane(options: TabPaneRenderOptions) {
        return () => (
            <Tab.Pane>
                <MasterDataItemList
                    itemTitle={options.title}
                    itemTitlePlural={options.titlePlural}
                    items={this.sortMasterDataItems(options.items)}
                    addModal={
                        <MasterDataItemAddModal
                            itemTitle={options.title}
                            onAdd={(title: string) => this.createMasterDataItem(options.items, title, options.createCallback)}
                        />
                    }
                    editModal={options.editModal}
                    onDelete={(item: MasterDataItem) => this.deleteMasterDataItem(options.items, item, options.deleteCallback)}
                    refreshDataCallback={() => this.refreshMasterDataAsync()}
                    favoriteItemId={options.favoriteItemId}
                />
            </Tab.Pane>
        );
    }

    sortMasterDataItems(items: MasterDataItem[]) {
        return items.sort((a, b) => a.name.localeCompare(b.name));
    }

    updateState() {
        this.setState({taskLists: this.state.taskLists, categories: this.state.categories, isLoaded: true});
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
