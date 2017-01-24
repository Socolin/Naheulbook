import {Component, Input, OnInit, forwardRef, Inject} from '@angular/core';

import {Stat} from './stat.model';
import {ItemStatModifier, StatModificationOperand} from './stat-modifier.model';
import {Origin, OriginService} from '../origin';
import {Job} from '../job';
import {JobService} from '../job';
import {CharacterService} from '../character';
import {isNullOrUndefined} from 'util';
@Component({
    selector: 'modifiers-editor',
    styleUrls: ['./modifiers-editor.component.scss'],
    templateUrl: './modifiers-editor.component.html'
})
export class ModifiersEditorComponent implements OnInit {
    @Input() modifiers: ItemStatModifier[];
    @Input() options: string[] = [];

    public stats: string[];
    public basicStats: string[] = ['AD', 'CHA', 'COU', 'INT', 'FO'];
    public combatStats: string[] = ['AT', 'PRD', 'PR', 'PR_MAGIC'];
    public lifeStats: string[] = ['EA', 'EV'];
    public magicStats: string[] = ['MPHYS', 'MPSY', 'RESM'];

    public otherSelectedStat: string;
    public basicSelectedStat: string;
    public lifeSelectedStat: string;
    public combatSelectedStat: string;
    public magicSelectedStat: string;

    public selectedStat: string;

    public specialsValue: string[] = [];
    public selectedType: StatModificationOperand = 'ADD';
    public value: number;
    public origins: Origin[];
    public jobs: Job[];

    constructor(@Inject(forwardRef(()  => CharacterService)) private characterService: CharacterService
        , private originService: OriginService
        , private jobService: JobService) {
        this.modifiers = [];
        this.specialsValue = [
            'AFFECT_ONLY_THROW'
            , 'AFFECT_ONLY_MELEE'
            , 'DONT_AFFECT_MAGIEPSY'
            , 'AFFECT_ONLY_MELEE_STAFF'
            , 'AFFECT_PR_FOR_ELEMENTS'
            , 'AFFECT_DISCRETION'
            , 'AFFECT_ONLY_DANSE'
            , 'AFFECT_ONLY_PRODIGE'
            , 'ONLY_IF_NOTHING_ON'
        ];
    }

    toggleSpecial(i: number, v: string) {
        if (this.modifiers[i].special) {
            let idx = this.modifiers[i].special.indexOf(v);
            if (idx === -1) {
                this.modifiers[i].special.push(v);
            } else {
                this.modifiers[i].special.splice(idx, 1);
            }
        }
    }
    removeModifier(i: number) {
        this.modifiers.splice(i, 1);
    }

    clear() {
        this.modifiers = [];
    }

    clearSelectedStat(type?: string) {
        if (type !== 'combat') {
            this.combatSelectedStat = null;
        }
        if (type !== 'life') {
            this.lifeSelectedStat = null;
        }
        if (type !== 'magic') {
            this.magicSelectedStat = null;
        }
        if (type !== 'basic') {
            this.basicSelectedStat = null;
        }
        if (type !== 'other') {
            this.otherSelectedStat = null;
        }
    }

    updateSelectedStat(type: string) {
        switch (type) {
            case 'combat':
                if (this.combatSelectedStat !== null) {
                    this.selectedStat = this.combatSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'life':
                if (this.lifeSelectedStat !== null) {
                    this.selectedStat = this.lifeSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'magic':
                if (this.magicSelectedStat !== null) {
                    this.selectedStat = this.magicSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'basic':
                if (this.basicSelectedStat !== null) {
                    this.selectedStat = this.basicSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'other':
                if (this.otherSelectedStat !== null) {
                    this.selectedStat = this.otherSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
        }
        this.addModifier();
    }

    addModifier() {
        if (isNullOrUndefined(this.selectedStat) || isNullOrUndefined(this.selectedType) || isNullOrUndefined(this.value)) {
            return true;
        }

        if (this.modifiers === null) {
            this.modifiers = [];
        }

        let modifier = new ItemStatModifier();
        modifier.stat = this.selectedStat;
        modifier.value = this.value;
        modifier.type = this.selectedType;

        this.modifiers.push(modifier);

        this.value = null;
        this.selectedStat = null;
        this.clearSelectedStat();
    }

    ngOnInit() {
        this.characterService.getStats().subscribe(stats => {
            let filteredStats = stats.map(s => s.name);
            filteredStats = filteredStats.filter(s => {
                return this.basicStats.indexOf(s) === -1
                    && this.combatStats.indexOf(s) === -1
                    && this.lifeStats.indexOf(s) === -1
                    && this.magicStats.indexOf(s) === -1;
            });
            this.stats = filteredStats;
        });
        this.jobService.getJobList().subscribe(jobs => {
            this.jobs = jobs;
        });
        this.originService.getOriginList().subscribe(origins => {
            this.origins = origins;
        });
    }
}
