import {Component, OnInit, Input} from '@angular/core';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTrait, TraitInfo} from "./monster.model";
import {MonsterService} from "./monster.service";
import {Observable} from "rxjs";

@Component({
    selector: 'monster-editor',
    templateUrl: 'monster-editor.component.html',
})
export class MonsterEditorComponent implements OnInit {
    @Input() monster: MonsterTemplate;
    public categories: MonsterTemplateCategory[] = [];
    public traits: MonsterTrait[] = [];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];

    constructor(private _monsterService: MonsterService) {
    }

    hasTrait(trait: MonsterTrait): TraitInfo {
        if (!this.monster.data.traits) {
            return null;
        }
        let i = 0;
        let traits = this.monster.data.traits;
        for (; i < traits.length; i++) {
            let t = traits[i];
            if (trait.id === t.traitId) {
                return t;
            }
        }
        return null;
    }

    hasTraitLevel(trait: MonsterTrait, level: number): boolean {
        let currentTrait = this.hasTrait(trait);
        if (currentTrait) {
            return currentTrait.level == level;
        }
        return false;
    }


    toggleTrait(trait: MonsterTrait) {
        if (!this.monster.data.traits) {
            this.monster.data.traits = [];
        }

        let currentTrait = null;
        let i = 0;
        let traits = this.monster.data.traits;
        for (; i < traits.length; i++) {
            let t = traits[i];
            if (trait.id === t.traitId) {
                currentTrait = t;
                break;
            }
        }
        if (currentTrait) {
            let maxLevel = 1;
            if (trait.levels) {
                maxLevel = trait.levels.length;
            }

            if (currentTrait.level >= maxLevel) {
                traits.splice(i, 1);
            }
            else {
                currentTrait.level++;
            }
        } else {
            traits.push(new TraitInfo(trait.id, 1));
        }
    }

    ngOnInit() {
        Observable.forkJoin(this._monsterService.getMonsterCategories()
            , this._monsterService.getMonsterTraits()).subscribe(
            res => {
                this.categories = res[0];
                this.traits = res[1];
                let simpleTraits = [];
                let powerTraits = [];
                for (let i = 0; i < this.traits.length; i++) {
                    let trait = this.traits[i];
                    if (!trait.levels || trait.levels.length == 0) {
                        simpleTraits.push(trait);
                    } else {
                        powerTraits.push(trait);
                    }
                }
                this.powerTraits = powerTraits;
                this.simpleTraits = simpleTraits;
            }
        );
    }
}
