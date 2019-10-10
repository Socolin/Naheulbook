import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {Skill} from '../skill';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';

export interface SkillInfoDialogData {
    skill: Skill;
}

@Component({
    templateUrl: './skill-info-dialog.component.html',
    styleUrls: ['./skill-info-dialog.component.scss']
})
export class SkillInfoDialogComponent implements OnInit {
    public viewGmSkillInfo = false;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: SkillInfoDialogData
    ) {
    }

    ngOnInit() {
    }

}
