import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTraitDictionary} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData} from './edit-monster-template-dialog.component';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {ItemTemplate} from '../item-template';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html',
    standalone: false
})
export class MonsterTemplateComponent implements OnInit {
    @Input() monsterTemplate: MonsterTemplate;
    @Input() isAdmin: boolean;
    @Output() edit: EventEmitter<MonsterTemplate> = new EventEmitter<MonsterTemplate>();
    public traisById?: MonsterTraitDictionary;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly notifications: NotificationsService,
    ) {
    }

    openEditMonsterDialog(): void {
        const dialogRef = this.dialog.openFullScreen<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData, MonsterTemplate>(
            EditMonsterTemplateDialogComponent, {
                data: {
                    monsterTemplate: this.monsterTemplate
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.notifications.success('Monster', 'Monstre éditer');
            this.edit.next(result);
        });
    }

    ngOnInit(): void {
        this.monsterTemplateService.getMonsterTraitsById().subscribe(
            traitsById => {
                this.traisById = traitsById
            }
        );
    }

    openItemInfo(itemTemplate: ItemTemplate) {
        this.dialog.open(ItemTemplateDialogComponent, {
            data: {itemTemplate},
            autoFocus: false
        });
    }
}
