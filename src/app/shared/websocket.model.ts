export interface WsMessage {
    opcode: string;
    type: string;
    id: number;
    data: any;
}

export class WsEvent {
    id: number;
    opcode?: string;
    data: any;
}
