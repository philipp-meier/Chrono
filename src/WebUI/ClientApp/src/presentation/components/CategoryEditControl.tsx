import IconLabel from "./IconLabel";
import { useEffect, useState } from "react";
import { Dropdown } from "semantic-ui-react";
import { Category } from "../../domain/models/Category";
import { getCategories } from "../../infrastructure/services/CategoryService";
import { useMediaQuery } from "react-responsive";

const CategoryEditControl = (props: {
  currentCategories: Category[];
  onAdd: (category: Category) => void;
  onDelete: (category: Category) => void;
  disabled?: boolean;
}) => {
  const [selection, setSelection] = useState("");
  const [availableCategories, setAvailableCategories] = useState(
    [] as Category[]
  );
  const isMediumScreen = useMediaQuery({ query: "(min-width:768px)" });

  useEffect(() => {
    const dataFetch = async () => {
      const categories = await getCategories();
      setAvailableCategories(categories);
    };

    dataFetch();
  }, []);

  const filteredCategories = availableCategories
    .filter((x) => !props.currentCategories.map((x) => x.name).includes(x.name))
    .map((x) => {
      return { key: x.name, text: x.name, value: x.name };
    });

  return (
    <>
      {(!props.disabled || props.currentCategories.length > 0) && (
        <label className="category-header">Categories</label>
      )}
      {!props.disabled && (
        <Dropdown
          search={isMediumScreen}
          selection
          placeholder="Add Category"
          options={filteredCategories}
          value={selection}
          style={{ marginBottom: "0.5em" }}
          clearable
          onChange={(e: any) => {
            const category = availableCategories.find(
              (x) => x.name === e.target.innerText
            );
            if (category) {
              props.onAdd(category);

              // Reset selection
              setSelection("");
            }
          }}
        />
      )}
      {props.currentCategories.map((category: Category, index: number) => (
        <IconLabel
          key={index}
          text={category.name}
          deleteCallback={
            props.disabled
              ? undefined
              : (categoryName: string) => {
                  const category = availableCategories.find(
                    (x) => x.name === categoryName
                  );
                  if (category) {
                    props.onDelete(category);
                  }
                }
          }
        />
      ))}
    </>
  );
};

export default CategoryEditControl;
