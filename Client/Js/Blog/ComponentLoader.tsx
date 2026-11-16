import * as React from "react";
import { createRoot } from "react-dom/client";

import { AppManager } from "./AppManager";
import CategoriesEditor from "./Components/CategoriesEditor/CategoriesEditor";
import Foo from "./Components/Foo";
import PostsManageTable from "./Components/PostsManageTable";
import TagsEditor from "./Components/TagsEditor/TagsEditor";

export class ComponentLoader {
  public static load(): void {
    document.querySelectorAll(".Blog").forEach((el) => {
      const root = createRoot(el);
      const moduleId = el.dataInt("moduleid");
      const module = AppManager.Modules.Item(moduleId.toString());
      const component = el.dataString("component", "foo");

      if (component === "posts-manage-table") {
        const blogsRaw = el.dataObject("blogs");
        const blogs = Array.isArray(blogsRaw) ? blogsRaw : [];
        root.render(
          <PostsManageTable
            module={module}
            editUrl={el.dataString("edit-url", "")}
            blogs={blogs}
          />
        );
        return;
      }

      if (component === "categories-editor") {
        const localesRaw = el.dataObject("locales");
        const locales = (Array.isArray(localesRaw) ? localesRaw : []).map(
          (l: any) => ({
            Code: l.Code || l.code || "",
            Name: l.Name || l.name || "",
          })
        );
        root.render(
          <CategoriesEditor
            module={module}
            vocabularyId={el.dataInt("vocabulary-id")}
            defaultLocale={el.dataString("default-locale", "")}
            locales={locales}
            multilingual={el.dataString("multilingual", "false") === "true"}
          />
        );
        return;
      }

      if (component === "tags-editor") {
        const localesRaw = el.dataObject("locales");
        const locales = (Array.isArray(localesRaw) ? localesRaw : []).map(
          (l: any) => ({
            Code: l.Code || l.code || "",
            Name: l.Name || l.name || "",
          })
        );
        root.render(
          <TagsEditor
            module={module}
            defaultLocale={el.dataString("default-locale", "")}
            locales={locales}
          />
        );
        return;
      }

      root.render(
        <Foo module={module} bar={el.dataString("bar", "foobar")} />
      );
    });
  }
}
