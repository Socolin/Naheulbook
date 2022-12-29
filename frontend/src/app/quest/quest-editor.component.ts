import {Component, Input, EventEmitter, Output} from '@angular/core';

import {QuestService} from './quest.service';
import {QuestTemplate} from './quest.model';

@Component({
    selector: 'quest-editor',
    templateUrl: './quest-editor.component.html'
})
export class QuestEditorComponent {
    @Input() quest: QuestTemplate;
    @Output() onValid: EventEmitter<QuestTemplate> = new EventEmitter<QuestTemplate>();

    public previewDescription: boolean;

    constructor(
        private readonly questService: QuestService,
    ) {
    }
}
