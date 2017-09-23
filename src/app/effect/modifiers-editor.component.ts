import {Component, Input, OnInit} from '@angular/core';

import {ItemStatModifier, StatModificationOperand, MiscService} from '../shared';
import {Origin, OriginService} from '../origin';
import {Job, JobService} from '../job';
import {MatCheckboxChange} from '@angular/material';

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
    public combatStats: string[] = ['AT', 'PRD', 'PR', 'PR_MAGIC', 'ESQ'];
    public lifeStats: string[] = ['EA', 'EV'];
    public magicStats: string[] = ['MPHYS', 'MPSY', 'RESM'];

    public otherSelectedStat: string | undefined;
    public basicSelectedStat: string | undefined;
    public lifeSelectedStat: string | undefined;
    public combatSelectedStat: string | undefined;
    public magicSelectedStat: string | undefined;

    public selectedStat: string | undefined;

    public specialsValue: string[] = [];
    public selectedType: StatModificationOperand = 'ADD';
    public value: number | undefined;
    public origins: Origin[];
    public jobs: Job[];

    constructor(private originService: OriginService
        , private _miscService: MiscService
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

    toggleSpecial(i: number, v: string, event: MatCheckboxChange) {
        if (this.modifiers[i].special) {
            let idx = this.modifiers[i].special.indexOf(v);
            if (idx === -1 && event.checked) {
                this.modifiers[i].special.push(v);
            } else if (idx !== -1 && !event.checked) {
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
            this.combatSelectedStat = undefined;
        }
        if (type !== 'life') {
            this.lifeSelectedStat = undefined;
        }
        if (type !== 'magic') {
            this.magicSelectedStat = undefined;
        }
        if (type !== 'basic') {
            this.basicSelectedStat = undefined;
        }
        if (type !== 'other') {
            this.otherSelectedStat = undefined;
        }
    }

    private getStatType(selectedStat: string): string {
        if (this.combatStats.indexOf(selectedStat) !== -1) {
            return 'combat';
        }
        if (this.lifeStats.indexOf(selectedStat) !== -1) {
            return 'life';
        }
        if (this.magicStats.indexOf(selectedStat) !== -1) {
            return 'magic';
        }
        if (this.basicStats.indexOf(selectedStat) !== -1) {
            return 'basic';
        }
        return 'other';
    }

    selectStat(stat: string) {
        let type = this.getStatType(stat);
        switch (type) {
            case 'combat':
                this.combatSelectedStat = stat;
                break;
            case 'life':
                this.lifeSelectedStat = stat;
                break;
            case 'magic':
                this.magicSelectedStat = stat;
                break;
            case 'basic':
                this.basicSelectedStat = stat;
                break;
            case 'other':
                this.otherSelectedStat = stat;
                break;
        }
        this.clearSelectedStat(type);
    }

    updateSelectedStat(type?: string) {
        switch (type) {
            case 'combat':
                if (this.combatSelectedStat != null) {
                    this.selectedStat = this.combatSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'life':
                if (this.lifeSelectedStat != null) {
                    this.selectedStat = this.lifeSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'magic':
                if (this.magicSelectedStat != null) {
                    this.selectedStat = this.magicSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'basic':
                if (this.basicSelectedStat != null) {
                    this.selectedStat = this.basicSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
            case 'other':
                if (this.otherSelectedStat != null) {
                    this.selectedStat = this.otherSelectedStat;
                    this.clearSelectedStat(type);
                }
                break;
        }
        this.addModifier();
    }

    addModifier() {
        if (!this.selectedStat || !this.selectedType || this.value == null) {
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

        this.value = undefined;
        this.selectedStat = undefined;
        this.clearSelectedStat();
    }

    selectValue(value: number) {
        this.value = value;
        this.addModifier();
    }

    ngOnInit() {
        this._miscService.getStats().subscribe(stats => {
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
