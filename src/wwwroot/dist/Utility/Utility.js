export function downloadFile(fileName, fileContents) {
    const blob = new Blob([fileContents], { type: "application/x-apple-aspen-config" });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
}
