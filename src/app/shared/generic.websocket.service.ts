import {Observer, Observable} from "rxjs";
import {NotificationsService} from "../notifications/notifications.service";
import {WebSocketService} from "./websocket.service";
import {MiscService} from "./misc.service";

export abstract class GenericWebsocketService {
    private notifyFunc: Function;
    private elementType: string;
    private registeredCallbacks: {[opcode: string]: Observer<any>} = {};

    constructor(private _notification: NotificationsService
        , private _miscService: MiscService
        , private _webSocketService: WebSocketService
        , elementType: string) {
        this.elementType = elementType;
    }

    registerPacket(opcode: string): Observable<any> {
        return Observable.create((function (observer: Observer<any>) {
            this.registeredCallbacks[opcode] = observer;
        }).bind(this));
    }

    notifyChange(message: string) {
        this.notifyFunc(message);
    }

    registerNotifyFunction(notifyFunc: Function) {
        this.notifyFunc = notifyFunc;
    }

    register(elementId: number) {
        this._webSocketService.register(this.elementType, elementId).subscribe(
            res => {
                try {
                    if (res.opcode in this.registeredCallbacks) {
                        this.registeredCallbacks[res.opcode].next(res);
                    }
                    else {
                        console.log("Unhandled websocket opcode: " + res.opcode);
                    }
                }
                catch (err) {
                    this._notification.error("Erreur", "Erreur WS");
                    this._miscService.postJson('/api/debug/report', err).subscribe();
                    console.log(err);
                }
            }
        );
    }

    unregister(elementId: number) {
        this._webSocketService.unregister(this.elementType, elementId);
    }
}
