import {Component, Input, OnInit, forwardRef, Inject} from '@angular/core';

import {Stat} from "./stat.model";
import {ItemStatModifier, StatModificationOperand} from "./stat-modifier.model";
import {Origin, OriginService} from "../origin";
import {Job} from "../job";
import {JobService} from "../job";
import {CharacterService} from "../character";
@Component({
    selector: 'modifiers-editor',
    templateUrl: 'modifiers-editor.component.html'
})
export class ModifiersEditorComponent implements OnInit {
    @Input() modifiers: ItemStatModifier[];
    @Input() options: string[] = [];

    public stats: Stat[];

    public specialsValue: string[] = [];
    public selectedStat: Stat;
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

    setStat(stat) {
        this.selectedStat = stat;
        this.addModifier();
    }

    setValue(value) {
        this.value = value;
        this.addModifier();
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

    addModifier() {
        if (this.value != null && this.selectedStat && this.selectedType) {
            if (this.modifiers == null) {
                this.modifiers = [];
            }
            let modifier = new ItemStatModifier();
            modifier.stat = this.selectedStat.name;
            modifier.value = this.value;
            modifier.type = this.selectedType;
            this.modifiers.push(modifier);
            this.value = null;
            this.selectedStat = null;
        }
        return true;
    }

    ngOnInit() {
        this.characterService.getStats().subscribe(stats => {
            this.stats = stats;
        });
        this.jobService.getJobList().subscribe(jobs => {
            this.jobs = jobs;
        });
        this.originService.getOriginList().subscribe(origins => {
            this.origins = origins;
        });
    }
}
