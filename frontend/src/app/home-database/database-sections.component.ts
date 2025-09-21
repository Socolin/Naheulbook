import {Component} from '@angular/core';
import {GmModeService} from '../shared';
import { MatIcon } from '@angular/material/icon';
import { MatCard } from '@angular/material/card';
import { MatNavList, MatListItem, MatListItemIcon } from '@angular/material/list';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';

@Component({
    templateUrl: './database-sections.component.html',
    styleUrls: ['./database-sections.component.scss'],
    imports: [MatIcon, MatCard, MatNavList, MatListItem, RouterLink, MatListItemIcon, AsyncPipe]
})
export class DatabaseSectionsComponent {
    constructor(public readonly gmModeService: GmModeService) {
    }
}

