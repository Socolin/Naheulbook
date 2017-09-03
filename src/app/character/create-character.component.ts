import {Component, OnInit, ViewChild} from '@angular/core';
import {Router} from '@angular/router';

import {getRandomInt} from '../shared/random';

import {Origin, OriginSelectorComponent} from '../origin';
import {Job, JobSelectorComponent, Speciality} from '../job';
import {Skill} from '../skill';

import {CharacterService} from './character.service';
import {SkillSelectorComponent} from './skill-selector.component';

@Component({
    selector: 'create-character',
    templateUrl: './create-character.component.html',
    styleUrls: ['./create-character.component.scss']
})
export class CreateCharacterComponent implements OnInit {
    public step: number;
    public creating = false;

    // Step 0: Statistics

    public cou: number;
    public int: number;
    public cha: number;
    public ad: number;
    public fo: number;

    // Step 1: Select origin

    public selectedOrigin: Origin | undefined;

    @ViewChild(OriginSelectorComponent)
    private originSelectorComponent: OriginSelectorComponent;

    // Step 2: Select job

    public selectedJob: Job | undefined;

    @ViewChild(JobSelectorComponent)
    private jobSelectorComponent: JobSelectorComponent;

    // Step 3: Select skills

    public selectedSkills: Skill[] | undefined;

    @ViewChild(SkillSelectorComponent)
    private skillsSelectorComponent: SkillSelectorComponent;

    // Step 4: Select fortune

    public money = 0;
    public money2 = 0;

    // Step 5: Stats modification due to bonus

    public modifiedStat: {[modifierName: string]: any};

    /// MOVE_1_POINT_STAT
    public move1PointStatValues: {[statName: string]: number};
    /// SUPER_BOURRIN
    public superBourrinValueAt = 0;
    public superBourrinValuePrd = 0;
    /// CHANGE_1_AT_PRD
    public changeAtPrdValue = 0;
    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA
    public removeAttOrPrdToIntOrChaRemoveStat = 'AT';
    public removeAttOrPrdToIntOrChaAddStat = 'INT';
    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD
    public removeAttOrPrdToIntOrAdRemoveStat = 'AT';
    public removeAttOrPrdToIntOrAdAddStat = 'INT';

    // Step 6: Select speciality

    public selectedSpeciality: Speciality | undefined;

    // Step 7: Name

    public name: string | undefined;
    public sex = 'Homme';

    // Step 8: FatePoint

    public fatePoint: number | undefined;

    constructor(private _router: Router
        , private _characterService: CharacterService) {
        this.setStep(0);
    }

    saveStats() {
        localStorage.setItem('savedStats', JSON.stringify({
            cou: this.cou,
            int: this.int,
            cha: this.cha,
            ad: this.ad,
            fo: this.fo,
        }));
    }

    clearSavedStats() {
        localStorage.removeItem('savedStats');
    }

    loadSavedStats() {
        let statsJson = localStorage.getItem('savedStats');
        if (statsJson) {
            let stats = JSON.parse(statsJson);
            this.cou = stats.cou;
            this.int = stats.int;
            this.cha = stats.cha;
            this.ad = stats.ad;
            this.fo = stats.fo;
            this.setStep(1);
        }
    }

    setStep(step: number) {
        if (step === 1) {
            this.saveStats();
            this.selectedOrigin = undefined;
        }
        if (step === 2) {
            this.selectedJob = undefined;
        }
        if (step === 3) {
            this.selectedSkills = undefined;
        }
        if (step === 4) {
            this.money = 0;
            this.money2 = 0;
        }
        if (step === 5) {
            this.modifiedStat = {};
            if (this.hasStatModification()) {
                this.superBourrinValueAt = 0;
                this.superBourrinValuePrd = 0;
                this.initChangeAtPrd();
                this.initRemoveAttOrPrdToIntOrCha();
                this.initRemoveAttOrPrdToIntOrAd();
                this.initMove1PointStatValues();
            } else {
                step = 6;
            }
        }
        if (step === 6) {
            this.initSelectSpeciality();
            if (!this.hasSpeciality()) {
                step = 7;
            }
        }
        if (step === 7) {
            this.name = undefined;
        }
        if (step === 8) {
            this.fatePoint = undefined;
        }

        this.step = step;
    }

