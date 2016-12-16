import {Input, Component, OnInit} from '@angular/core';
import {Group} from './group.model';
import {GroupService} from './group.service';
import {NotificationsService} from '../notifications/notifications.service';

@Component({
    selector: 'group-history',
    templateUrl: './group-history.component.html'
})
export class GroupHistoryComponent implements OnInit {
    @Input() group: Group;

    public historyPage: number = 0;
    public currentDay: string = null;
    public history: any[];
    public loadMore: boolean = true;

    public historyNewEntryText: string = null;
    public historyNewEntryGm: boolean = false;

    constructor(private _groupService: GroupService, private _notification: NotificationsService) {
    }

    loadHistory(next?: boolean) {
        if (!next) {
            this.historyPage = 0;
            this.currentDay = null;
            this.history = [];
        }

        this._groupService.loadHistory(this.group.id, this.historyPage).subscribe(
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

    addLog() {
        this._groupService.addLog(this.group.id, this.historyNewEntryText, this.historyNewEntryGm).subscribe(
            () => {
                this.historyNewEntryText = null;
                this._notification.success('Historique', 'Entrée ajoutée');
                this.loadHistory();
            }
        );
        return false;
    }

    ngOnInit(): void {
        this.loadHistory();
    }
}
