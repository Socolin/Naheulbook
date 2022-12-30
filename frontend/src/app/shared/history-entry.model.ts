export type HistoryEntry = GroupHistoryEntryResponse | CharacterHistoryEntryResponse;

export interface GroupHistoryEntryResponse {
    isGroup: true;

    id: number;
    action: string;
    date: any;
    info?: string;
    data: any;
}

export interface CharacterHistoryEntryResponse {
    isGroup: false;

    id: number;
    action: string;
    date: any;

    info?: string;
    data: any;
    item?: { name?: string };
    modifier?: { name: string };
}
