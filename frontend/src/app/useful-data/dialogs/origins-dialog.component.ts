import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { OriginListComponent } from '../../origin/origin-list.component';

@Component({
    templateUrl: './origins-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './origins-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, OriginListComponent]
})
export class OriginsDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
