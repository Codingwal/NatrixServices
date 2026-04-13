export type FilteredKeysOf<TObj, TFilter> = {
    [K in keyof TObj]: TObj[K] extends TFilter ? K : never;
}[keyof TObj]