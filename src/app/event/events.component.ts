import {Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild} from '@angular/core';

import {Group} from '../group/group.model';
import {NEvent} from './event.model';
import {EventService} from './event.service';
import {date2Timestamp} from '../date/util';
import {GroupWebsocketService} from '../group/group.websocket.service';
import {GroupActionService} from '../group/group-action.service';
import {EventEditorComponent} from './event-editor.component';

@Component({
    selector: 'events',
    styleUrls: ['./events.component.scss'],
    templateUrl: './events.component.html',
})
export class EventsComponent implements OnInit, OnChanges {
    @Input() group: Group;
    public events: NEvent[] = [];
    public groupTimestamp: number = 0;
    public pastEventCount: number = 0;
    public futureEventCount: number = 0;

    @ViewChild('eventEditor')
    private eventEditorComponent: EventEditorComponent;

    constructor(private  _eventService: EventService
        , private _actionService: GroupActionService
        , private _groupWebsocketService: GroupWebsocketService) {
    }

    public openAddEventDialog() {
        this.eventEditorComponent.openDialog();
    }

    createEvent(event: NEvent) {
        this._eventService.createEvent(this.group.id, event).subscribe(this.onAddEvent.bind(this));
    }

    deleteEvent(event: NEvent) {
        this._eventService.deleteEvent(event.id).subscribe(this.onDeleteEvent.bind(this));
        return false;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('group' in changes) {
            if (this.group.data.date) {
                this.groupTimestamp = date2Timestamp(this.group.data.date);
            }
        }
    }

    onDeleteEvent(event: NEvent) {
        let index = this.events.findIndex(e => e.id === event.id);
        if (index !== -1) {
            this.events.splice(index, 1);
        }
        this.sortEvents();
    }
    onAddEvent(event: NEvent) {
        let index = this.events.findIndex(e => e.id === event.id);
        if (index === -1) {
            this.events.push(event);
        }
        this.sortEvents();
    }

    sortEvents(): void {
        this.events.sort((a: NEvent, b: NEvent) => {
           return a.timestamp - b.timestamp;
        });
        this.countEvents();
    }

    countEvents() {
        this.pastEventCount = 0;
        this.futureEventCount = 0;
        for (let i = 0; i < this.events.length; i++) {
            let event = this.events[i];
            if (event.timestamp <= this.groupTimestamp) {
                this.pastEventCount++;
            } else {
                this.futureEventCount++;
            }
        }
    }

    registerActions() {
        this._groupWebsocketService.registerPacket('addEvent').subscribe(this.onAddEvent.bind(this));
        this._groupWebsocketService.registerPacket('deleteEvent').subscribe(this.onDeleteEvent.bind(this));
        this._actionService.registerAction('dateChanged').subscribe(
            (data) => {
                this.groupTimestamp = date2Timestamp(data.element.data.date);
                this.countEvents();
            }
        );
    }

    ngOnInit(): void {
        if (this.group.data.date) {
            this.groupTimestamp = date2Timestamp(this.group.data.date);
        }
        this._eventService.loadEvents(this.group.id).subscribe(
            events => {
                this.events = events;
                this.sortEvents();
            }
        );
        this.registerActions();
    }
}
