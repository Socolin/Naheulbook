import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Origin} from './origin.model';
import {OriginService} from './origin.service';

@Component({
    selector: 'origin-list',
    templateUrl: './origin-list.component.html',
})
export class OriginListComponent implements OnInit {
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
