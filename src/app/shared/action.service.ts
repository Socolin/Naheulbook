import {Injectable} from '@angular/core';
import {Observer, Observable} from 'rxjs';

@Injectable()
export class ActionService<T> {
    private actionObservers: {[actionName: string]: Observer<{element: T, data: any}>} = {};
    private actionObservable: {[actionName: string]: Observable<{element: T, data: any}>} = {};

    public registerAction(action: string): Observable<{element: T, data: any}> {
        if (action in this.actionObservable) {
            return this.actionObservable[action];
        }
        this.actionObservable[action] = Observable.create((function (observer) {
            this.actionObservers[action] = observer;
        }).bind(this));
        return this.actionObservable[action];
    }

    public emitAction(actionName: string, element: T, data?: any) {
        if (!(actionName in this.actionObservers)) {
            throw new Error('action: `' + actionName + '\' was not registered');
        }
        this.actionObservers[actionName].next({element: element, data: data});
        return false;
    }
}
