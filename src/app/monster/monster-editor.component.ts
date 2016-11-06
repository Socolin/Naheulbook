import {Component, OnInit, Input, OnChanges, SimpleChanges} from '@angular/core';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTrait, TraitInfo} from "./monster.model";
import {MonsterService} from "./monster.service";
import {Observable} from "rxjs";
import {AutocompleteValue} from "../shared/autocomplete-input.component";
import {ItemTemplate} from "../item/item-template.model";
import {ItemService} from "../item/item.service";
import {LocationService} from "../location/location.service";
import {Location} from "../location/location.model";
import {removeDiacritics} from "../shared/remove_diacritics";

@Component({
    selector: 'monster-editor',
    templateUrl: 'monster-editor.component.html',
})
export class MonsterEditorComponent implements OnInit, OnChanges {
    @Input() monster: MonsterTemplate;
    public categories: MonsterTemplateCategory[] = [];
    public locations: Location[] = [];
    public locationsById: {[id: number]:  Location} = null;
    public defenseStat: string = 'PRD';
    public locationSearchName: string;

    public traits: MonsterTrait[] = [];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];

    private autocompleteLocationsCallback: Function = this.updateAutocompleteLocation.bind(this);
    private autocompleteItemCallback: Function = this.updateAutocompleteItem.bind(this);
    private newItem: ItemTemplate;

    constructor(private _monsterService: MonsterService
        , private _locationService: LocationService
        , private _itemService: ItemService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {

    }

    updateAutocompleteItem(filter: string): Observable<AutocompleteValue[]> {
        return this._itemService.searchItem(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    removeLocation(locationId: number) {
        let index = this.monster.locations.indexOf(locationId);
        if (index !== -1) {
            this.monster.locations.splice(index, 1);
        }
    }

    selectLocation(location: Location) {
        if (!this.monster.locations) {
            this.monster.locations = [];
        }
        if (this.monster.locations.indexOf(location.id) == -1) {
            this.monster.locations.push(location.id);
        }
        this.locationSearchName = "";
    }

    updateAutocompleteLocation(filter: string) {
        return Observable.create((function (observer) {
            let result = [];
            if (!filter) {
                observer.next(result);
                return;
            }
            let cleanFilter = removeDiacritics(filter.toLowerCase());
            if (cleanFilter.length === 0) {
                observer.next(result);
                return;
            }
            for (let i = 0; i < this.locations.length; i++) {
                let location = this.locations[i];
                if (location.name.toLowerCase().replace(" ","").indexOf(cleanFilter) !== -1) {
                    result.push(new AutocompleteValue(location, location.name));
                }
            }
            observer.next(result);
        }).bind(this));
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
            , this._locationService.getLocations()
            , this._monsterService.getMonsterTraits()).subscribe(
            res => {
                this.categories = res[0];
                this.locations = res[1];
                let locationsById = {};
                for (let i = 0; i < this.locations.length; i++) {
                    let loc = this.locations[i];
                    locationsById[loc.id] = loc;
                }
                this.locationsById = locationsById;
                this.traits = res[2];
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
