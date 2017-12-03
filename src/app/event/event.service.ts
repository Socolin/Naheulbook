import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs';

import {JsonService} from '../shared';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {NEvent} from './event.model';

@Injectable()
export class EventService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    loadEvents(groupId: number): Observable<NEvent[]> {
        return this.postJson('/api/group/loadEvents', {
            groupId: groupId
        }).map(res => NEvent.eventsFromJson(res.json()));
    }

    createEvent(groupId: number, event: NEvent): Observable<NEvent> {
        return this.postJson('/api/group/createEvent', {
            groupId: groupId,
            event: event
        }).map(res => NEvent.fromJson(res.json()));
    }

    deleteEvent(eventId: number): Observable<NEvent> {
        return this.postJson('/api/group/deleteEvent', {
            eventId: eventId
        }).map(res => res.json());
    }
}
