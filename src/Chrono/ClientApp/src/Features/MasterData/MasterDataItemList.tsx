import {Button, Confirm, Container, List} from "semantic-ui-react";
import React, {ReactElement, useState} from "react";
import {MasterDataItem} from "./MasterDataItem";

// Shared
import NoItemsMessage from "../../Shared/Components/NoItemsMessage";

const MasterDataItemList = (props: {
  items: MasterDataItem[];
  itemTitle: string;
  itemTitlePlural: string;
  addModal: ReactElement;
  editModal?: ReactElement;
  onDelete: (item: MasterDataItem) => void;
  refreshDataCallback?: () => void;
  favoriteItemId?: number;
}) => {
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [currentItem, setCurrentItem] = useState<MasterDataItem | null>(null);

  const masterDataItems = props.items.map((x, idx) => {
      const favoriteItemHeaderStyle = {color: "#2185d0", fontWeight: "bold", fontSize: "1.1em"};
      return (
        <List.Item key={idx}>
          <Container style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
            <div>
              <span style={props.favoriteItemId === x.id ? favoriteItemHeaderStyle : undefined}>
                {x.name}
              </span>
            </div>
            <div style={{display: "flex", justifyContent: "flex-end"}}>
              {props.editModal && <Button
                  data-edit={x.name}
                  onClick={() => {
                    setCurrentItem(x);
                    setShowEditModal(true);
                  }}
              >
                  Edit
              </Button>}
              <Button
                data-delete={x.name}
                onClick={() => {
                  setCurrentItem(x);
                  setShowDeleteConfirm(true);
                }}
              >
                Delete
              </Button>
            </div>
          </Container>
        </List.Item>
      )
    }
  )

  return (
    <>
      <Container textAlign="right" style={{marginBottom: "1em"}}>
        {props.addModal}
      </Container>

      <Container>
        {masterDataItems.length > 0 && <List divided verticalAlign="middle">
          {masterDataItems}
        </List>}
        {masterDataItems.length == 0 &&
            <NoItemsMessage text={`You do not have any ${props.itemTitlePlural.toLowerCase()} yet.`}/>
        }
      </Container>
      {/* Is there a better way to inject custom (edit) modals with properties? */}
      {showEditModal && React.cloneElement(props.editModal!, {
        context: currentItem,
        onButtonClick: (saved: boolean) => {
          if (saved)
            props.refreshDataCallback!();

          setShowEditModal(!showEditModal);
        }
      })}
      <Confirm
        open={showDeleteConfirm}
        content={`Do you really want to delete the item "${
          currentItem ? currentItem.name : "-"
        }"?`}
        onCancel={() => setShowDeleteConfirm(false)}
        onConfirm={() => {
          props.onDelete(currentItem!);
          setShowDeleteConfirm(false);
        }}
      />
    </>
  );
};

export default MasterDataItemList;
