import {ChangeDetectionStrategy, Component, Input, OnInit} from '@angular/core';

import {Character} from './character.model';
import {CharacterService} from './character.service';
import {
    ActiveStatsModifier,
    getHistoryEntryIcon,
    HistoryEntry,
    HistoryItemData,
    IconComponent,
    ModifierPipe,
    ValueChangePipe
} from '../shared';
import {Item, ItemData} from '../item';
import {ItemDialogComponent, ItemDialogData} from '../item/item-dialog.component';
import {ItemTemplateService} from '../item-template';
import {NhbkMatDialog} from '../material-workaround';
import {NotificationsService} from '../notifications';
import {DateComponent} from '../date/date.component';
import {MatButton} from '@angular/material/button';
import {DatePipe, NgTemplateOutlet} from '@angular/common';
import {NhbkDateDurationPipe} from '../date/nhbk-duration.pipe';

@Component({
    selector: 'character-history',
    templateUrl: './character-history.component.html',
    styleUrls: ['./character-history.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [DateComponent, MatButton, DatePipe, NhbkDateDurationPipe, IconComponent, ModifierPipe, ValueChangePipe, NgTemplateOutlet]
})
export class CharacterHistoryComponent implements OnInit {
    @Input() character: Character;

    public historyPage = 0;
    public currentDay: string | undefined;
    public history: {logs: any[]; date: string}[] = [];

    public readonly getLogIcon = getHistoryEntryIcon;
    public loadMore: boolean;
    public loading: boolean;

    constructor(
        private readonly characterService: CharacterService,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly dialog: NhbkMatDialog,
        private readonly notifications: NotificationsService,
    ) {
    }

    openItem(historyItem: HistoryItemData) {
        this.itemTemplateService.getItem(historyItem.templateId).subscribe({
            next: (template) => {
                const item = new Item();
                item.id = 0;
                item.data = new ItemData(historyItem.data);
                item.modifiers = ActiveStatsModifier.modifiersFromJson(historyItem.modifiers);
                item.template = template;
                this.dialog.open<ItemDialogComponent, ItemDialogData>(ItemDialogComponent, {
                    data: {item, readonly: true},
                    autoFocus: false,
                });
            },
            error: (err) => this.notifications.error('Impossible de charger l\'objet', err),
        });
    }

    loadHistory(next?: boolean) {
        this.loading = true;
        if (!next) {
            this.historyPage = 0;
            this.currentDay = undefined;
            this.history = [];
        }

        this.characterService.loadHistory(this.character.id, this.historyPage).subscribe(
            res => {
                if (res.length === 0) {
                    this.loadMore = false;
                    return;
                }
                this.loadMore = true;
                let logs: HistoryEntry[] = [];
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
                this.loading = false;
            }
        );
        this.historyPage++;
        return false;
    }

    ngOnInit(): void {
        this.loadHistory();
    }
}
