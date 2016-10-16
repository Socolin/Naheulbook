import {Component, Input, EventEmitter, Output} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {QuestService} from "./quest.service";
import {QuestTemplate} from './quest.model';

@Component({
    selector: 'quest-editor',
    templateUrl: 'quest-editor.component.html'
})
export class QuestEditorComponent {
    @Input() quest: QuestTemplate;
    @Output() onValid: EventEmitter<QuestTemplate> = new EventEmitter<QuestTemplate>();

    constructor(private _questService: QuestService
        , private _notification: NotificationsService
        , private router: Router) {
    }
}
