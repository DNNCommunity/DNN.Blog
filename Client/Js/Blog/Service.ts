export interface DnnServiceFramework extends JQueryStatic {
    dnnSF(moduleId: number): DnnServiceFramework;
    getServiceRoot(path: string): string;
    setModuleHeaders(): void;
    getTabId(): string;
}

export default class DataService {
    private moduleId: number = -1;
    private dnn: DnnServiceFramework = <DnnServiceFramework>$;
    public baseServicepath: string = this.dnn.dnnSF(this.moduleId).getServiceRoot('DotNetNuke/Blog');
    public tabId: string = this.dnn.dnnSF(this.moduleId).getTabId();
    constructor(mid: number) {
        this.moduleId = mid;
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
}
