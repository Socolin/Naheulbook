import {Injectable} from '@angular/core';
import {Subject, Observable, Observer} from 'rxjs/Rx';
import {WsMessage} from './websocket.model';

@Injectable()
export class WebSocketService {
    private socket: Subject<WsMessage>;
    private pendingData: string[] = [];
    private webSocket: WebSocket;

    public connect(url): Subject<WsMessage> {
        if (!this.socket) {
            this.socket = this.create(url);
        }
        return this.socket;
    }

    private onConnected(ev: Event) {
        for (let i = 0; i < this.pendingData.length; i++) {
            let data = this.pendingData[i];
            this.webSocket.send(data);
        }
    }

    private create(url): Subject<WsMessage> {
        let ws = new WebSocket(url);

        let observable: Observable<MessageEvent> = Observable.create(
            (obs: Observer<MessageEvent>) => {
                ws.onopen = this.onConnected.bind(this);
                ws.onmessage = obs.next.bind(obs);
                ws.onerror = obs.error.bind(obs);
                ws.onclose = obs.complete.bind(obs);

                return ws.close.bind(ws);
            }
        );

        let observer = {
            next: (data: WsMessage) => {
                if (ws.readyState === WebSocket.OPEN) {
                    ws.send(JSON.stringify(data));
                } else if (ws.readyState === WebSocket.CONNECTING) {
                    this.pendingData.push(JSON.stringify(data));
                }
            },
        };

        this.webSocket = ws;

        return Subject.create(observer, observable.map(ev => JSON.parse(ev.data)));
    }
}
