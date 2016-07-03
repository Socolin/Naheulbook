import {Component} from '@angular/core';

import {Stat} from "./stat.model";
import {CharacterService} from "../character/character.service";

@Component({
    selector: 'stat-requirements-editor',
    templateUrl: 'app/shared/stat-requirements-editor.component.html',
    inputs: ["requirements"],
})
export class StatRequirementsEditorComponent {
    public stats: Stat[];
    public requirements: Object[];
    public selectedStat: Stat;
    public minValue: number;
    public maxValue: number;

    constructor(private characterService: CharacterService) {
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
        this.characterService.getStats().subscribe(stats => {
            this.stats = stats;
        });
    }
}
