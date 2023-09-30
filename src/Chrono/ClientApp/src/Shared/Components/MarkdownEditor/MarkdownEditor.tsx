import "./MarkdownEditor.css"
import {Form, Segment, Tab, TextArea,} from "semantic-ui-react";
import {dracula} from 'react-syntax-highlighter/dist/esm/styles/hljs';
import ReactMarkdown from "react-markdown";
import SyntaxHighlighter from "react-syntax-highlighter";

type MarkdownEditorProps = {
  textLabel: string;
  text: string;
  onTextChanged: (e: any) => void;
  textAreaRows?: number;
  required?: boolean;
  disabled?: boolean;
}

export const MarkdownEditor = (props: MarkdownEditorProps) => {
  const tabPanes = [
    {
      menuItem: "Write",
      render: () => {
        return (
          <Tab.Pane>
            <Form.Field
              control={TextArea}
              placeholder={props.textLabel}
              value={props.text}
              onChange={props.onTextChanged}
              rows={props.textAreaRows}
              required={props.required}
              disabled={props.disabled}
            ></Form.Field>
          </Tab.Pane>
        );
      }
    },
    {
      menuItem: "Preview",
      render: () => {
        return (
          <Tab.Pane>
            <Segment className="markdown-preview">
              <ReactMarkdown children={props.text} components={{
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

            </Segment>
          </Tab.Pane>
        );
      }
    }
  ];

  return (
    <div className={props.required ? "required field" : "field"}>
      <label>{props.textLabel}</label>
      <Tab panes={tabPanes} menu={{attached: true, tabular: false}}/>
    </div>
  );
};
