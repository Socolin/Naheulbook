import {Component, Inject, OnInit} from '@angular/core';
import {Skill} from '../skill';
import { MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { NgIf } from '@angular/common';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

export interface SkillInfoDialogData {
    skill: Skill;
    canViewGmSkillInfo: boolean;
}

@Component({
    templateUrl: './skill-info-dialog.component.html',
    styleUrls: ['./skill-info-dialog.component.scss'],
    imports: [MatDialogTitle, NgIf, CdkScrollable, MatDialogContent, MatDialogActions, MatButton, MatDialogClose]
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
