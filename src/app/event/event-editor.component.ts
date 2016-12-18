import {Component, Output, EventEmitter, Input, OnInit} from '@angular/core';
import {NEvent} from './event.model';
import {NhbkDateOffset, NhbkDate} from '../date/date.model';
import {Group} from '../group/group.model';
import {dateOffset2TimeDuration, date2Timestamp, timestamp2Date} from '../date/util';

@Component({
    selector: 'event-editor',
    templateUrl: './event-editor.component.html',
})
export class EventEditorComponent implements OnInit {
    @Input() group: Group;
    @Output() onCreate: EventEmitter<NEvent> = new EventEmitter<NEvent>();
    @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

    public event: NEvent = new NEvent();
    public durationType: string = 'offset';
    public durationDateOffset: NhbkDateOffset = new NhbkDateOffset();
    public durationOffset: number = 0;
    public eventDate: NhbkDate = new NhbkDate();

    closeSelector() {
        this.onClose.emit(true);
    }

    setDurationType(type: string) {
        this.durationType = type;
    }

    setTimeDuration(dateOffset: NhbkDateOffset) {
        if (this.durationType !== 'offset') {
            throw new Error('Try to set time duration while durationType is not `time` but is `' + this.durationType + '`');
        }

        let groupTimestamp = date2Timestamp(this.group.data.date);
        this.durationOffset = dateOffset2TimeDuration(dateOffset);
        this.event.timestamp = groupTimestamp + this.durationOffset;
        this.eventDate = timestamp2Date(this.event.timestamp);
    }

    setDate(date: NhbkDate) {
        let groupTimestamp = date2Timestamp(this.group.data.date);
        let timestamp = date2Timestamp(date);
        this.durationOffset = timestamp - groupTimestamp;
        this.event.timestamp = timestamp;
        this.eventDate = date;
    }

    createEvent() {
        if (!this.event.timestamp) {
            return;
        }
        this.onCreate.emit(this.event);
    }

    ngOnInit(): void {
        this.eventDate = this.group.data.date;
    }
}
