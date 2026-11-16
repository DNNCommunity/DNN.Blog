import { AppModule, IAppModule } from "./Models/IAppModule";
import { KeyedCollection } from "./Models/IKeyedCollection";
import DataService from "./Service";

declare global {
  interface Element {
    dataInt: (prop: string) => number;
    dataString: (prop: string, defaultValue: string) => string;
    dataObject: (prop: string) => any;
  }
}

if (!Element.prototype.dataInt) {
  Element.prototype.dataInt = function (this: Element, prop: string): number {
    if (this.getAttribute("data-" + prop) == null) return 0;
    return parseInt(this.getAttribute("data-" + prop) as string);
  };

  Element.prototype.dataString = function (
    this: Element,
    prop: string,
    defaultValue: string
  ): string {
    if (this.getAttribute("data-" + prop) == null) return defaultValue;
    return this.getAttribute("data-" + prop) as string;
  };

  Element.prototype.dataObject = function (this: Element, prop: string): any {
    if (this.getAttribute("data-" + prop) == null) return null;
    return JSON.parse(this.getAttribute("data-" + prop) as string);
  };
}

export class AppManager {
  public static Modules = new KeyedCollection<IAppModule>();

  public static loadData(): void {
    document.querySelectorAll(".DotNetNukeBlog").forEach((el) => {
      var moduleId = el.dataInt("moduleid");
      if (AppManager.Modules.ContainsKey(moduleId.toString())) return;
      AppManager.Modules.Add(
        moduleId.toString(),
        new AppModule(
          moduleId,
          el.dataInt("tabid"),
          el.dataString("locale", "en-US"),
          el.dataObject("resources"),
          el.dataObject("common"),
          el.dataObject("security"),
          new DataService(moduleId)
        )
      );
    });
  }
}
