export type FilteredKeysOf<TObj, TFilter> = {
    [K in keyof TObj]: TObj[K] extends TFilter ? K : never;
}[keyof TObj]

export function downloadFile(fileName: string, fileContents: string) {
    const blob = new Blob([fileContents], { type: "application/x-apple-aspen-config" })
    const url: string = window.URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);    
    window.URL.revokeObjectURL(url);
}