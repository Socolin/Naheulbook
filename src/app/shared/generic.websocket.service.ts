import {Observer, Observable} from "rxjs";
import {NotificationsService} from "../notifications/notifications.service";
import {WebSocketService} from "./websocket.service";
import {MiscService} from "./misc.service";
import {WsEvent} from "./websocket.model";

export abstract class GenericWebsocketService {
    private notifyFunc: Function;
    private elementType: string;
    private registeredObservers: {[opcode: string]: Observer<WsEvent>} = {};
    private registeredObservables: {[opcode: string]: Observable<WsEvent>} = {};

    constructor(private _notification: NotificationsService
        , private _miscService: MiscService
        , private _webSocketService: WebSocketService
        , elementType: string) {
        this.elementType = elementType;
    }

    registerPacket(opcode: string): Observable<WsEvent> {
        if (opcode in this.registeredObservables) {
            return this.registeredObservables[opcode];
        }
        let observable = Observable.create((function (observer: Observer<WsEvent>) {
            this.registeredObservers[opcode] = observer;
        }).bind(this)).share();
        this.registeredObservables[opcode] = observable;
        return observable;
    }

    notifyChange(message: string) {
        if (this.notifyFunc) {
            this.notifyFunc(message);
        }
    }

    registerNotifyFunction(notifyFunc: Function) {
        this.notifyFunc = notifyFunc;
    }

    register(elementId: number) {
        this._webSocketService.register(this.elementType, elementId).subscribe(
            res => {
                try {
                    if (res.opcode in this.registeredObservers) {
                        this.registeredObservers[res.opcode].next(res);
                    }
                    else {
                        console.log("Unhandled websocket opcode: " + res.opcode);
                    }
                }
                catch (err) {
                    this._notification.error("Erreur", "Erreur WS");
                    this._miscService.postJson('/api/debug/report', err, true).subscribe();
                    console.log(err);
                }
            }
        );
    }

    unregister(elementId: number) {
        this._webSocketService.unregister(this.elementType, elementId);
    }
}
