import {Component, Inject, OnInit} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {
    MonsterInventoryElement,
    MonsterTemplate,
    MonsterTemplateSubCategory,
    MonsterTemplateType,
    MonsterTrait,
    TraitInfo
} from './monster.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
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
import {NhbkMatDialog} from '../material-workaround';

export interface EditMonsterTemplateDialogData {
    monsterTemplate?: MonsterTemplate;
    type?: MonsterTemplateType;
    subCategory?: MonsterTemplateSubCategory;
}

@Component({
    templateUrl: './edit-monster-template-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-monster-template-dialog.component.scss']
})
export class EditMonsterTemplateDialogComponent implements OnInit {
    public saving = false;
    public form = new UntypedFormGroup({
        name: new UntypedFormControl(undefined, [Validators.required]),
        data: new UntypedFormGroup({
            at: new UntypedFormControl(),
            prd: new UntypedFormControl(),
            esq: new UntypedFormControl(),
            ev: new UntypedFormControl(),
            ea: new UntypedFormControl(),
            pr: new UntypedFormControl(),
            pr_magic: new UntypedFormControl(),
            dmg: new UntypedFormControl(),
            cou: new UntypedFormControl(),
            chercheNoise: new UntypedFormControl(),
            resm: new UntypedFormControl(),
            xp: new UntypedFormControl(),
            note: new UntypedFormControl(),
            sex: new UntypedFormControl(),
            page: new UntypedFormControl()
        })
    });

    public monsterTemplateTypes: MonsterTemplateType[] = [];
    public traitsById: { [id: number]: MonsterTrait } = {};

    public selectedType?: MonsterTemplateType;
    public selectedSubCategory?: MonsterTemplateSubCategory;
    public selectedTraits: TraitInfo[] = [];
    public monsterInventory: MonsterInventoryElement[] = [];

    constructor(
        private readonly dialogRef: MatDialogRef<EditMonsterTemplateDialogComponent, MonsterTemplate>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EditMonsterTemplateDialogData,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly dialog: NhbkMatDialog,
    ) {
        this.form.reset(data.monsterTemplate);
    }

    openCreateSubCategoryDialog() {
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
            this.monsterTemplateService.createSubCategory(selectedType, result.text).subscribe(
                subCategory => {
                    selectedType.subCategories.push(subCategory);
                    this.selectedSubCategory = subCategory;
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

    selectSubCategory(subCategory: MonsterTemplateSubCategory | 'new') {
        if (subCategory === 'new') {
            this.openCreateSubCategoryDialog();
        } else {
            this.selectedSubCategory = subCategory;
        }
    }

    selectType(type: MonsterTemplateType | 'new') {
        if (type === 'new') {
            this.openCreateTypeDialog();
        } else {
            this.selectedType = type;
            if (this.selectedType.subCategories.length) {
                this.selectedSubCategory = this.selectedType.subCategories[0];
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
                    this.selectedType = this.data.monsterTemplate.subCategory.type;
                    this.selectedSubCategory = this.data.monsterTemplate.subCategory;
                    this.selectedTraits = [...this.data.monsterTemplate.data.traits || []];
                    this.monsterInventory = this.data.monsterTemplate.inventory;
                } else {
                    if (this.data.subCategory && this.data.type) {
                        this.selectedSubCategory = this.data.subCategory;
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
        if (!this.selectedSubCategory) {
            return;
        }

        this.saving = true;
        let request: MonsterTemplateRequest = {
            subCategoryId: this.selectedSubCategory.id,
            data: {
                ...this.form.value.data,
                traits: this.selectedTraits
            },
            name: this.form.value.name,
            inventory: this.monsterInventory.map(inventoryElement => ({
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
