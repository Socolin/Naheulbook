import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {Origin} from '../origin';
import {Job} from '../job';
import {Skill} from '../skill';

import {CharacterService} from './character.service';
import {Speciality} from './speciality.model';
import {getRandomInt} from '../shared/random';

@Component({
    selector: 'create-character',
    templateUrl: './create-character.component.html',
})
export class CreateCharacterComponent {
    public step: number;
    public creating: boolean = false;

    // Step 0: Statistics

    public cou: number;
    public int: number;
    public cha: number;
    public ad: number;
    public fo: number;

    // Step 1: Select origin

    public selectedOrigin: Origin;

    // Step 2: Select job

    public selectedJob: Job;

    // Step 3: Select skills

    public selectedSkills: Skill[];

    // Step 4: Select fortune

    public money: number;
    public money2: number;

    // Step 5: Stats modification due to bonus

    public modifiedStat: {[modifierName: string]: any};

    /// MOVE_1_POINT_STAT
    public move1PointStatValues: {[statName: string]: number};
    /// SUPER_BOURRIN
    public superBourrinValue: number;
    /// CHANGE_1_AT_PRD
    public changeAtPrdValue: number;
    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA
    public removeAttOrPrdToIntOrChaRemoveStat: string;
    public removeAttOrPrdToIntOrChaAddStat: string;
    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD
    public removeAttOrPrdToIntOrAdRemoveStat: string;
    public removeAttOrPrdToIntOrAdAddStat: string;

    // Step 6: Select speciality

    public selectedSpeciality: Speciality;

    // Step 7: Name

    public name: string;
    public sex: string;

    // Step 8: FatePoint

    public fatePoint: number;

    // Bonuses management

    public bonuses: string[];

    constructor(private _router: Router
        , private _characterService: CharacterService) {
        this.setStep(0);
    }

