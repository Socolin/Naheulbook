export interface WsMessage {
    opcode: string;
    type: string;
    id: number;
    data: any;
}

export interface WsEvent {
    id: number;
    opcode?: string;
    data: any;
}
