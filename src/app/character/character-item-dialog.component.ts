import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';

import {Item} from '../item';
import {Character} from './character.model';
import {God, ItemStatModifier, MiscService} from '../shared';
import {NamesByNumericId} from '../shared/shared,model';
import {ItemTemplateService, ItemTemplateSubCategoryDictionary} from '../item-template';
import {OriginService} from '../origin';
import {JobService} from '../job';
import {forkJoin} from 'rxjs';
import {ItemActionService} from './item-action.service';

export interface CharacterItemDialogData {
    item: Item,
    character?: Character,
    gmView: boolean,
    itemActionService: ItemActionService;
    itemInLoot?: boolean;
}

@Component({
    templateUrl: './character-item-dialog.component.html',
    styleUrls: ['./character-item-dialog.component.scss']
})
export class CharacterItemDialogComponent implements OnInit {
    public loading = true;
    public originsName?: NamesByNumericId;
    public jobsName?: NamesByNumericId;
    public godsByTechName?: { [techName: string]: God };
    public itemCategoriesById?: ItemTemplateSubCategoryDictionary;
    public modifiers?: ItemStatModifier[];

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
        private readonly originService: OriginService,
        private readonly jobService: JobService,
        private readonly miscService: MiscService,
        @Inject(MAT_DIALOG_DATA) public data: CharacterItemDialogData,
    ) {
        this.updateModifiers();
    }

    private updateModifiers() {
        if (this.data.character && this.data.item && this.data.item.template.modifiers) {
            const modifiers: ItemStatModifier[] = [];
            for (let i = 0; i < this.data.item.template.modifiers.length; i++) {
                let modifier = this.data.item.template.modifiers[i];
                if (modifier.jobId && !this.data.character.hasJob(modifier.jobId)) {
                    continue;
                }
                if (modifier.originId && modifier.originId !== this.data.character.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (let j = 0; j < modifiers.length; j++) {
                    let newMod = modifiers[j];
                    if (newModifier.stat === newMod.stat
                        && newModifier.type === newMod.type
                        && (!newModifier.special || newModifier.special.length === 0)
                        && (!newMod.special || newMod.special.length === 0)) {
                        newMod.value += newModifier.value;
                        newModifier = null;
                        break;
                    }
                }
                if (newModifier) {
                    modifiers.push(newModifier);
                }
            }

            this.modifiers = [];
        } else {
            this.modifiers = undefined;
        }
    }

    removeModifier(modifierIndex: number) {
        if (!this.data.item.modifiers) {
            return;
        }

        this.data.itemActionService.onAction(
            'update_modifiers',
            this.data.item,
            this.data.item.modifiers.filter((v, i) => i !== modifierIndex)
        );
    }

    disableModifier(modifierIndex: number) {
        if (!this.data.item.modifiers) {
            return;
        }
        const modifier = this.data.item.modifiers[modifierIndex];
        modifier.active = false;
        this.data.itemActionService.onAction('update_modifiers', this.data.item, this.data.item.modifiers);
    }

    activeModifier(modifierIndex: number) {
        if (!this.data.item.modifiers) {
            return;
        }

        const modifier = this.data.item.modifiers[modifierIndex];
        modifier.active = true;
        switch (modifier.durationType) {
            case 'time':
                modifier.currentTimeDuration = modifier.timeDuration;
                break;
            case 'combat':
                modifier.currentCombatCount = modifier.combatCount;
                break;
            case 'lap':
                modifier.currentLapCount = modifier.lapCount;
                break;
        }

        this.data.itemActionService.onAction('update_modifiers', this.data.item, this.data.item.modifiers);
    }

    updateItemUg(ug: number) {
        if (!this.data.item.template.data.useUG) {
            return;
        }
        this.data.itemActionService.onAction('update_data', this.data.item, {
            ...this.data.item.data,
            ug: ug
        });
    }

    ngOnInit() {
        forkJoin([
            this.originService.getOriginsNamesById(),
            this.jobService.getJobsNamesById(),
            this.miscService.getGodsByTechName(),
            this.itemTemplateService.getSubCategoriesById(),
        ]).subscribe(([originsName, jobsName, godsByTechName, itemCategoriesById]) => {
            this.originsName = originsName;
            this.jobsName = jobsName;
            this.godsByTechName = godsByTechName;
            this.itemCategoriesById = itemCategoriesById;

            this.loading = false;
        });
    }
}
