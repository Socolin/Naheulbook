import {Component, Input, ViewChild} from '@angular/core';

import {NEvent} from './event.model';
import {EventService} from './event.service';
import {EventEditorComponent} from './event-editor.component';
import { MatCard, MatCardHeader, MatCardTitleGroup, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { NhbkDateDurationPipe } from '../date/nhbk-duration.pipe';

@Component({
    selector: 'events',
    styleUrls: ['./events.component.scss'],
    templateUrl: './events.component.html',
    imports: [EventEditorComponent, MatCard, MatCardHeader, MatCardTitleGroup, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions, MatButton, NhbkDateDurationPipe]
})
export class EventsComponent {
    @Input() group: {
        id: number,
        data: any,
        pastEventCount: number,
        futureEventCount: number,
        events: NEvent[],
        addEvent(event: NEvent): void,
        removeEvent(eventId: number): void,
    };

    @ViewChild('eventEditor', {static: true})
    private eventEditorComponent: EventEditorComponent;

    constructor(private  _eventService: EventService) {
    }

    public openAddEventDialog() {
        this.eventEditorComponent.openDialog();
    }

    createEvent(eventData: NEvent): boolean {
        this._eventService.createEvent(this.group.id, eventData).subscribe((event: NEvent) => {
            this.group.addEvent(event)
        });
        return false;
    }

    deleteEvent(event: NEvent): boolean {
        this._eventService.deleteEvent(this.group.id, event.id).subscribe(() => {
            this.group.removeEvent(event.id)
        });
        return false;
    }
}
