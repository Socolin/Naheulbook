import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
    templateUrl: './travel-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './travel-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon]
})
export class TravelDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
