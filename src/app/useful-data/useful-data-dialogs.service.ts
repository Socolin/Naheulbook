import {Injectable} from '@angular/core';
import {Subject, Subscription} from 'rxjs';

export type PanelNames =
    'criticalSuccess'
    | 'epicFails'
    | 'effects'
    | 'entropicSpells'
    | 'sleep'
    | 'skills'
    | 'items'
    | 'jobs'
    | 'origins';

@Injectable()
export class UsefulDataDialogsService {
    public openPanelAction: Subject<{ panelName: string, arg?: any }> = new Subject<{ panelName: string, arg?: any }>();

    public openPanel(panelName: PanelNames, arg?: any): void {
        this.openPanelAction.next({panelName, arg});
    }

    public onOpenPanel(cb: (panelName: string, arg?: any) => void): Subscription {
        return this.openPanelAction.subscribe(action => {
            cb(action.panelName, action.arg);
        });
    }
}
