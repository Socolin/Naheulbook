import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MonsterTrait, TraitInfo} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {MonsterTraitDialogComponent} from './monster-trait-dialog.component';
import {MatSelectionList} from '@angular/material/list';
import {NhbkMatDialog} from '../material-workaround';

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
    standalone: false
})
export class SelectMonsterTraitsDialogComponent implements OnInit {
    public traits?: MonsterTrait[];
    public simpleTraits: MonsterTrait[] = [];
    public powerTraits: MonsterTrait[] = [];
    public selectedSimpleTraits: { [id: number]: boolean } = {};
    public selectedPowerTraits: { [id: number]: number } = {};

    @ViewChild('simpleTraitsSelector')
    public simpleTraitsSelector: MatSelectionList;

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
        for (const traitId of this.simpleTraitsSelector.selectedOptions.selected.map(x => x.value)) {
            selectedTraits.push({
                traitId: traitId,
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
