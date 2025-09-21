import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import { Portal, CdkPortal } from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {DurationSelectorDialogComponent, DurationSelectorDialogData, DurationSelectorDialogResult, NhbkDate, NhbkDateOffset} from '../date';
import {date2Timestamp, dateOffset2TimeDuration, timestamp2Date} from '../date/util';

import {NEvent} from './event.model';
import {NhbkMatDialog} from '../material-workaround';
import {DateSelectorDialogComponent, DateSelectorDialogData, DateSelectorDialogResult} from '../date/date-selector-dialog.component';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButtonToggleGroup, MatButtonToggle } from '@angular/material/button-toggle';
import { DateComponent } from '../date/date.component';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { NhbkDateDurationPipe } from '../date/nhbk-duration.pipe';

@Component({
    selector: 'event-editor',
    templateUrl: './event-editor.component.html',
    imports: [CdkPortal, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatFormField, MatInput, FormsModule, MatButtonToggleGroup, MatButtonToggle, DateComponent, MatIconButton, MatIcon, MatCardActions, MatButton, NhbkDateDurationPipe]
})
export class EventEditorComponent implements OnInit {
    @Input() group: {
        id: number,
        data: any;
        addEvent(event: NEvent): void,
        removeEvent(eventId: number): void,
    };
    @Output() create: EventEmitter<NEvent> = new EventEmitter<NEvent>();
    @Output() closed: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('eventEditorDialog', {static: true})
    public eventEditorDialog: Portal<any>;
    public eventEditorOverlayRef: OverlayRef;

    public event: NEvent = new NEvent();
    public durationType = 'offset';
    public durationDateOffset: NhbkDateOffset = new NhbkDateOffset();
    public durationOffset = 0;
    public eventDate: NhbkDate = new NhbkDate();

    constructor(
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    openDialog() {
        this.eventEditorOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.eventEditorDialog);
    }

    closeDialog() {
        this.eventEditorOverlayRef.detach();
        this.closed.emit(true);
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
        this.closeDialog();
        this.create.emit(this.event);
    }

    ngOnInit(): void {
        this.eventDate = this.group.data.date;
    }

    openDateSelectorDialog() {
        const dialogRef = this.dialog.open<DateSelectorDialogComponent, DateSelectorDialogData, DateSelectorDialogResult>(
            DateSelectorDialogComponent, {
                data: {
                    date: this.eventDate,
                    title: 'Choisir la date de l\'évènement'
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.setDate(result.date);
        })
   }

    openDurationSelector() {
        const dialogRef = this.dialog.open<DurationSelectorDialogComponent, DurationSelectorDialogData, DurationSelectorDialogResult>(
            DurationSelectorDialogComponent,
            {
                autoFocus: false,
                data: {
                    duration: this.durationDateOffset
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.setTimeDuration(result.duration);
        });
    }
}
