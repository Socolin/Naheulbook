import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

@Component({
    selector: 'app-confirm-gm-mode',
    templateUrl: './confirm-gm-mode-dialog.component.html',
    styleUrls: ['./confirm-gm-mode-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatDialogActions, MatButton, MatDialogClose]
})
export class ConfirmGmModeDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }
}
