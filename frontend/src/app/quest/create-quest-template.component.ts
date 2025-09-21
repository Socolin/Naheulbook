import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {QuestService} from './quest.service';
import {QuestTemplate} from './quest.model';
import { QuestEditorComponent } from './quest-editor.component';

@Component({
    templateUrl: './create-quest-template.component.html',
    imports: [QuestEditorComponent]
})
export class CreateQuestTemplateComponent {
    public quest: QuestTemplate = new QuestTemplate();

    constructor(
        private readonly questService: QuestService,
        private readonly router: Router,
    ) {
    }

    create(quest: QuestTemplate) {
        this.questService.createQuestTemplate(quest).subscribe(
            () => {
                this.router.navigate(['/database/quests/']);
            }
        );
    }
}
