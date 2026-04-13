export interface DeviceConfig {
    id: string;
    enableBlocking: boolean;
}

export interface FilterReference {
    id: string;
    enableBlocking: boolean;
}

export interface BlockingState {
    enabled: boolean
}

export interface FilterConfig {
    id: string;
    description: string;
    domainsToBlock: string[];
}