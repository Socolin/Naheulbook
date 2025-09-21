import { Component, OnInit } from '@angular/core';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';

@Component({
    selector: 'app-add-loot-dialog',
    templateUrl: './add-loot-dialog.component.html',
    styleUrls: ['./add-loot-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatFormField, MatLabel, MatInput, MatDialogActions, MatButton, MatDialogClose]
})
export class AddLootDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
