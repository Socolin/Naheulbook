import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
    templateUrl: './recovery-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './recovery-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon]
})
export class RecoveryDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
