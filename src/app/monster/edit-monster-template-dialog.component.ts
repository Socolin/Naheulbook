import {Component, Inject, OnInit} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import {
    MonsterSimpleInventory,
    MonsterTemplate,
    MonsterTemplateCategory,
    MonsterTemplateType,
    MonsterTrait,
    TraitInfo
} from './monster.model';
import {MAT_DIALOG_DATA, MatDialog} from '@angular/material/dialog';
import {PromptDialogComponent, PromptDialogData, PromptDialogResult} from '../shared';
import {MonsterTemplateService} from './monster-template.service';
import {Location, LocationService} from '../location';
import {forkJoin} from 'rxjs';
import {AddMonsterItemDialogComponent} from './add-monster-item-dialog.component';
import {MatSelectChange} from '@angular/material/select';

export interface EditMonsterTemplateDialogData {
    monsterTemplate?: MonsterTemplate;
}

@Component({
    templateUrl: './edit-monster-template-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-monster-template-dialog.component.scss']
})
export class EditMonsterTemplateDialogComponent implements OnInit {

    public form = new FormGroup({
        name: new FormControl(),
        at: new FormControl(),
        prd: new FormControl(),
        esq: new FormControl(),
        ev: new FormControl(),
        maxEv: new FormControl(),
        ea: new FormControl(),
        maxEa: new FormControl(),
        pr: new FormControl(),
        pr_magic: new FormControl(),
        dmg: new FormControl(),
        cou: new FormControl(),
        chercheNoise: new FormControl(),
        resm: new FormControl(),
        xp: new FormControl(),
        note: new FormControl(),
        sex: new FormControl(),
    });

    public monsterTemplateTypes: MonsterTemplateType[] = [];
    public locations: Location[] = [];
    public locationsById: { [id: number]: Location } | undefined;
    public traits: MonsterTrait[] = [];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];

    public selectedType?: MonsterTemplateType;
    public selectedCategory?: MonsterTemplateCategory;
    public selectedLocations: Location[] = [];
    public selectedTraits: TraitInfo[];
    public monsterInventory: MonsterSimpleInventory[] = [];

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: EditMonsterTemplateDialogData,
        private monsterTemplateService: MonsterTemplateService,
        private locationService: LocationService,
        private dialog: MatDialog
    ) {
    }

    openCreateCategoryDialog() {
        if (!this.selectedType) {
            return;
        }

        const dialogRef = this.dialog.open<PromptDialogComponent, PromptDialogData, PromptDialogResult>(
            PromptDialogComponent, {
                data: {
                    confirmText: 'CRÉER',
                    cancelText: 'ANNULER',
                    placeholder: 'Nom',
                    title: `Créer une sous-catégorie pour "${this.selectedType.name}"`
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.monsterTemplateService.createCategory(this.selectedType, name).subscribe(
                category => {
                    this.selectedType.categories.push(category);
                    this.selectedCategory = category;
                    // FIXME: invalidate cache in service
                });
        })
    }

    openCreateTypeDialog() {
        const dialogRef = this.dialog.open<PromptDialogComponent, PromptDialogData, PromptDialogResult>(
            PromptDialogComponent, {
                data: {
                    confirmText: 'CRÉER',
                    cancelText: 'ANNULER',
                    placeholder: 'Nom',
                    title: 'Créer une catégorie',
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.monsterTemplateService.createType(result.text).subscribe(
                type => {
                    this.monsterTemplateTypes.push(type);
                    this.selectType(type);
                    // FIXME: invalidate cache in service
                });
        })
    }

    selectCategory(category: MonsterTemplateCategory | 'new') {
        if (category === 'new') {
            this.openCreateCategoryDialog();
        } else {
            this.selectedCategory = category;
        }
    }

    selectType(type: MonsterTemplateType | 'new') {
        if (type === 'new') {
            this.openCreateTypeDialog();
        } else {
            this.selectedType = type;
            if (this.selectedType.categories.length) {
                this.selectedCategory = this.selectedType.categories[0];
            }
        }
    }

    openAddItemToInventoryDialog() {
        const dialogRef = this.dialog.open<AddMonsterItemDialogComponent, any, MonsterSimpleInventory>(AddMonsterItemDialogComponent);
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.monsterInventory.push(result);
        })
    }

    ngOnInit() {
        forkJoin([this.monsterTemplateService.getMonsterTypes()
            , this.locationService.getLocations()
            , this.monsterTemplateService.getMonsterTraits()
        ]).subscribe(
            ([monsterTemplateTypes, locations, traits]: [MonsterTemplateType[], Location[], MonsterTrait[]]) => {
                this.monsterTemplateTypes = monsterTemplateTypes;

                this.locations = locations;
                this.locationsById = locations.reduce((dic, loc) => {
                    dic[loc.id] = loc;
                    return dic
                }, {});

                this.traits = traits;
                this.powerTraits = traits.filter(t => t.levels && t.levels.length);
                this.simpleTraits = traits.filter(t => !t.levels || t.levels.length === 0);

                if (this.data.monsterTemplate) {
                    this.selectedType = this.data.monsterTemplate.category.type;
                    this.selectedCategory = this.data.monsterTemplate.category;
                    this.selectedTraits = [...this.data.monsterTemplate.data.traits];
                    this.selectedLocations = this.data.monsterTemplate.locations
                        .map(locationId => this.locationsById[locationId])
                        .filter(l => !!l);
                } else {
                    this.selectType(this.monsterTemplateTypes[0]);
                }
            }
        );
    }

    removeInventoryElement(inventoryElement: MonsterSimpleInventory) {
        const i = this.monsterInventory.findIndex(e => e === inventoryElement);
        if (i !== -1) {
            this.monsterInventory.splice(i, 1);
        }
    }

    openEditInventoryElementDialog(inventoryElement: MonsterSimpleInventory) {

    }
}
