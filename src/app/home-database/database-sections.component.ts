import {Component} from '@angular/core';
import {GmModeService} from '../shared';

@Component({
    templateUrl: './database-sections.component.html',
    styleUrls: ['./database-sections.component.scss'],
})
export class DatabaseSectionsComponent {
    constructor(public readonly gmModeService: GmModeService) {
    }
}

