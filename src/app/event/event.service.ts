
import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

import {NEvent} from './event.model';

@Injectable()
export class EventService {
    constructor(private httpClient: HttpClient) {
    }

    loadEvents(groupId: number): Observable<NEvent[]> {
        return this.httpClient.get<NEvent[]>(`/api/v2/groups/${groupId}/events`)
            .pipe(map(res => NEvent.fromResponses(res)));
    }

    createEvent(groupId: number, event: NEvent): Observable<NEvent> {
        return this.httpClient.post<NEvent>(`/api/v2/groups/${groupId}/events`, event).pipe(map(res => NEvent.fromResponse(res)));
    }

    deleteEvent(groupId: number, eventId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/groups/${groupId}/events/${eventId}`);
    }
}
