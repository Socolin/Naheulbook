import {Component, Inject} from '@angular/core';
import {Character} from './character.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {Skill} from '../skill';
import {Job, Speciality} from '../job';
import {Guid} from '../api/shared/util';

export interface LevelUpDialogData {
    readonly character: Character;
}

export interface LevelUpDialogResult {
    evOrEa: 'EV' | 'EA';
    evOrEaValue: number;
    targetLevelUp: number;
    statToUp: string;
    skillId?: Guid;
    specialityIds: Guid[];
}

export class LevelUpInfo {
    evOrEa: 'EV' | 'EA' = 'EV';
    evOrEaValue?: number;
    targetLevelUp: number;
    statToUp?: string;
    skill?: Skill;
    specialities: { [jobId: string]: Speciality } = {};
}

@Component({
    templateUrl: './level-up-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './level-up-dialog.component.scss']
})
export class LevelUpDialogComponent {
    public levelUpInfo: LevelUpInfo = new LevelUpInfo();

    constructor(
        private readonly dialogRef: MatDialogRef<LevelUpDialogComponent, LevelUpDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: LevelUpDialogData,
    ) {
        this.initLevelUp();
    }

    characterHasToken(flagName: string) {
        if (this.data.character.origin.hasFlag(flagName)) {
            return true
        }
        for (let job of this.data.character.jobs) {
            if (job.hasFlag(flagName)) {
                return true;
            }
        }
        if (this.data.character.specialities) {
            for (let speciality of this.data.character.specialities) {
                if (speciality.hasFlag(flagName)) {
                    return true;
                }
            }
        }
        return false;
    }


    initLevelUp() {
        this.levelUpInfo = new LevelUpInfo();
        this.levelUpInfo.evOrEa = 'EV';
        this.levelUpInfo.evOrEaValue = undefined;
        this.levelUpInfo.targetLevelUp = this.data.character.level + 1;
        this.levelUpInfo.statToUp = undefined;
    }

    rollLevelUp() {
        let diceLevelUp = this.data.character.origin.data.diceEvLevelUp;
        if (this.levelUpInfo.evOrEa === 'EV') {
            if (this.characterHasToken('LEVELUP_DICE_EV_-1')) {
                this.levelUpInfo.evOrEaValue = Math.max(1, Math.ceil(Math.random() * diceLevelUp) - 1);
                return;
            }
        } else {
            let job = this.data.character.jobs.find(j => !!j.getStatData(this.data.character.origin).diceEaLevelUp);
            if (job) {
                diceLevelUp = job.getStatData(this.data.character.origin).diceEaLevelUp!;
            } else {
                diceLevelUp = 6;
            }
        }
        this.levelUpInfo.evOrEaValue = Math.ceil(Math.random() * diceLevelUp);
    }

    onLevelUpSelectSkills(skills: Skill[]) {
        this.levelUpInfo.skill = skills[0];
    }

    levelUpShouldSelectSkill() {
        return this.levelUpInfo.targetLevelUp === 3
            || this.levelUpInfo.targetLevelUp === 6
            || this.levelUpInfo.targetLevelUp === 10;
    }
    levelUpShouldSelectSpeciality(job: Job): boolean {
        if (!job.specialities) {
            return false;
        }
        let specialities = this.data.character.getJobsSpecialities(job);
        for (let speciality of specialities) {
            if (speciality.hasFlag('ONE_SPECIALITY')) {
                return false;
            }
        }
        return job.hasFlag('SELECT_SPECIALITY_LVL_5_10')
            && !job.hasFlag('ONE_SPECIALITY')
            && (this.levelUpInfo.targetLevelUp === 5 || this.levelUpInfo.targetLevelUp === 10);
    }

    levelUpSelectSpeciality(job: Job, speciality: Speciality) {
        if (this.levelUpShouldSelectSpeciality(job)) {
            this.levelUpInfo.specialities[job.id] = speciality;
        }
    }

    levelUpFormReady() {
        if (!this.levelUpInfo.evOrEaValue) {
            return false;
        }
        for (let job of this.data.character.jobs) {
            if (this.levelUpShouldSelectSpeciality(job)) {
                if (this.levelUpInfo.specialities[job.id] == null) {
                    return false;
                }
            }
        }
        if (this.levelUpShouldSelectSkill()) {
            if (!this.levelUpInfo.skill) {
                return false;
            }
        }
        return true;
    }

    levelUp() {
        if (!this.levelUpInfo.evOrEaValue) {
            return;
        }
        if (!this.levelUpInfo.statToUp) {
            return;
        }

        let skillId: Guid | undefined = undefined;
        if (this.levelUpInfo.skill) {
            skillId = this.levelUpInfo.skill.id;
        }

        let specialityIds: Guid[] = [];
        for (let jobId in this.levelUpInfo.specialities) {
            if (!this.levelUpInfo.specialities.hasOwnProperty(jobId)) {
                continue;
            }
            const speciality = this.levelUpInfo.specialities[jobId];
            if (!speciality) {
                continue;
            }
            specialityIds.push(speciality.id);
        }

        this.dialogRef.close({
            evOrEa: this.levelUpInfo.evOrEa,
            evOrEaValue: this.levelUpInfo.evOrEaValue,
            targetLevelUp: this.levelUpInfo.targetLevelUp,
            statToUp: this.levelUpInfo.statToUp,
            skillId: skillId,
            specialityIds: specialityIds,
        })

    }
}
