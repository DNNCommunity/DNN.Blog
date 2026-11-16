import { Utils } from "./Utils";

function createHandler(
  component: any,
  key: string,
  property: string,
  type?: string
) {
  return (e: any) => {
    const el: any = e.target;
    var value: any = el.type === "checkbox" ? el.checked : el.value;
    if (type) {
      switch (type) {
        case "int":
          if (value != "" && !Utils.isInt(value)) return;
          if (value != "") value = parseInt(value);
      }
    }
    var obj = component.state[key];
    obj[property] = value;
    component.setState({
      [key]: obj
    });
  };
}

export function linkState(
  component: any,
  key: string,
  property: string,
  type?: string
) {
  return createHandler(component, key, property, type);
}