    swapStats(change: string[]) {
        let tmp = this[change[0]];
        this[change[0]] = this[change[1]];
        this[change[1]] = tmp;

        this.setStep(1);
    }

    // Flags management

    hasFlag(flagName: string) {
        if (this.selectedOrigin && this.selectedOrigin.hasFlag(flagName)) {
            return true;
        }
        if (this.selectedJob && this.selectedJob.hasFlag(flagName)) {
            return true;
        }
        return this.selectedSpeciality && this.selectedSpeciality.hasFlag(flagName);
    }

    // Step 0: Statistics

    rollStats() {
        this.cou = 7 + getRandomInt(1, 6);
        this.int = 7 + getRandomInt(1, 6);
        this.cha = 7 + getRandomInt(1, 6);
        this.ad = 7 + getRandomInt(1, 6);
        this.fo = 7 + getRandomInt(1, 6);
        this.setStep(1);

        return false;
    }

    // Step 1: Select origin

    onSelectOrigin(origin) {
        this.selectedOrigin = origin;
        this.setStep(2);
    }

    selectRandomOrigin() {
        this.originSelectorComponent.randomSelect();
    }

    // Step 2: Select job

    onSelectJob(job) {
        if (!this.selectedOrigin) {
            throw new Error('selectedOrigin should not be undef');
        }
        this.selectedJob = job;
        this.setStep(3);
    }

    selectRandomJob() {
        this.jobSelectorComponent.randomSelect();
    }

    // Step 3: Select skills

    onSelectSkills(skills) {
        this.selectedSkills = skills;
        this.setStep(4);
    }

    selectRandomSkills() {
        this.skillsSelectorComponent.randomSelect();
    }

    // Step 4: Select fortune

    has2MoneyRoll() {
        return this.hasFlag('2_ROLL_MONEY');
    }

    rollMoney() {
        this.money = getRandomInt(1, 6) + getRandomInt(1, 6);
        if (this.has2MoneyRoll()) {
            this.money2 = getRandomInt(1, 6) + getRandomInt(1, 6);
        }
        this.setStep(5);
    }

    validMondey() {
        this.setStep(5);
    }

    // Step 5: Stats modification due to bonus

    public hasStatModification() {
        return this.hasSuperBourin()
            || this.hasMove1PointStat()
            || this.hasChangeAtPrd()
            || this.hasRemoveAttOrPrdToIntOrCha()
            || this.hasRemoveAttOrPrdToIntOrAd()
            ;
    }

    public isStatModificationValid() {
        if (this.hasSuperBourin()) {
            if (!this.isSuperBourrinValid()) {
                return false;
            }
        }
        if (this.hasMove1PointStat()) {
            if (!this.isMove1PointStatValid()) {
                return false;
            }
        }
        if (this.hasChangeAtPrd()) {
            if (!this.isChangeAtPrdValid()) {
                return false;
            }
        }
        if (this.hasRemoveAttOrPrdToIntOrCha()) {
            if (!this.isRemoveAttOrPrdToIntOrChaValid()) {
                return false;
            }
        }
        if (this.hasRemoveAttOrPrdToIntOrAd()) {
            if (!this.isRemoveAttOrPrdToIntOrAdValid()) {
                return false;
            }
        }
        return true;
    }


    /// SUPER_BOURRIN

    hasSuperBourin() {
        return this.hasFlag('SUPER_BOURRIN');
    }

    isSuperBourrinValid() {
        return (this.superBourrinValueAt + this.superBourrinValuePrd >= 0
            && this.superBourrinValueAt + this.superBourrinValuePrd <= 3);
    }

    updateSuperBourrin() {
        this.modifiedStat['SUPER_BOURRIN'] = {};
        this.modifiedStat['SUPER_BOURRIN']['AT'] = -this.superBourrinValueAt;
        this.modifiedStat['SUPER_BOURRIN']['PRD'] = -this.superBourrinValuePrd;
        this.modifiedStat['SUPER_BOURRIN']['PI'] = (this.superBourrinValueAt + this.superBourrinValuePrd);
        this.modifiedStat['SUPER_BOURRIN'].name = 'Super-bourrin';
    }

    /// END SUPER_BOURRIN

    /// MOVE_1_POINT_STAT

