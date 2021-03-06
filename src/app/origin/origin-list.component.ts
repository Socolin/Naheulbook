import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Origin} from './origin.model';
import {OriginService} from './origin.service';

@Component({
    selector: 'origin-list',
    templateUrl: './origin-list.component.html',
    styleUrls: ['./origin-list.component.scss'],
})
export class OriginListComponent implements OnInit {
    public origins: Origin[];

    constructor(
        private readonly originService: OriginService,
        private readonly notification: NotificationsService
    ) {
    }

    getOrigins() {
        this.originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
            }
        );
    }

    ngOnInit() {
        this.getOrigins();
    }
}
