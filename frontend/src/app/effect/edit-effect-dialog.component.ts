import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Effect, EffectSubCategory, EffectType} from './effect.model';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {PromptDialogComponent, StatModifier} from '../shared';
import {IDurable} from '../api/shared';
import {EffectService} from './effect.service';
import {CreateEffectTypeDialogComponent} from './create-effect-type-dialog.component';
import {NhbkMatDialog} from '../material-workaround';

export interface EditEffectDialogData {
    effect?: Effect;
    subCategory?: EffectSubCategory
}

@Component({
    templateUrl: './edit-effect-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-effect-dialog.component.scss']
})
export class EditEffectDialogComponent implements OnInit {
    public saving = false;
    public effectTypes: EffectType[];

    public selectedType?: EffectType;
    public selectedSubCategory?: EffectSubCategory;
    public form: UntypedFormGroup;
    public statModifiers: StatModifier[] = [];
    public duration: IDurable = {
        durationType: 'forever'
    };

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly effectService: EffectService,
        public dialogRef: MatDialogRef<EditEffectDialogComponent, Effect>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EditEffectDialogData
    ) {
        this.form = new UntypedFormGroup({
            name: new UntypedFormControl(undefined, Validators.required),
            description: new UntypedFormControl(),
            dice: new UntypedFormControl()
        });

        if (data.effect) {
            this.form.reset(data.effect);
            this.statModifiers = [...data.effect.modifiers];
            this.duration = {
                durationType: data.effect.durationType,
                combatCount: data.effect.combatCount,
                lapCount: data.effect.lapCount,
                duration: data.effect.duration,
                timeDuration: data.effect.timeDuration
            }
        }
    }

    ngOnInit() {
        this.effectService.getEffectTypes().subscribe(effectTypes => {
            this.effectTypes = effectTypes;
            if (this.data.effect && this.data.effect.subCategory) {
                this.selectSubCategory(this.data.effect.subCategory);
            } else if (this.data.subCategory) {
                this.selectSubCategory(this.data.subCategory);
            } else {
                this.selectType(effectTypes[0]);
            }
        });
    }

    selectType(effectType: EffectType | 'new') {
        if (effectType === 'new') {
            const dialogRef = this.dialog.open(PromptDialogComponent, {
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
                this.effectService.createType(result.text).subscribe(type => {
                    this.effectTypes.push(type);
                    this.selectType(type);
                    this.effectService.invalidateEffectTypes();
                });
            })
        } else {
            this.selectedType = effectType;
            this.selectedSubCategory = effectType.subCategories[0];
        }
    }

    selectSubCategory(effectSubCategory: EffectSubCategory | 'new') {
        if (effectSubCategory === 'new') {
            if (!this.selectedType) {
                return;
            }
            const dialogRef = this.dialog.open(CreateEffectTypeDialogComponent);
            const selectedType = this.selectedType;
            dialogRef.afterClosed().subscribe((result) => {
                if (!result) {
                    return;
                }
                this.effectService.createSubCategory(selectedType, result.name, result.diceSize, result.diceCount, result.note)
                    .subscribe(subCategory => {
                        this.selectedSubCategory = subCategory;
                        this.effectService.invalidateEffectTypes();
                    });
            })
        } else {
            this.selectedSubCategory = effectSubCategory;
            this.selectedType = effectSubCategory.type;
        }
    }

    saveEffect() {
        if (!this.selectedSubCategory) {
            return;
        }
        this.saving = true;

        if (this.data.effect) {
            this.effectService.editEffect(this.data.effect.id, {
                ...this.form.value,
                ...this.duration,
                modifiers: [...this.statModifiers],
                subCategoryId: this.selectedSubCategory.id
            }).subscribe(effect => {
                this.dialogRef.close(effect);
            }, () => {
                this.saving = false;
            });
        } else {
            this.effectService.createEffect(this.selectedSubCategory.id, {
                ...this.form.value,
                ...this.duration,
                modifiers: [...this.statModifiers]
            }).subscribe(effect => {
                this.dialogRef.close(effect);
            }, () => {
                this.saving = false;
            });
        }
    }
}
