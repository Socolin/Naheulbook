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
    @Input() requirements: any[] | undefined;
    public selectedStat: Stat | undefined;
    public minValue: number | undefined;
    public maxValue: number | undefined;

    constructor(private _miscService: MiscService) {
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
            this.requirements.push({ stat: this.selectedStat.name, min: this.minValue, max: this.maxValue });
            this.selectedStat = undefined;
            this.minValue = undefined;
            this.maxValue = undefined;
        }
        return true;
    }

    ngOnInit() {
        this._miscService.getStats().subscribe(stats => {
            this.stats = stats;
        });
    }
}
