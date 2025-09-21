import {Component, EventEmitter, Input, Output} from '@angular/core';

import {QuestService} from './quest.service';
import {QuestTemplate} from './quest.model';
import { FormsModule } from '@angular/forms';
import { TextareaAutosizeDirective } from '../shared/textarea-autosize.directive';
import { TextFormatterPipe } from '../shared/text-formatter.pipe';

@Component({
    selector: 'quest-editor',
    templateUrl: './quest-editor.component.html',
    imports: [FormsModule, TextareaAutosizeDirective, TextFormatterPipe]
})
export class QuestEditorComponent {
    @Input() quest: QuestTemplate;
    @Output() valid: EventEmitter<QuestTemplate> = new EventEmitter<QuestTemplate>();

    public previewDescription: boolean;

    constructor(
        private readonly questService: QuestService,
    ) {
    }
}
