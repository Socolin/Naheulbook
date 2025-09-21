import { Component, OnInit } from '@angular/core';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatDialogClose } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SkillListComponent } from '../../skill/skill-list.component';

@Component({
    templateUrl: './skills-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './skills-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, SkillListComponent]
})
export class SkillsDialogComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
