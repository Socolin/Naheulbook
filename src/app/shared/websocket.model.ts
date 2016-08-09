export interface WsMessage {
    opcode: string;
    type: string;
    id: number;
    data: any;
}

export interface WsEvent {
    opcode: string;
    data: any;
}
