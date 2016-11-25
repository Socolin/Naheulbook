import {Input, Component, OnInit} from '@angular/core';

import {Character} from './character.model';
import {CharacterService} from './character.service';

@Component({
    selector: 'character-history',
    templateUrl: 'character-history.component.html'
})
export class CharacterHistoryComponent implements OnInit {
    @Input() character: Character;

    public historyPage: number = 0;
    public currentDay: string = null;
    public history: any[];
    public loadMore: boolean;

    constructor(private _characterService: CharacterService) {
    }

    loadHistory(next?: boolean) {
        if (!next) {
            this.historyPage = 0;
            this.currentDay = null;
            this.history = [];
        }

        this._characterService.loadHistory(this.character.id, this.historyPage).subscribe(
            res => {
                if (res.length === 0) {
                    this.loadMore = false;
                    return;
                }
                this.loadMore = true;
                let logs = [];
                if (this.currentDay) {
                    logs = this.history[this.history.length - 1].logs;
                }
                for (let i = 0; i < res.length; i++) {
                    let l = res[i];
                    l.date = new Date(l.date);

                    let day = l.date.toString().substring(0, 15);
                    if (!this.currentDay || day !== this.currentDay) {
                        this.currentDay = day;
                        logs = [];
                        this.history.push({logs: logs, date: l.date});
                    }
                    logs.push(l);
                }
            }
        );
        this.historyPage++;
        return false;
    }

    ngOnInit(): void {
        this.loadHistory();
    }
}