    setStep(step: number) {
        if (step === 1) {
            this.selectedOrigin = null;
        }
        if (step === 2) {
            this.selectedJob = null;
        }
        if (step === 3) {
            this.selectedSkills = null;
        }
        if (step === 4) {
            this.money = 0;
            this.money2 = 0;
        }
        if (step === 5) {
            this.modifiedStat = {};
            if (this.hasStatModification()) {
                this.superBourrinValue = 0;
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
            this.name = null;
        }
        if (step === 8) {
            this.fatePoint = null;
        }

        this.step = step;
    }

    // Bonuses management

    hasBonus(bonus: string) {
        return this.bonuses && this.bonuses.indexOf(bonus) !== -1;
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

    updateStat() {
        this.selectedOrigin = null;
        this.selectedJob = null;
        if (this.cou && this.int && this.cha && this.ad && this.fo) {
            this.setStep(1);
        } else {
            this.setStep(0);
        }
    }

    // Step 1: Select origin

    onSelectOrigin(origin) {
        this.selectedOrigin = origin;
        this.bonuses = [];
        if (origin.bonuses) {
            for (let i = 0; i < origin.bonuses.length; i++) {
                this.bonuses.push(origin.bonuses[i].token);
            }
        }
        this.setStep(2);
    }

    // Step 2: Select job

    onSelectJob(job) {
        if (job != null && job.bonuses) {
            for (let i = 0; i < job.bonuses.length; i++) {
                this.bonuses.push(job.bonuses[i].token);
            }
        }
        this.selectedJob = job;
        this.selectedSkills = null;
        this.setStep(3);
    }

    // Step 3: Select skills

    onSelectSkills(skills) {
        this.selectedSkills = skills;
        this.setStep(4);
    }

    // Step 4: Select fortune

    has2MoneyRoll() {
        return this.hasBonus('2_ROLL_MONEY');
    }

    rollMoney() {
        this.money = getRandomInt(1, 6) + getRandomInt(1, 6);
        if (this.has2MoneyRoll()) {
            this.money2 = getRandomInt(1, 6) + getRandomInt(1, 6);
        }
        this.setStep(5);
    }

    updateMoney() {
        this.setStep(4);
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
        return this.hasBonus('SUPER_BOURRIN');
    }

    isSuperBourrinValid() {
        return (this.superBourrinValue >= 0 && this.superBourrinValue <= 3);
    }

    updateSuperBourrin() {
        this.modifiedStat['SUPER_BOURRIN'] = {};
        this.modifiedStat['SUPER_BOURRIN']['AT'] = -this.superBourrinValue;
        this.modifiedStat['SUPER_BOURRIN']['PI'] = this.superBourrinValue;
        this.modifiedStat['SUPER_BOURRIN'].name = 'Super-bourrin';
    }

    /// END SUPER_BOURRIN

    /// MOVE_1_POINT_STAT

    initMove1PointStatValues() {
        this.move1PointStatValues = {'cou': 0, 'int': 0, 'cha': 0, 'ad': 0, 'fo': 0};
        this.modifiedStat['MOVE_1_POINT_STAT'] = {};
    }

    hasMove1PointStat() {
        return this.hasBonus('MOVE_1_POINT_STAT');
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

    /// END MOVE_1_POINT_STAT

    /// CHANGE_1_AT_PRD

    hasChangeAtPrd() {
        return this.hasBonus('CHANGE_1_AT_PRD');
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
        return this.hasBonus('REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA');
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

    setremoveAttOrPrdToIntOrChaRemoveStat(stat: string) {
        this.removeAttOrPrdToIntOrChaRemoveStat = stat;
        this.updateRemoveAttOrPrdToIntOrCha();
    }

    setRemoveAttOrPrdToIntOrChaAddStat(stat: string) {
        this.removeAttOrPrdToIntOrChaAddStat = stat;
        this.updateRemoveAttOrPrdToIntOrCha();
    }

    /// END REMOVE_1_AT_OR_PRD_TO_INT_OR_CHA

    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD

    hasRemoveAttOrPrdToIntOrAd() {
        return this.hasBonus('REMOVE_1_AT_OR_PRD_TO_INT_OR_AD');
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

    setremoveAttOrPrdToIntOrAdRemoveStat(stat: string) {
        this.removeAttOrPrdToIntOrAdRemoveStat = stat;
        this.updateRemoveAttOrPrdToIntOrAd();
    }

    setRemoveAttOrPrdToIntOrAdAddStat(stat: string) {
        this.removeAttOrPrdToIntOrAdAddStat = stat;
        this.updateRemoveAttOrPrdToIntOrAd();
    }

    /// REMOVE_1_AT_OR_PRD_TO_INT_OR_AD


    // Step 6: Select speciality

    hasSpeciality() {
        return this.selectedJob && this.selectedJob.specialities && this.selectedJob && this.selectedJob.specialities.length > 0;
    }

    initSelectSpeciality() {
        this.selectedSpeciality = null;
    }

    selectSpeciality(speciality: Speciality) {
        this.selectedSpeciality = speciality;
        this.setStep(7);
    }

    // Step 7: Name

    validName() {
        this.setStep(8);
    }

    selectSex(sex: string) {
        this.sex = sex;
        return false;
    }

    // Step 8: FatePoint

    rollFatePoint() {
        this.fatePoint = getRandomInt(0, 3);
    }

    validFatePoint() {
        this.setStep(9);
    }

    // Step 9: Confirm

    createCharacter() {
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
        creationData['money'] = this.money;
        if (this.money2) {
            creationData['money2'] = this.money2;
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

        this._characterService.createCharacter(creationData).subscribe(
            res => {
                if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
                    this._router.navigate(['/character/group'
                        , +this._router.routerState.snapshot.root.queryParams['groupId']]);
                } else {
                    this._router.navigate(['/character/detail', res.id]);
                }
            },
            () => {
                this.creating = false;
            }
        );
    }
}

