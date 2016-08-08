export interface WsMessage {
    opcode: string;
    type: string;
    id: number;
    data: any;
}
