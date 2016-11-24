import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {QuestService} from './quest.service';
import {QuestTemplate} from './quest.model';

@Component({
    templateUrl: 'create-quest-template.component.html'
})
export class CreateQuestTemplateComponent {
    public quest: QuestTemplate = new QuestTemplate();

    constructor(private _questService: QuestService
        , private router: Router) {
    }

    create(quest: QuestTemplate) {
        this._questService.createQuestTemplate(quest).subscribe(
            () => {
                this.router.navigate(['/database/quests/']);
            }
        );
    }
}
