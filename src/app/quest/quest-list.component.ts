import {Component, OnInit} from '@angular/core';

import {QuestTemplate} from "./quest.model";
import {QuestService} from "./quest.service";

@Component({
    selector: 'quest-list',
    templateUrl: 'quest-list.component.html'
})
export class QuestListComponent implements OnInit {
    public quests: QuestTemplate[];

    constructor(private _questService: QuestService) {
    }

    getQuests() {
        this._questService.getQuestList().subscribe(
            res => {
                this.quests = res;
            }
        );
    }

    ngOnInit() {
        this.getQuests();
    }
}
