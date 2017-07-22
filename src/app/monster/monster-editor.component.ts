import {Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';
import {Observable} from 'rxjs';

import {removeDiacritics, NhbkDialogService, AutocompleteValue} from '../shared';
import {Location, LocationService} from '../location';
import {ItemTemplate} from '../item';

import {
    MonsterTemplate, MonsterTemplateCategory, MonsterTrait, TraitInfo,
    MonsterSimpleInventory, MonsterTemplateService, MonsterTemplateType
} from '.';

@Component({
    selector: 'monster-editor',
    styleUrls: ['./monster-editor.component.scss'],
    templateUrl: './monster-editor.component.html',
})
export class MonsterEditorComponent implements OnInit, OnChanges {
    @Input() monster: MonsterTemplate;
    public types: MonsterTemplateType[] = [];
    public selectedType: MonsterTemplateType;
    public locations: Location[] = [];
    public locationsById: {[id: number]:  Location} = null;
    public defenseStat = 'PRD';
    public locationSearchName: string;

    public traits: MonsterTrait[] = [];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];

    public autocompleteLocationsCallback: Function = this.updateAutocompleteLocation.bind(this);

    @ViewChild('createCategoryDialog')
    public createCategoryDialog: Portal<any>;
    public createCategorOverlayRef: OverlayRef;

    @ViewChild('createTypeDialog')
    public createTypeDialog: Portal<any>;
    public createTypeOverlayRef: OverlayRef;

    constructor(private _monsterTemplateService: MonsterTemplateService
        , private _locationService: LocationService
        , private _nhbkDialogService: NhbkDialogService) {
    }

    addItemSimpleInventory(itemTemplate: ItemTemplate) {
        let inventoryItem = new MonsterSimpleInventory();
        inventoryItem.itemTemplate = itemTemplate;
        inventoryItem.minCount = 1;
        inventoryItem.maxCount = 1;
        inventoryItem.chance = 1;

        if (!this.monster.simpleInventory) {
            this.monster.simpleInventory = [];
        }
        this.monster.simpleInventory.push(inventoryItem);
    }

    removeItemSimpleInventory(index: number) {
        this.monster.simpleInventory.splice(index, 1);
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
        if (this.monster.locations.indexOf(location.id) === -1) {
            this.monster.locations.push(location.id);
        }
        this.locationSearchName = '';
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
                if (location.name.toLowerCase().replace(' ', '').indexOf(cleanFilter) !== -1) {
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
            return currentTrait.level === level;
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

    openCreateCategoryDialog() {
        this.createCategorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.createCategoryDialog);
    }

    closeCreateCategoryDialog() {
        this.createCategorOverlayRef.detach();
    }

    createCategory(name: string) {
        this.closeCreateCategoryDialog();
        this._monsterTemplateService.createCategory(this.selectedType, name).subscribe(
            category => {
                this.selectedType.categories.push(category);
                this.monster.category = category;
            });
    }

    openCreateTypeDialog() {
        this.createTypeOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.createTypeDialog);
    }

    closeCreateTypeDialog() {
        this.createTypeOverlayRef.detach();
    }

    createType(name: string) {
        this.closeCreateTypeDialog();
        this._monsterTemplateService.createType(name).subscribe(
            type => {
                this.types.push(type);
                this.selectType(type);
            });
    }

    selectCategory(category: MonsterTemplateCategory) {
        if (category === undefined) {
            this.openCreateCategoryDialog();
        }
        this.monster.category = category;
    }

    selectType(type: MonsterTemplateType) {
        if (type === undefined) {
            this.openCreateTypeDialog();
        }
        this.selectedType = type;
        if (this.selectedType.categories.length) {
            this.monster.category = this.selectedType.categories[0];
        }
    }

    updateDefenseStat() {
        if (this.defenseStat === 'PRD') {
            this.monster.data.esq = null;
        }
        if (this.defenseStat === 'ESQ') {
            this.monster.data.prd = null;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('monster' in changes) {
            if (this.monster.data.prd) {
                this.defenseStat = 'PRD';
            }
            else {
                this.defenseStat = 'ESQ';
            }
        }
    }

    ngOnInit() {
        Observable.forkJoin(this._monsterTemplateService.getMonsterTypes()
            , this._locationService.getLocations()
            , this._monsterTemplateService.getMonsterTraits()).subscribe(
            ([types, locations, traits]: [MonsterTemplateType[], Location[], MonsterTrait[]]) => {
                this.types = types;
                if (this.monster.category) {
                    this.selectedType = this.monster.category.type;
                }
                else if (types.length) {
                    this.selectType(this.types[0]);
                }
                this.locations = locations;
                let locationsById = {};
                for (let i = 0; i < this.locations.length; i++) {
                    let loc = this.locations[i];
                    locationsById[loc.id] = loc;
                }
                this.locationsById = locationsById;
                this.traits = traits;
                let simpleTraits = [];
                let powerTraits = [];
                for (let i = 0; i < this.traits.length; i++) {
                    let trait = this.traits[i];
                    if (!trait.levels || trait.levels.length === 0) {
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
