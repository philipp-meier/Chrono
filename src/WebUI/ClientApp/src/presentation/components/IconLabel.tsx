import { Icon, SemanticICONS } from "semantic-ui-react";

const IconLabel = (props: {
  text: string;
  icon?: SemanticICONS;
  deleteCallback?: (labelText: string) => void;
}) => {
  return (
    <div
      className="ui label"
      style={{ marginLeft: "0em", textAlign: "center" }}
    >
      {props.icon && <Icon name={props.icon} />}
      {props.text}
      {props.deleteCallback && (
        <Icon
          name="delete"
          onClick={(e: MouseEvent) => {
            const label = (e.target as HTMLElement).parentElement!;
            const labelValue = label.textContent!;
            props.deleteCallback!(labelValue);
          }}
        />
      )}
    </div>
  );
};

export default IconLabel;
