import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { MonsterListComponent } from './monster-list.component';

@Component({
    templateUrl: './monsters-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './monsters-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MonsterListComponent]
})
export class MonstersDialogComponent implements OnInit {

    constructor() { }

    ngOnInit() {
    }

}
