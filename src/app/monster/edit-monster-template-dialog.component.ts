import {Component, Inject, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {
    MonsterInventoryElement,
    MonsterTemplate,
    MonsterTemplateCategory,
    MonsterTemplateType,
    MonsterTrait,
    TraitInfo
} from './monster.model';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {PromptDialogComponent, PromptDialogData, PromptDialogResult} from '../shared';
import {MonsterTemplateService} from './monster-template.service';
import {forkJoin} from 'rxjs';
import {AddMonsterItemDialogComponent, AddMonsterItemDialogData} from './add-monster-item-dialog.component';
import {
    SelectMonsterTraitsDialogComponent,
    SelectMonsterTraitsDialogData,
    SelectMonsterTraitsDialogResult
} from './select-monster-traits-dialog.component';
import {MonsterTemplateRequest} from '../api/requests';

export interface EditMonsterTemplateDialogData {
    monsterTemplate?: MonsterTemplate;
    type?: MonsterTemplateType;
    category?: MonsterTemplateCategory;
}

@Component({
    templateUrl: './edit-monster-template-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-monster-template-dialog.component.scss']
})
export class EditMonsterTemplateDialogComponent implements OnInit {
    public saving = false;
    public form = new FormGroup({
        name: new FormControl(undefined, [Validators.required]),
        data: new FormGroup({
            at: new FormControl(),
            prd: new FormControl(),
            esq: new FormControl(),
            ev: new FormControl(),
            ea: new FormControl(),
            pr: new FormControl(),
            pr_magic: new FormControl(),
            dmg: new FormControl(),
            cou: new FormControl(),
            chercheNoise: new FormControl(),
            resm: new FormControl(),
            xp: new FormControl(),
            note: new FormControl(),
            sex: new FormControl(),
        })
    });

    public monsterTemplateTypes: MonsterTemplateType[] = [];
    public traitsById: { [id: number]: MonsterTrait } = {};

    public selectedType?: MonsterTemplateType;
    public selectedCategory?: MonsterTemplateCategory;
    public selectedTraits: TraitInfo[] = [];
    public monsterInventory: MonsterInventoryElement[] = [];

    constructor(
        private readonly dialogRef: MatDialogRef<EditMonsterTemplateDialogComponent, MonsterTemplate>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EditMonsterTemplateDialogData,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly dialog: MatDialog,
    ) {
        this.form.reset(data.monsterTemplate);
    }

    openCreateCategoryDialog() {
        if (!this.selectedType) {
            return;
        }
        const selectedType = this.selectedType;

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
            this.monsterTemplateService.createCategory(selectedType, result.text).subscribe(
                category => {
                    selectedType.categories.push(category);
                    this.selectedCategory = category;
                    this.monsterTemplateService.invalidateMonsterTypes();
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
                    this.monsterTemplateService.invalidateMonsterTypes();
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
        const dialogRef = this.dialog.open<AddMonsterItemDialogComponent, AddMonsterItemDialogData, MonsterInventoryElement>(
            AddMonsterItemDialogComponent,
            {data: {}}
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.monsterInventory.push(result);
        })
    }

    openEditInventoryElementDialog(inventoryElement: MonsterInventoryElement) {
        const dialogRef = this.dialog.open<AddMonsterItemDialogComponent, AddMonsterItemDialogData, MonsterInventoryElement>(
            AddMonsterItemDialogComponent,
            {data: {inventoryElement}}
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            const index = this.monsterInventory.findIndex(element => element === inventoryElement);
            if (index !== -1) {
                this.monsterInventory[index] = result;
            } else {
                this.monsterInventory.push(result);
            }
        })
    }

    removeInventoryElement(inventoryElement: MonsterInventoryElement) {
        const i = this.monsterInventory.findIndex(e => e === inventoryElement);
        if (i !== -1) {
            this.monsterInventory.splice(i, 1);
        }
    }

    openSelectTraitsDialog() {
        const dialogRef = this.dialog.open<SelectMonsterTraitsDialogComponent, SelectMonsterTraitsDialogData,
            SelectMonsterTraitsDialogResult>(
            SelectMonsterTraitsDialogComponent, {
                autoFocus: false, data: {
                    selectedTraits: this.selectedTraits
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.selectedTraits = result.selectedTraits;
        });
    }

    ngOnInit() {
        forkJoin([
            this.monsterTemplateService.getMonsterTypes(),
            this.monsterTemplateService.getMonsterTraitsById(),
        ]).subscribe(
            ([monsterTemplateTypes, traitsById]: [MonsterTemplateType[], { [id: number]: MonsterTrait }]) => {
                this.monsterTemplateTypes = monsterTemplateTypes;

                this.traitsById = traitsById;

                if (this.data.monsterTemplate) {
                    this.selectedType = this.data.monsterTemplate.category.type;
                    this.selectedCategory = this.data.monsterTemplate.category;
                    this.selectedTraits = [...this.data.monsterTemplate.data.traits || []];
                    this.monsterInventory = this.data.monsterTemplate.simpleInventory;
                } else {
                    if (this.data.category && this.data.type) {
                        this.selectedCategory = this.data.category;
                        this.selectedType = this.data.type;
                    } else if (this.data.type) {
                        this.selectType(this.data.type);
                    } else {
                        this.selectType(this.monsterTemplateTypes[0]);
                    }
                }
            }
        );
    }

    save() {
        if (!this.selectedCategory) {
            return;
        }

        this.saving = true;
        let request: MonsterTemplateRequest = {
            categoryId: this.selectedCategory.id,
            data: {
                ...this.form.value.data,
                traits: this.selectedTraits
            },
            name: this.form.value.name,
            simpleInventory: this.monsterInventory.map(inventoryElement => ({
                ...inventoryElement,
                id: inventoryElement.id || undefined,
                itemTemplateId: inventoryElement.itemTemplate.id,
            }))
        };
        if (!this.data.monsterTemplate) {
            this.monsterTemplateService.createMonsterTemplate(
                request
            ).subscribe((result) => {
                this.dialogRef.close(result);
            }, () => {
                this.saving = false;
            });
        } else {
            this.monsterTemplateService.editMonsterTemplate(
                this.data.monsterTemplate.id,
                request
            ).subscribe((result) => {
                this.dialogRef.close(result);
            }, () => {
                this.saving = false;
            });
        }
    }
}
