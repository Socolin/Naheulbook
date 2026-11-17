import {Component, Inject, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {MonsterTrait, TraitInfo} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {MonsterTraitDialogComponent} from './monster-trait-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {MatIcon} from '@angular/material/icon';
import {FormsModule} from '@angular/forms';
import {MatCheckbox} from '@angular/material/checkbox';

export interface SelectMonsterTraitsDialogData {
    selectedTraits: TraitInfo[];
}

export interface SelectMonsterTraitsDialogResult {
    selectedTraits: TraitInfo[];
}

@Component({
    selector: 'app-select-monster-traits-dialog',
    templateUrl: './select-monster-traits-dialog.component.html',
    styleUrls: ['./select-monster-traits-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatDialogTitle, MatDialogContent, MatProgressSpinner, MatCheckbox, MatIconButton, MatIcon, MatButtonToggleGroup, MatButtonToggle, FormsModule, MatDialogActions, MatButton, MatDialogClose]
})
export class SelectMonsterTraitsDialogComponent implements OnInit {
    public traits?: MonsterTrait[];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];
    public selectedSimpleTraits: { [id: number]: boolean } = {};
    public selectedPowerTraits: { [id: number]: number } = {};
    public readonly levels = [0, 1, 2, 3];

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly dialogRef: MatDialogRef<SelectMonsterTraitsDialogComponent, SelectMonsterTraitsDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: SelectMonsterTraitsDialogData,
    ) {
    }

    ngOnInit() {
        this.monsterTemplateService.getMonsterTraits().subscribe((traits) => {
            this.traits = traits;
            this.powerTraits = traits.filter(t => t.levels && t.levels.length);
            this.simpleTraits = traits.filter(t => !t.levels || t.levels.length === 0);

            for (const trait of this.simpleTraits) {
                let traitInfo = this.data.selectedTraits.find(x => x.traitId === trait.id);
                this.selectedSimpleTraits[trait.id] = traitInfo ? traitInfo.level > 0 : false;
            }
            for (const trait of this.powerTraits) {
                let traitInfo = this.data.selectedTraits.find(x => x.traitId === trait.id);
                this.selectedPowerTraits[trait.id] = traitInfo ? traitInfo.level : 0;
            }
        })
    }

    save() {
        const selectedTraits: TraitInfo[] = [];
        for (const trait of this.simpleTraits.filter(t => this.selectedSimpleTraits[t.id])) {
            selectedTraits.push({
                traitId: trait.id,
                level: 1
            });
        }

        for (const traitId of Object.keys(this.selectedPowerTraits)) {
            if (!this.selectedPowerTraits.hasOwnProperty(traitId)) {
                continue;
            }
            if (!this.selectedPowerTraits[traitId]) {
                continue;
            }
            selectedTraits.push({
                traitId: +traitId,
                level: this.selectedPowerTraits[traitId]
            });
        }

        this.dialogRef.close({selectedTraits});
    }

    showTraitInfo($event: MouseEvent, trait: MonsterTrait) {
        $event.preventDefault();
        $event.stopImmediatePropagation();
        $event.stopPropagation();
        this.dialog.open(MonsterTraitDialogComponent, {data: {trait}});
    }
}
