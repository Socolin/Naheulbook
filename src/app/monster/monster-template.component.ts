import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTraitDictionary} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MatDialog} from '@angular/material/dialog';
import {
    EditMonsterTemplateDialogComponent,
    EditMonsterTemplateDialogData
} from './edit-monster-template-dialog.component';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {ItemTemplate} from '../item-template';

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html'
})
export class MonsterTemplateComponent implements OnInit {
    @Input() monsterTemplate: MonsterTemplate;
    @Input() isAdmin: boolean;
    @Output() edit: EventEmitter<MonsterTemplate> = new EventEmitter<MonsterTemplate>();
    public traisById?: MonsterTraitDictionary;

    constructor(
        private readonly dialog: MatDialog,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly notifications: NotificationsService,
    ) {
    }

    openEditMonsterDialog(): void {
        const dialogRef = this.dialog.open<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData, MonsterTemplate>(
            EditMonsterTemplateDialogComponent, {
                data: {
                    monsterTemplate: this.monsterTemplate
                },
                minWidth: '100vw', height: '100vh',
                autoFocus: false
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
            panelClass: 'app-dialog-no-padding',
            data: {itemTemplate},
            autoFocus: false
        });
    }
}
