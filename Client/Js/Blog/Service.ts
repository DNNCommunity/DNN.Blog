import { ICategoryTree } from "./Models/ICategory";
import { IManagePostList, IManagePostQuery } from "./Models/IManagePost";
import { ITagItem, ITagList, ITagListQuery } from "./Models/ITag";

export interface DnnServiceFramework extends JQueryStatic {
    dnnSF(moduleId: number): DnnServiceFramework;
    getServiceRoot(path: string): string;
    setModuleHeaders(): void;
    getTabId(): string;
}

export default class DataService {
    private moduleId: number = -1;
    private dnn: DnnServiceFramework = <DnnServiceFramework>$;
    public baseServicepath: string = "";
    public tabId: string = "";
    constructor(mid: number) {
        this.moduleId = mid;
        this.baseServicepath = this.dnn.dnnSF(this.moduleId).getServiceRoot("DotNetNuke/Blog");
        this.tabId = this.dnn.dnnSF(this.moduleId).getTabId();
    };
    private ajaxCall(type: string, servicePath: string, controller: string, action: string, id: any, headers: any, data: any, success: Function, fail?: Function, isUploadForm?: boolean)
        : void {
        var opts: JQuery.AjaxSettings = {
            headers: headers,
            type: type === "POSTFORM" ? "POST": type,
            url: servicePath + controller + '/' + action + (id != undefined
                ? '/' + id
                : ''),
            beforeSend: this
                .dnn
                .dnnSF(this.moduleId)
                .setModuleHeaders,
            contentType: type === "POSTFORM" ? undefined : "application/json; charset=utf-8",
            data: type == "POST" ? JSON.stringify(data) : data,
            dataType: "json"
        };
        if (isUploadForm) {
            opts.contentType = false;
            opts.processData = false;
        }
        $.ajax(opts)
            .done(function (retdata: any) {
                if (success != undefined) {
                    success(retdata);
                }
            })
            .fail(function (xhr: any, status: any) {
                if (fail != undefined) {
                    fail(xhr.responseText);
                }
            });
    };
    public fooBar(success: Function): void {
        this.ajaxCall('GET', this.baseServicepath, 'ProductTemplates', 'AddItem', undefined, {}, null, success)
    }

    public getManagePosts(
        query: IManagePostQuery,
        success: (data: IManagePostList) => void,
        fail?: (error: string) => void
    ): void {
        const params: any = {
            page: query.page,
            pageSize: query.pageSize,
            orderBy: query.orderBy
        };
        if (query.search) {
            params.search = query.search;
        }
        if (query.blogId != null && query.blogId > 0) {
            params.blogId = query.blogId;
        }
        this.ajaxCall(
            "GET",
            this.baseServicepath,
            "Posts",
            "List",
            undefined,
            {},
            params,
            (data: any) => {
                success({
                    Items: (data && (data.Items || data.items)) || [],
                    Page: (data && (data.Page != null ? data.Page : data.page)) || 0,
                    PageSize: (data && (data.PageSize || data.pageSize)) || query.pageSize,
                    Total: (data && (data.Total != null ? data.Total : data.total)) || 0
                });
            },
            fail
        );
    }

