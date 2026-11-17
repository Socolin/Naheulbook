import {Component, Input, ChangeDetectionStrategy} from '@angular/core';
import {ReactiveFormsModule, UntypedFormArray, UntypedFormControl, UntypedFormGroup} from '@angular/forms';
import {MatFormField, MatHint, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {MonsterAttackData} from '../api/shared';

export function createMonsterAttackFormGroup(attack?: Partial<MonsterAttackData>): UntypedFormGroup {
    return new UntypedFormGroup({
        name: new UntypedFormControl(attack?.name),
        at: new UntypedFormControl(attack?.at),
        dmg: new UntypedFormControl(attack?.dmg),
    });
}

export function createMonsterAttacksFormArray(attacks?: MonsterAttackData[]): UntypedFormArray {
    const formArray = new UntypedFormArray([]);
    setMonsterAttacksFormArrayValue(formArray, attacks);
    return formArray;
}

export function setMonsterAttacksFormArrayValue(formArray: UntypedFormArray, attacks?: MonsterAttackData[]) {
    formArray.clear();
    for (const attack of attacks?.length ? attacks : [{} as MonsterAttackData]) {
        formArray.push(createMonsterAttackFormGroup(attack));
    }
}

/**
 * Drop attacks with no information and normalize empty fields, to use on the form value before saving.
 */
export function cleanMonsterAttacks(attacks?: Partial<MonsterAttackData>[]): MonsterAttackData[] {
    return (attacks || [])
        .filter(attack => attack.name || attack.dmg || (attack.at !== null && attack.at !== undefined))
        .map(attack => ({
            name: attack.name || undefined,
            at: attack.at ?? 0,
            dmg: attack.dmg || undefined,
        }));
}

@Component({
    selector: 'app-monster-attacks-editor',
    templateUrl: './monster-attacks-editor.component.html',
    styleUrls: ['./monster-attacks-editor.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [ReactiveFormsModule, MatFormField, MatHint, MatLabel, MatInput, MatIconButton, MatButton, MatIcon, MatTooltip]
})
export class MonsterAttacksEditorComponent {
    @Input({required: true})
    attacks: UntypedFormArray;

    get attackGroups(): UntypedFormGroup[] {
        return this.attacks.controls as UntypedFormGroup[];
    }

    addAttack() {
        this.attacks.push(createMonsterAttackFormGroup());
        this.attacks.markAsDirty();
    }

    removeAttack(index: number) {
        this.attacks.removeAt(index);
        this.attacks.markAsDirty();
    }
}