    initMove1PointStatValues() {
        this.move1PointStatValues = {'cou': 0, 'int': 0, 'cha': 0, 'ad': 0, 'fo': 0};
        this.modifiedStat['MOVE_1_POINT_STAT'] = {};
    }

    hasMove1PointStat() {
        return this.hasFlag('MOVE_1_POINT_STAT');
    }

    isMove1PointStatValid() {
        let foundNegative = 0;
        let foundPositive = 0;
        for (let i in this.move1PointStatValues) {
            if (!this.move1PointStatValues.hasOwnProperty(i)) {
                continue;
            }
            let value = this.move1PointStatValues[i];
            if (value === 1) {
                foundPositive++;
            } else if (value === -1) {
                foundNegative++;
            } else if (value !== 0) {
                return false;
            }
        }
        if (foundNegative === 0 && foundPositive === 0) {
            return true;
        }
        if (foundNegative === 1 && foundPositive === 1) {
            return true;
        }
        return false;
    }

    updateMove1PointStat() {
        if (this.isMove1PointStatValid()) {
            for (let i in this.move1PointStatValues) {
                if (!this.move1PointStatValues.hasOwnProperty(i)) {
                    continue;
                }
                let value = this.move1PointStatValues[i];
                if (value !== 0) {
                    this.modifiedStat['MOVE_1_POINT_STAT'][i.toUpperCase()] = value;
                    this.modifiedStat['MOVE_1_POINT_STAT'].name = 'Polyvalence ranger';
                } else {
                    delete this.modifiedStat['MOVE_1_POINT_STAT'][i.toUpperCase()];
                }
            }
        }
    }

    move1PointStat(stat: string, value: number) {
        for (let i in this.move1PointStatValues) {
            if (!this.move1PointStatValues.hasOwnProperty(i)) {
                continue;
            }
            if (stat === i) {
                continue;
            }
            if (value > 0 && this.move1PointStatValues[i] > 0) {
                this.move1PointStatValues[i] = 0;
            }
            if (value < 0 && this.move1PointStatValues[i] < 0) {
                this.move1PointStatValues[i] = 0;
            }
        }
        if (this.move1PointStatValues[stat] === value) {
            this.move1PointStatValues[stat] = 0;
        } else {
            this.move1PointStatValues[stat] = value;
        }
        this.updateMove1PointStat();
    }

    /// END MOVE_1_POINT_STAT

    /// CHANGE_1_AT_PRD

    hasChangeAtPrd() {
        return this.hasFlag('CHANGE_1_AT_PRD');
    }

    isChangeAtPrdValid() {
        return (this.changeAtPrdValue >= -1 && this.changeAtPrdValue <= 1);
    }

    initChangeAtPrd() {
        this.changeAtPrdValue = 0;
    }

    updateChangeAtPrd() {
        this.modifiedStat['CHANGE_1_AT_PRD'] = {};
        this.modifiedStat['CHANGE_1_AT_PRD']['AT'] = this.changeAtPrdValue;
        this.modifiedStat['CHANGE_1_AT_PRD']['PRD'] = -this.changeAtPrdValue;
        this.modifiedStat['CHANGE_1_AT_PRD'].name = 'Guerrier Lvl 1';

    }

    /// END CHANGE_1_AT_PRD

    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA

    hasRemoveAttOrPrdToIntOrCha() {
        return this.hasFlag('REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA');
    }

    isRemoveAttOrPrdToIntOrChaValid() {
        return (this.removeAttOrPrdToIntOrChaRemoveStat === 'AT' || this.removeAttOrPrdToIntOrChaRemoveStat === 'PRD')
            && (this.removeAttOrPrdToIntOrChaAddStat === 'INT' || this.removeAttOrPrdToIntOrChaAddStat === 'CHA');
    }

    initRemoveAttOrPrdToIntOrCha() {
        this.removeAttOrPrdToIntOrChaRemoveStat = 'AT';
        this.removeAttOrPrdToIntOrChaAddStat = 'INT';
    }

    updateRemoveAttOrPrdToIntOrCha() {
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA'] = {};
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA'][this.removeAttOrPrdToIntOrChaRemoveStat] = -1;
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA'][this.removeAttOrPrdToIntOrChaAddStat] = 1;
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA'].name = 'Marchand Lvl 1';
    }

    /// END REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA

    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD

