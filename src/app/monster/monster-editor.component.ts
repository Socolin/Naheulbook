import {Component, OnInit, Input, OnChanges, SimpleChanges} from '@angular/core';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTrait, TraitInfo} from "./monster.model";
import {MonsterService} from "./monster.service";
import {Observable} from "rxjs";
import {AutocompleteValue} from "../shared/autocomplete-input.component";
import {ItemTemplate} from "../item/item-template.model";
import {ItemService} from "../item/item.service";

@Component({
    selector: 'monster-editor',
    templateUrl: 'monster-editor.component.html',
})
export class MonsterEditorComponent implements OnInit, OnChanges {
    @Input() monster: MonsterTemplate;
    public categories: MonsterTemplateCategory[] = [];
    public defenseStat: string = 'PRD';

    public traits: MonsterTrait[] = [];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];

    private autocompleteItemCallback: Function = this.updateAutocompleteItem.bind(this);
    private newItem: ItemTemplate;

    constructor(private _monsterService: MonsterService
        , private _itemService: ItemService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate, input) {

    }

    updateAutocompleteItem(filter: string) {
        return this._itemService.searchItem(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
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

    selectCategory(category: any) {
        let categoryId = category.target.value;
        for (let i = 0; i < this.categories.length; i++) {
            let c = this.categories[i];
            if (c.id == categoryId) {
                this.monster.type = c;
                break;
            }
        }
    }

    setDefenseStat(stat: string) {
        if (stat === 'PRD') {
            this.monster.data.prd = null;
        }
        if (stat === 'ESQ') {
            this.monster.data.esq = null;
        }
        this.defenseStat = stat;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('monster' in changes) {
            if (this.monster.data.prd) {
                this.setDefenseStat('PRD');
            }
            else {
                this.setDefenseStat('ESQ');
            }
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
