import * as React from "react";
import { createRoot } from 'react-dom/client';

import { AppManager } from "./AppManager";
import Foo from "./Components/Foo";

export class ComponentLoader {
  public static load(): void {
    document.querySelectorAll(".Blog").forEach((el) => {
      const root = createRoot(el);
      const moduleId = el.dataInt("moduleid");
      root.render(
        <Foo
          module={AppManager.Modules.Item(moduleId.toString())}
          bar={el.dataString("bar", "foobar")}
        />
      );
    });
  }
}