    public approvePost(
        blogId: number,
        postId: number,
        success: Function,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Posts",
            "Approve",
            undefined,
            {},
            { BlogId: blogId, PostId: postId },
            success,
            fail
        );
    }

    public deletePost(
        blogId: number,
        postId: number,
        success: Function,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Posts",
            "Delete",
            undefined,
            {},
            { BlogId: blogId, PostId: postId },
            success,
            fail
        );
    }

    private normalizeCategoryTree(data: any): ICategoryTree {
        const locales = ((data && (data.Locales || data.locales)) || []).map((l: any) => ({
            Code: l.Code || l.code || "",
            Name: l.Name || l.name || ""
        }));
        const mapNode = (n: any): any => ({
            TermId: n.TermId != null ? n.TermId : n.termId,
            Name: n.Name || n.name || "",
            ParentTermId: n.ParentTermId != null ? n.ParentTermId : (n.parentTermId != null ? n.parentTermId : -1),
            ViewOrder: n.ViewOrder != null ? n.ViewOrder : (n.viewOrder != null ? n.viewOrder : 0),
            Localizations: n.Localizations || n.localizations || {},
            Children: ((n.Children || n.children) || []).map(mapNode)
        });
        return {
            Items: ((data && (data.Items || data.items)) || []).map(mapNode),
            DefaultLocale: (data && (data.DefaultLocale || data.defaultLocale)) || "",
            Locales: locales,
            IsMultilingual: !!(data && (data.IsMultilingual != null ? data.IsMultilingual : data.isMultilingual))
        };
    }

    public getCategoryTree(
        success: (data: ICategoryTree) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "GET",
            this.baseServicepath,
            "Categories",
            "Tree",
            undefined,
            {},
            null,
            (data: any) => success(this.normalizeCategoryTree(data)),
            fail
        );
    }

    public addCategories(
        lines: string,
        parentTermId: number,
        success: (data: ICategoryTree) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Categories",
            "Add",
            undefined,
            {},
            { Lines: lines, ParentTermId: parentTermId },
            (data: any) => success(this.normalizeCategoryTree(data)),
            fail
        );
    }

    public updateCategory(
        payload: { TermId: number; Name?: string; ParentTermId?: number; ViewOrder?: number },
        success: (data: ICategoryTree) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Categories",
            "Update",
            undefined,
            {},
            payload,
            (data: any) => success(this.normalizeCategoryTree(data)),
            fail
        );
    }

    public deleteCategory(
        termId: number,
        success: (data: ICategoryTree) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Categories",
            "Delete",
            undefined,
            {},
            { TermId: termId },
            (data: any) => success(this.normalizeCategoryTree(data)),
            fail
        );
    }

    public localizeCategory(
        payload: { TermId: number; Name: string; Localizations: Record<string, string> },
        success: (data: ICategoryTree) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Categories",
            "Localize",
            undefined,
            {},
            payload,
            (data: any) => success(this.normalizeCategoryTree(data)),
            fail
        );
    }

    private parseApiError(responseText: string, fallback: string): string {
        if (!responseText) return fallback;
        try {
            const data = JSON.parse(responseText);
            return (data && (data.Message || data.message)) || fallback;
        } catch {
            return fallback;
        }
    }

    private normalizeTagList(data: any, pageSizeFallback: number): ITagList {
        const locales = ((data && (data.Locales || data.locales)) || []).map((l: any) => ({
            Code: l.Code || l.code || "",
            Name: l.Name || l.name || ""
        }));
        const mapItem = (t: any): ITagItem => ({
            TermId: t.TermId != null ? t.TermId : t.termId,
            Name: t.Name || t.name || "",
            Localizations: t.Localizations || t.localizations || {}
        });
        return {
            Items: ((data && (data.Items || data.items)) || []).map(mapItem),
            Page: (data && (data.Page != null ? data.Page : data.page)) || 0,
            PageSize: (data && (data.PageSize || data.pageSize)) || pageSizeFallback,
            Total: (data && (data.Total != null ? data.Total : data.total)) || 0,
            DefaultLocale: (data && (data.DefaultLocale || data.defaultLocale)) || "",
            Locales: locales
        };
    }

    public getTags(
        query: ITagListQuery,
        success: (data: ITagList) => void,
        fail?: (error: string) => void
    ): void {
        const params: any = {
            page: query.page,
            pageSize: query.pageSize
        };
        if (query.search) {
            params.search = query.search;
        }
        this.ajaxCall(
            "GET",
            this.baseServicepath,
            "Tags",
            "List",
            undefined,
            {},
            params,
            (data: any) => success(this.normalizeTagList(data, query.pageSize)),
            (text: string) => fail && fail(this.parseApiError(text, "Failed to load tags."))
        );
    }

    public addTag(
        name: string,
        success: (item: ITagItem) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Tags",
            "Add",
            undefined,
            {},
            { Name: name },
            (data: any) => {
                const item = (data && (data.Item || data.item)) || data;
                success({
                    TermId: item.TermId != null ? item.TermId : item.termId,
                    Name: item.Name || item.name || name,
                    Localizations: item.Localizations || item.localizations || {}
                });
            },
            (text: string) => fail && fail(this.parseApiError(text, "Failed to add tag."))
        );
    }

    public updateTagCell(
        payload: { TermId: number; Locale: string; Value: string },
        success: (value: string) => void,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Tags",
            "UpdateCell",
            undefined,
            {},
            payload,
            (data: any) => {
                const value =
                    data && (data.Value != null ? data.Value : data.value);
                success(value != null ? value : payload.Value);
            },
            (text: string) => fail && fail(this.parseApiError(text, "Failed to save."))
        );
    }

    public deleteTag(
        termId: number,
        success: Function,
        fail?: (error: string) => void
    ): void {
        this.ajaxCall(
            "POST",
            this.baseServicepath,
            "Tags",
            "Delete",
            undefined,
            {},
            { TermId: termId },
            success,
            (text: string) => fail && fail(this.parseApiError(text, "Failed to delete tag."))
        );
    }
}
