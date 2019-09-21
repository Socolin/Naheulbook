import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Effect, EffectCategory, EffectType} from './effect.model';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {PromptDialogComponent, StatModifier} from '../shared';
import {IDurable} from '../api/shared';
import {EffectService} from './effect.service';
import {CreateEffectTypeDialogComponent} from './create-effect-type-dialog.component';

export interface EditEffectDialogData {
    effect?: Effect;
    category?: EffectCategory
}

@Component({
    templateUrl: './edit-effect-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-effect-dialog.component.scss']
})
export class EditEffectDialogComponent implements OnInit {
    public saving = false;
    public effectTypes: EffectType[];

    public selectedType?: EffectType;
    public selectedCategory?: EffectCategory;
    public form: FormGroup;
    public statModifiers: StatModifier[] = [];
    public duration: IDurable = {
        durationType: 'forever'
    };

    constructor(
        private readonly dialog: MatDialog,
        private readonly effectService: EffectService,
        public dialogRef: MatDialogRef<EditEffectDialogComponent, Effect>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EditEffectDialogData
    ) {
        this.form = new FormGroup({
            name: new FormControl(undefined, Validators.required),
            description: new FormControl(),
            dice: new FormControl()
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
            if (this.data.effect && this.data.effect.category) {
                this.selectCategory(this.data.effect.category);
            } else if (this.data.category) {
                this.selectCategory(this.data.category);
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
            this.selectedCategory = effectType.categories[0];
        }
    }

    selectCategory(effectCategory: EffectCategory | 'new') {
        if (effectCategory === 'new') {
            const dialogRef = this.dialog.open(CreateEffectTypeDialogComponent);
            dialogRef.afterClosed().subscribe((result) => {
                if (!result) {
                    return;
                }
                this.effectService.createCategory(this.selectedType, result.name, result.diceSize, result.diceCount, result.note)
                    .subscribe(subCategory => {
                        this.selectedCategory = subCategory;
                        this.effectService.invalidateEffectTypes();
                    });
            })
        } else {
            this.selectedCategory = effectCategory;
            this.selectedType = effectCategory.type;
        }
    }

    saveEffect() {
        if (!this.selectedCategory) {
            return;
        }
        this.saving = true;

        if (this.data.effect) {
            this.effectService.editEffect(this.data.effect.id, {
                ...this.form.value,
                ...this.duration,
                modifiers: [...this.statModifiers],
                categoryId: this.selectedCategory.id
            }).subscribe(effect => {
                this.dialogRef.close(effect);
            }, () => {
                this.saving = false;
            });
        } else {
            this.effectService.createEffect(this.selectedCategory.id, {
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
