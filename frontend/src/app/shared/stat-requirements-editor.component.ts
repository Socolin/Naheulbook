import {Component, Input, OnInit} from '@angular/core';

import {Stat} from './stat.model';
import {MiscService} from './misc.service';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/autocomplete';
import { MatDivider } from '@angular/material/list';

@Component({
    selector: 'stat-requirements-editor',
    styleUrls: ['./stat-requirements-editor.component.scss'],
    templateUrl: './stat-requirements-editor.component.html',
    imports: [MatIconButton, MatIcon, MatFormField, MatInput, FormsModule, MatSelect, MatOption, MatDivider, MatButton]
})
export class StatRequirementsEditorComponent implements OnInit {
    public basicStats: string[] = ['AD', 'CHA', 'COU', 'INT', 'FO'];
    public combatStats: string[] = ['AT', 'PRD', 'PR', 'PR_MAGIC', 'ESQ'];
    public lifeStats: string[] = ['EA', 'EV'];
    public magicStats: string[] = ['MPHYS', 'MPSY', 'RESM'];
    public stats: string[];
    @Input() requirements: any[] | undefined;
    public selectedStat: string | undefined;
    public minValue: number | undefined;
    public maxValue: number | undefined;

    constructor(
        private readonly miscService: MiscService
    ) {
        this.requirements = [];
    }

    setStat(stat) {
        this.selectedStat = stat;
        this.addRequirement();
    }

    removeRequirement(i: number) {
        if (!this.requirements) {
            return;
        }
        this.requirements.splice(i, 1);
    }

    clear() {
        this.requirements = [];
    }

    addRequirement() {
        if ((this.minValue || this.maxValue) && this.selectedStat) {
            if (this.requirements == null) {
                this.requirements = [];
            }
            this.requirements.push({stat: this.selectedStat, min: this.minValue, max: this.maxValue});
            this.selectedStat = undefined;
            this.minValue = undefined;
            this.maxValue = undefined;
        }
        return true;
    }


    ngOnInit() {
        this.miscService.getStats().subscribe(stats => {
            let filteredStats = stats.map(s => s.name);
            filteredStats = filteredStats.filter(s => {
                return this.basicStats.indexOf(s) === -1
                    && this.combatStats.indexOf(s) === -1
                    && this.lifeStats.indexOf(s) === -1
                    && this.magicStats.indexOf(s) === -1;
            });
            this.stats = filteredStats;
        });
    }
}
