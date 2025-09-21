import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { JobListComponent } from '../../job/job-list.component';

@Component({
    templateUrl: './jobs-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './jobs-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, JobListComponent]
})
export class JobsDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
