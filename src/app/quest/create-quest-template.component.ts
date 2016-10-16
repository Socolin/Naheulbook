import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {QuestService} from "./quest.service";
import {QuestTemplate} from './quest.model';
import {QuestEditorComponent} from './quest-editor.component';

@Component({
    templateUrl: 'create-quest-template.component.html'
})
export class CreateQuestTemplateComponent {
    public quest: QuestTemplate = new QuestTemplate();

    constructor(private _questService: QuestService
        , private _notification: NotificationsService
        , private router: Router) {
    }

    create(quest: QuestTemplate) {
        this._questService.createQuestTemplate(quest).subscribe(
            () => {
                this.router.navigate(['/database/quests/']);
            },
            err => {
                console.log(err);
                if (err.status === 500) {
                    this._notification.error("Erreur", "Erreur serveur");
                } else {
                    this._notification.error("Erreur", "Erreur requête");
                }
            }
        );
    }
}
