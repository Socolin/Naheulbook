import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {NotificationsService} from './notifications.service';

@Component({
    selector: 'simple-notifications',
    templateUrl: './simple-notifications.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrls: ['./simple-notifications.component.css']
})
export class SimpleNotificationsComponent implements OnInit {

    constructor(public _notifications: NotificationsService) {
    }

    ngOnInit() {
    }
}
