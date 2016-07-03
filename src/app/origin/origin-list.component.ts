import {Component} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Origin} from "./origin.model";
import {OriginComponent} from './origin.component';
import {OriginService} from "./origin.service";

@Component({
    selector: 'origin-list',
    templateUrl: 'app/origin/origin-list.component.html',
    styleUrls: ['/styles/origin-list.css'],
    directives: [OriginComponent],
})
export class OriginListComponent {
    public origins: Origin[];

    constructor(private _originService: OriginService
        , private _notification: NotificationsService) {
    }

    getOrigins() {
        this._originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
            },
            error => {
                this._notification.error('Erreur', 'Erreur serveur');
                console.log(error);
            }
        );
    }

    ngOnInit() {
        this.getOrigins();
    }
}
