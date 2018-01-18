import {Component, Input, ViewChild} from '@angular/core';

import {NEvent} from './event.model';
import {EventService} from './event.service';
import {EventEditorComponent} from './event-editor.component';

@Component({
    selector: 'events',
    styleUrls: ['./events.component.scss'],
    templateUrl: './events.component.html',
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

    @ViewChild('eventEditor')
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
        this._eventService.deleteEvent(event.id).subscribe(deletedEvent => {
            this.group.removeEvent(deletedEvent.id)
        });
        return false;
    }
}
