import {Component, Inject, Input, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';

import {Item} from '../item';
import {Character} from './character.model';
import {God, MiscService, NhbkDialogService} from '../shared';
import {NamesByNumericId} from '../shared/shared,model';
import {LoginService} from '../user';
import {ItemTemplateCategoryDictionary, ItemTemplateService} from '../item-template';
import {OriginService} from '../origin';
import {JobService} from '../job';
import {SkillService} from '../skill';
import {forkJoin} from 'rxjs';

export interface CharacterItemDialogData {
    item: Item,
    character: Character,
    gmView: boolean
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
    public itemCategoriesById?: ItemTemplateCategoryDictionary;
    public modifiers: any[] = [];

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
        private readonly originService: OriginService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly jobService: JobService,
        private readonly miscService: MiscService,
        @Inject(MAT_DIALOG_DATA) public data: CharacterItemDialogData,
    ) {
        this.updateModifiers();
    }

    private updateModifiers() {

        this.modifiers = [];
        if (this.data.item && this.data.item.template.modifiers) {
            for (let i = 0; i < this.data.item.template.modifiers.length; i++) {
                let modifier = this.data.item.template.modifiers[i];
                if (modifier.job && !this.data.character.hasJob(modifier.job)) {
                    continue;
                }
                if (modifier.origin && modifier.origin !== this.data.character.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (let j = 0; j < this.modifiers.length; j++) {
                    let newMod = this.modifiers[j];
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
                    this.modifiers.push(newModifier);
                }
            }
        }
    }

    ngOnInit() {
        forkJoin([
            this.originService.getOriginsNamesById(),
            this.jobService.getJobsNamesById(),
            this.miscService.getGodsByTechName(),
            this.itemTemplateService.getCategoriesById(),
        ]).subscribe(([originsName, jobsName, godsByTechName, itemCategoriesById]) => {
            this.originsName = originsName;
            this.jobsName = jobsName;
            this.godsByTechName = godsByTechName;
            this.itemCategoriesById = itemCategoriesById;

            this.loading = false;
        });
    }

}
