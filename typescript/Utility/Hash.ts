export async function hash(str: string): Promise<string> {
    const encoder = new TextEncoder();
    const buffer = await window.crypto.subtle.digest("SHA-256", encoder.encode(str));
    return buffer2hexStr(buffer);
}

export function buffer2hexStr(buffer: ArrayBuffer): string {
    return [...new Uint8Array(buffer)]
        .map(x => x.toString(16).padStart(2, "0"))
        .join("");
}