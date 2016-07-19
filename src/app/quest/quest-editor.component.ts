import {Component, Input, EventEmitter, Output} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {QuestService} from "./quest.service";
import {QuestTemplate} from './quest.model';
import {TextFormatterPipe, TextareaAutosizeDirective} from '../shared';

@Component({
    moduleId: module.id,
    selector: 'quest-editor',
    pipes: [TextFormatterPipe],
    directives: [TextareaAutosizeDirective],
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
