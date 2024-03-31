import {Component, Inject, OnInit} from '@angular/core';
import {Skill} from '../skill';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA} from '@angular/material/legacy-dialog';

export interface SkillInfoDialogData {
    skill: Skill;
    canViewGmSkillInfo: boolean;
}

@Component({
    templateUrl: './skill-info-dialog.component.html',
    styleUrls: ['./skill-info-dialog.component.scss']
})
export class SkillInfoDialogComponent implements OnInit {
    public viewGmSkillInfo = false;
    public canViewGmSkillInfo: boolean;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: SkillInfoDialogData
    ) {
        this.canViewGmSkillInfo = data.canViewGmSkillInfo;
    }

    ngOnInit() {
    }

}
