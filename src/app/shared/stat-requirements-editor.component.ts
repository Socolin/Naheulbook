import {Component, Input, OnInit} from '@angular/core';

import {Stat} from './stat.model';
import {MiscService} from './misc.service';

@Component({
    selector: 'stat-requirements-editor',
    styleUrls: ['./stat-requirements-editor.component.scss'],
    templateUrl: './stat-requirements-editor.component.html',
})
export class StatRequirementsEditorComponent implements OnInit {
    public stats: Stat[];
    @Input() requirements: any[];
    public selectedStat: Stat;
    public minValue: number;
    public maxValue: number;

    constructor(private _miscService: MiscService) {
        this.requirements = [];
    }

    setStat(stat) {
        this.selectedStat = stat;
        this.addRequirement();
    }

    removeRequirement(i: number) {
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
            this.requirements.push({ stat: this.selectedStat.name, min: this.minValue, max: this.maxValue });
            this.selectedStat = null;
            this.minValue = null;
            this.maxValue = null;
        }
        return true;
    }

    ngOnInit() {
        this._miscService.getStats().subscribe(stats => {
            this.stats = stats;
        });
    }
}
