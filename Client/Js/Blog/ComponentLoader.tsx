import * as React from "react";
import { createRoot } from "react-dom/client";

import { AppManager } from "./AppManager";
import CategoriesEditor from "./Components/CategoriesEditor/CategoriesEditor";
import Foo from "./Components/Foo";
import CommentsManageTable from "./Components/CommentsManageTable";
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
            deleteConfirm={el.dataString("delete-confirm", "Delete this post?")}
            deleteLabel={el.dataString("delete-label", "Delete")}
            cancelLabel={el.dataString("cancel-label", "Cancel")}
          />
        );
        return;
      }

      if (component === "comments-manage-table") {
        const blogsRaw = el.dataObject("blogs");
        const blogs = Array.isArray(blogsRaw) ? blogsRaw : [];
        root.render(
          <CommentsManageTable
            module={module}
            blogs={blogs}
            deleteConfirm={el.dataString(
              "delete-confirm",
              "Delete this comment? Its content will be replaced with a placeholder."
            )}
            bulkDeleteConfirm={el.dataString(
              "bulk-delete-confirm",
              "Delete the selected comments? Their content will be replaced with a placeholder."
            )}
            deleteLabel={el.dataString("delete-label", "Delete")}
            flagLabel={el.dataString("flag-label", "Flag")}
            flagConfirm={el.dataString(
              "flag-confirm",
              "Flag this comment? It will be hidden from public view."
            )}
            bulkDeleteLabel={el.dataString(
              "bulk-delete-label",
              "Delete selected"
            )}
            cancelLabel={el.dataString("cancel-label", "Cancel")}
            searchPlaceholder={el.dataString(
              "search-placeholder",
              "Search comments"
            )}
            authorPlaceholder={el.dataString(
              "author-placeholder",
              "Filter by author"
            )}
            allBlogsLabel={el.dataString("all-blogs-label", "All blogs")}
            allStatusesLabel={el.dataString(
              "all-statuses-label",
              "All statuses"
            )}
            showMoreLabel={el.dataString("show-more-label", "Show more")}
            showLessLabel={el.dataString("show-less-label", "Show less")}
            emptyLabel={el.dataString("empty-label", "No comments found.")}
            approvedLabel={el.dataString("approved-label", "Approved")}
            pendingLabel={el.dataString("pending-label", "Pending")}
            deletedLabel={el.dataString("deleted-label", "Deleted")}
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
