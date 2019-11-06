import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable()
export class GmModeService {
    get gmMode(): Observable<boolean> {
        return this._gmModeSubject;
    };

    get gmModeSnapshot(): boolean {
        return this._gmModeSnapshot;
    }

    private _gmModeSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private _gmModeSnapshot = false;

    constructor() {
        this.setGmMode(localStorage.getItem('gmMode') === '1');
    }

    public setGmMode(active: boolean) {
        this._gmModeSubject.next(active);
        this._gmModeSnapshot = active;
        localStorage.setItem('gmMode', active ? '1' : '0');
    }
}