    hasRemoveAttOrPrdToIntOrAd() {
        return this.hasFlag('REMOVE_1_AT_OR_PRD_TO_INT_OR_AD');
    }

    isRemoveAttOrPrdToIntOrAdValid() {
        return (this.removeAttOrPrdToIntOrAdRemoveStat === 'AT' || this.removeAttOrPrdToIntOrAdRemoveStat === 'PRD')
            && (this.removeAttOrPrdToIntOrAdAddStat === 'INT' || this.removeAttOrPrdToIntOrAdAddStat === 'AD');
    }

    initRemoveAttOrPrdToIntOrAd() {
        this.removeAttOrPrdToIntOrAdRemoveStat = 'AT';
        this.removeAttOrPrdToIntOrAdAddStat = 'INT';
    }

    updateRemoveAttOrPrdToIntOrAd() {
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_AD'] = {};
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_AD'][this.removeAttOrPrdToIntOrAdRemoveStat] = -1;
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_AD'][this.removeAttOrPrdToIntOrAdAddStat] = 1;
        this.modifiedStat['REMOVE_1_AT_OR_PRD_TO_INT_OR_AD'].name = 'Ingénieur Lvl 1';
    }

    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD


    // Step 6: Select speciality

    hasSpeciality() {
        return this.selectedJob && this.selectedJob.specialities && this.selectedJob.specialities.length > 0;
    }

    initSelectSpeciality() {
        this.selectedSpeciality = undefined;
    }

    selectSpeciality(speciality: Speciality) {
        this.selectedSpeciality = speciality;
        this.setStep(7);
    }

    randomSpeciality() {
        // FI
    }

    // Step 7: Name

    randomNameAndSex() {
        if (getRandomInt(0, 1)) {
            this.sex = 'Homme';
        } else {
            this.sex = 'Femme';
        }
        this.randomName();
    }

    randomName() {

    }

    validName() {
        this.setStep(8);
    }

    // Step 8: FatePoint

    rollFatePoint() {
        this.fatePoint = getRandomInt(0, 3);
        this.validFatePoint();
    }

    validFatePoint() {
        this.setStep(9);
    }

    // Step 9: Confirm

    createCharacter() {
        if (!this.selectedOrigin) {
            throw new Error('createCharacter: `selectedOrigin` should not be undef');
        }
        if (!this.selectedSkills) {
            throw new Error('createCharacter: `selectedSkills` should not be undef');
        }

        this.creating = true;
        let creationData = {};
        creationData['name'] = this.name;
        creationData['stats'] = {};
        creationData['stats']['COU'] = this.cou;
        creationData['stats']['INT'] = this.int;
        creationData['stats']['CHA'] = this.cha;
        creationData['stats']['AD'] = this.ad;
        creationData['stats']['FO'] = this.fo;
        creationData['origin'] = this.selectedOrigin.id;
        if (this.selectedJob) {
            creationData['job'] = this.selectedJob.id;
        }

        creationData['sex'] = this.sex;
        creationData['skills'] = [];
        creationData['skills'].push(this.selectedSkills[0].id);
        creationData['skills'].push(this.selectedSkills[1].id);
        creationData['money'] = this.money * 10;
        if (this.money2) {
            creationData['money'] += this.money2 * 10;
        }
        if (this.modifiedStat) {
            creationData['modifiedStat'] = this.modifiedStat;
        }
        if (this.selectedSpeciality) {
            creationData['speciality'] = this.selectedSpeciality.id;
        }

        creationData['fatePoint'] = this.fatePoint;

        if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('isNpc')) {
            creationData['isNpc'] = this._router.routerState.snapshot.root.queryParams['isNpc'];
        }
        if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
            creationData['groupId'] = +this._router.routerState.snapshot.root.queryParams['groupId'];
        } else {
            creationData['groupId'] = null;
        }

        this.clearSavedStats();
        this._characterService.createCharacter(creationData).subscribe(
            res => {
                if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
                    this._router.navigate(['/gm/group'
                        , +this._router.routerState.snapshot.root.queryParams['groupId']]);
                } else {
                    this._router.navigate(['player', 'character', 'detail', res.id]);
                }
            },
            () => {
                this.creating = false;
            }
        );
    }

    ngOnInit() {
        this.loadSavedStats();
    }
}

