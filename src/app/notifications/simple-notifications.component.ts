import {Component, OnInit} from '@angular/core';
import {NotificationsService} from './notifications.service';

@Component({
    moduleId: module.id,
    selector: 'simple-notifications',
    templateUrl: 'simple-notifications.component.html',
    styleUrls: ['simple-notifications.component.css']
})
export class SimpleNotificationsComponent implements OnInit {

    constructor(private _notifications: NotificationsService) {
    }

    ngOnInit() {
    }
}
