import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTraitDictionary} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData} from './edit-monster-template-dialog.component';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {ItemTemplate} from '../item-template';
import {NhbkMatDialog} from '../material-workaround';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent } from '@angular/material/card';
import { MatIconButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { MonsterTraitComponent } from './monster-trait.component';
import { MatRipple } from '@angular/material/core';
import { IconComponent } from '../shared/icon.component';
import { DecimalPipe } from '@angular/common';
import { MarkdownPipe } from '../markdown/markdown.pipe';

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html',
    imports: [MatCard, MatCardHeader, MatCardTitle, MatIconButton, MatMenuTrigger, MatIcon, MatMenu, MatMenuItem, MatCardContent, MonsterTraitComponent, MatRipple, IconComponent, DecimalPipe, MarkdownPipe]
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
