import {Component, Input, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTraitDictionary} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MatDialog} from '@angular/material/dialog';
import {
    EditMonsterTemplateDialogComponent,
    EditMonsterTemplateDialogData
} from './edit-monster-template-dialog.component';

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html'
})
export class MonsterTemplateComponent implements OnInit {
    @Input() monsterTemplate: MonsterTemplate;
    public traisById?: MonsterTraitDictionary;

    constructor(
        private dialog: MatDialog,
        private monsterTemplateService: MonsterTemplateService,
        private notifications: NotificationsService,
    ) {
    }

    openEditMonsterDialog(): void {
        const dialogRef = this.dialog.open<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData>(
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
        });
    }

    ngOnInit(): void {
        this.monsterTemplateService.getMonsterTraitsById().subscribe(
            traitsById => {
                this.traisById = traitsById
            }
        );
    }
}
