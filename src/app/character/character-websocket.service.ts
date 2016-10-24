import {Injectable} from "@angular/core";
import {WebSocketService} from "../shared/websocket.service";
import {NotificationsService} from "../notifications/notifications.service";
import {Character} from "./character.model";
import {CharacterService} from "./character.service";
import {Observable, Observer} from "rxjs";

@Injectable()
export class CharacterWebsocketService {
    private notifyFunc: Function;
    private character: Character;
    private registeredCallbacks: {[opcode: string]: Observer<any>} = {};

    constructor(private _notification: NotificationsService
        , private _characterService: CharacterService
        , private _webSocketService: WebSocketService) {
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

    register(character: Character) {
        this.character = character;
        this._webSocketService.register('character', character).subscribe(
            res => {
                try {
                    if (res.opcode in this.registeredCallbacks) {
                        this.registeredCallbacks[res.opcode].next(res.data);
                    }
                    else {
                        console.log("Unhandled websocket opcode: " + res.opcode);
                    }
                }
                catch (err) {
                    this._notification.error("Erreur", "Erreur WS");
                    this._characterService.postJson('/api/debug/report', err).subscribe();
                    console.log(err);
                }
            }
        );
    }

    unregister() {
        this._webSocketService.unregister('character', this.character.id);
    }
}
