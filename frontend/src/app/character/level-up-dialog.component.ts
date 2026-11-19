import {Component, Inject, ChangeDetectionStrategy} from '@angular/core';
import {Character} from './character.model';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import {Skill} from '../skill';
import {Job, Speciality} from '../job';
import {Guid} from '../api/shared/util';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatStepper, MatStep, MatStepLabel, MatStepperNext } from '@angular/material/stepper';
import { MatButtonToggleGroup, MatButtonToggle } from '@angular/material/button-toggle';
import { FormsModule } from '@angular/forms';
import {MatFormField, MatHint, MatLabel, MatSuffix} from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { SkillSelectorComponent } from './skill-selector.component';
import { SpecialitySelectorComponent } from './speciality-selector.component';
import {firstValueFrom} from 'rxjs';
import {NhbkMatDialog} from '../material-workaround';
import {MasteryResponse, TalentResponse} from '../api/responses';
import {TalentService} from '../talent/talent.service';
import {MasteryService} from '../mastery/mastery.service';
import {getTalentPrerequisiteId, toTalentLearnableGroups} from '../talent/character-talent-panel.component';
import {getMasteryPrerequisiteId, toMasteryLearnableGroups} from '../mastery/character-mastery-panel.component';
import {buildCandidates, Candidates} from '../shared/entry-group-tabs/entry-candidates';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';

/**
 * At each odd level from level 3 (3, 5, 7...), the character gets 1 point to learn either a talent or a mastery
 */
export function givesTalentOrMasteryPoint(level: number): boolean {
    return level >= 3 && level % 2 === 1;
}

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
    talentId?: Guid;
    masteryId?: Guid;
}

export class LevelUpInfo {
    evOrEa: 'EV' | 'EA' = 'EV';
    evOrEaValue?: number;
    targetLevelUp: number;
    statToUp?: string;
    skill?: Skill;
    specialities: { [jobId: string]: Speciality } = {};
    /** What the talent or mastery point is spent on */
    pointKind: 'talent' | 'mastery' = 'talent';
    talent?: TalentResponse;
    mastery?: MasteryResponse;
}

@Component({
    templateUrl: './level-up-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './level-up-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatLabel, MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatStepper, MatStep, MatStepLabel, MatButtonToggleGroup, FormsModule, MatButtonToggle, MatFormField, MatInput, MatHint, MatSuffix, MatButton, MatStepperNext, SkillSelectorComponent, SpecialitySelectorComponent]
})
export class LevelUpDialogComponent {
    public levelUpInfo: LevelUpInfo = new LevelUpInfo();

    constructor(
        private readonly dialogRef: MatDialogRef<LevelUpDialogComponent, LevelUpDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: LevelUpDialogData,
        private readonly dialog: NhbkMatDialog,
        private readonly talentService: TalentService,
        private readonly masteryService: MasteryService,
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

    levelUpShouldChooseTalentOrMastery(): boolean {
        return givesTalentOrMasteryPoint(this.levelUpInfo.targetLevelUp);
    }

    get chosenTalentOrMastery(): TalentResponse | MasteryResponse | undefined {
        return this.levelUpInfo.pointKind === 'talent' ? this.levelUpInfo.talent : this.levelUpInfo.mastery;
    }

    changePointKind(kind: 'talent' | 'mastery') {
        this.levelUpInfo.pointKind = kind;
        this.levelUpInfo.talent = undefined;
        this.levelUpInfo.mastery = undefined;
    }

    clearTalentOrMastery() {
        this.levelUpInfo.talent = undefined;
        this.levelUpInfo.mastery = undefined;
    }

    async chooseTalentOrMastery() {
        const kind = this.levelUpInfo.pointKind;
        // Conditions are checked against the level the character is reaching
        const level = this.levelUpInfo.targetLevelUp;
        let candidates: Candidates;
        let all: (TalentResponse | MasteryResponse)[];
        if (kind === 'talent') {
            const groups = await firstValueFrom(this.talentService.getTalentGroups());
            const knownIds = new Set(this.data.character.talents.map(x => x.id));
            candidates = buildCandidates(toTalentLearnableGroups(groups), knownIds, level, getTalentPrerequisiteId);
            all = groups.flatMap(g => g.talents);
        } else {
            const groups = await firstValueFrom(this.masteryService.getMasteryGroups());
            const knownIds = new Set(this.data.character.masteries.map(x => x.id));
            candidates = buildCandidates(toMasteryLearnableGroups(groups), knownIds, level, getMasteryPrerequisiteId);
            all = groups.flatMap(g => g.masteries);
        }

        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {data: {title: kind === 'talent' ? 'Choisir un talent' : 'Choisir une maîtrise', singleChoice: true, ...candidates}}
        );
        const result = await firstValueFrom(dialogRef.afterClosed());
        const chosen = all.find(x => x.id === result?.selectedIds[0]);
        if (!chosen || kind !== this.levelUpInfo.pointKind) {
            return;
        }
        if (kind === 'talent') {
            this.levelUpInfo.talent = chosen as TalentResponse;
        } else {
            this.levelUpInfo.mastery = chosen as MasteryResponse;
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
        if (this.levelUpShouldChooseTalentOrMastery() && !this.chosenTalentOrMastery) {
            return false;
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
            talentId: this.levelUpShouldChooseTalentOrMastery() ? this.levelUpInfo.talent?.id : undefined,
            masteryId: this.levelUpShouldChooseTalentOrMastery() ? this.levelUpInfo.mastery?.id : undefined,
        })

    }
}
