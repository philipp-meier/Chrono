import ReactMarkdown from "react-markdown";
import {useMediaQuery} from "react-responsive";
import {Button, Container, Icon} from "semantic-ui-react";
import SyntaxHighlighter from "react-syntax-highlighter"
import {dracula} from 'react-syntax-highlighter/dist/esm/styles/hljs';

// Shared
import {Task} from "../../Shared/Entities/Task";
import {Category} from "../../Shared/Entities/Category";
import IconLabel from "../../Shared/Components/IconLabel";

const TaskItemViewControl = (props: { task: Task; moveUp?: any; moveDown?: any }) => {
  const isMobileOptimized = useMediaQuery({query: "(max-width:682px)"});
  const labels = props.task.categories.map(
    (category: Category, index: number) => (
      <IconLabel key={index} text={category.name}/>
    )
  );

  let description = props.task.description;
  const maxDescriptionLength = 500;
  if (description && description.length > maxDescriptionLength) {
    description = `${description.substring(0, maxDescriptionLength - 3)}...`;
  }

  return (
    <Container className="item tasklist-item">
      <Container className="content">
        <Container className="header">
          <Container className="headline">
            <IconLabel
              text={(!isMobileOptimized ? "No.: " : "#") + props.task.position}
            />
            <a
              href={`/lists/${props.task.listId}/tasks/${props.task.id}`}
              className="task-name"
              style={{marginLeft: "0.25em"}}
            >
              {props.task.name}
            </a>
          </Container>
          {!props.task.done && (
            <Container className="actions" textAlign="right">
              <Button
                icon
                basic
                primary
                size="tiny"
                aria-label="Move up"
                onClick={props.moveUp}
                disabled={!props.moveUp}
              >
                <Icon name="angle up"/>
              </Button>
              <Button
                icon
                basic
                color="red"
                size="tiny"
                aria-label="Move down"
                onClick={props.moveDown}
                disabled={!props.moveDown}
              >
                <Icon name="angle down"/>
              </Button>
            </Container>
          )}
        </Container>
        <Container className="description">
          <ReactMarkdown children={description} components={{
            code({node, inline, className, children, ...props}) {
              const match = /language-(\w+)/.exec(className || '')
              return !inline && match ? (
                <SyntaxHighlighter
                  {...props}
                  children={String(children).replace(/\n$/, '')}
                  style={dracula}
                  language={match[1]}
                  PreTag="div"
                />
              ) : (
                <code {...props} className={className}>
                  {children}
                </code>
              )
            }
          }}/>
        </Container>
        <Container className="extra">{labels}</Container>
      </Container>
    </Container>
  );
};

export default TaskItemViewControl;
