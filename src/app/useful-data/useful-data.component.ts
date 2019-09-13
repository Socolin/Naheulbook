import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {Portal} from '@angular/cdk/portal';
import {UsefulDataService} from './useful-data.service';
import {MatDialog} from '@angular/material/dialog';
import {EpicFailsDialogComponent} from './dialogs/epic-fails-dialog.component';

@Component({
    selector: 'useful-data',
    styleUrls: ['./useful-data.component.scss'],
    templateUrl: './useful-data.component.html',
})
export class UsefulDataComponent implements OnInit {
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();
    public effectsCategoryId = 1;
    public panelByNames: { [name: string]: Portal<any> } = {};

    constructor(
        private usefulDataService: UsefulDataService,
        private dialog: MatDialog
    ) {

    }

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        return false;
    }

    openEpicFailsDialog() {
        this.dialog.open(EpicFailsDialogComponent, {minWidth: '100vw', height: '100vh', autoFocus: false});
    }

    ngOnInit(): void {
        // this.criticData = this.usefulDataService.getCriticalData();
    }
}
