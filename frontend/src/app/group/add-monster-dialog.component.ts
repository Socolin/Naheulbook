import {Component, OnInit} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup} from '@angular/forms';
import {getRandomInt} from '../shared';
import {Item, ItemData} from '../item';
import {MonsterTemplate, MonsterTemplateService, MonsterTemplateSubCategory, MonsterTemplateType} from '../monster';
import {ItemTemplate} from '../item-template';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {MatDialogRef} from '@angular/material/dialog';
import {NhbkMatDialog} from '../material-workaround';


export interface AddMonsterDialogResult {
    name: string;
    at: number;
    prd: number;
    esq: number;
    ev: number;
    maxEv: number;
    ea: number;
    maxEa: number;
    pr: number;
    pr_magic: number;
    dmg: string;
    cou: number;
    chercheNoise: boolean;
    resm: number;
    xp: number;
    note: string;
    sex?: 'f' | 'h';
    items: Item[];
}

@Component({
    selector: 'app-add-monster-dialog',
    templateUrl: './add-monster-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './add-monster-dialog.component.scss'],
    standalone: false
})
export class AddMonsterDialogComponent implements OnInit {
    public monsterTypes: MonsterTemplateType[] = [];

    public selectedMonsterType?: MonsterTemplateType;
    public selectedMonsterSubCategory?: MonsterTemplateSubCategory;
    public selectedMonsterTemplate?: MonsterTemplate;
    public filteredMonsters: MonsterTemplate[] = [];
    public filter?: string;
    public items: Item[] = [];

    public form = new UntypedFormGroup({
        name: new UntypedFormControl(),
        at: new UntypedFormControl(),
        prd: new UntypedFormControl(),
        esq: new UntypedFormControl(),
        ev: new UntypedFormControl(),
        maxEv: new UntypedFormControl(),
        ea: new UntypedFormControl(),
        maxEa: new UntypedFormControl(),
        pr: new UntypedFormControl(),
        pr_magic: new UntypedFormControl(),
        dmg: new UntypedFormControl(),
        cou: new UntypedFormControl(),
        chercheNoise: new UntypedFormControl(),
        resm: new UntypedFormControl(),
        xp: new UntypedFormControl(),
        note: new UntypedFormControl(),
        sex: new UntypedFormControl(),
    });

    constructor(
        private readonly dialogRef: MatDialogRef<AddMonsterDialogComponent>,
        private readonly dialog: NhbkMatDialog,
        private readonly monsterTemplateService: MonsterTemplateService,
    ) {
    }

    addMonster() {
        this.dialogRef.close({...this.form.value, items: this.items});
    }

    randomMonsterInventory() {
        let template = this.selectedMonsterTemplate;
        if (template && template.inventory) {
            this.items = [];
            for (let i = 0; i < template.inventory.length; i++) {
                let inventoryItem = template.inventory[i];
                let quantity = getRandomInt(inventoryItem.minCount, inventoryItem.maxCount);
                if (!quantity) {
                    continue;
                }
                if (Math.random() > inventoryItem.chance) {
                    continue;
                }
                let item = new Item();
                item.template = inventoryItem.itemTemplate;
                if (inventoryItem.itemTemplate.data.notIdentifiedName) {
                    item.data.name = inventoryItem.itemTemplate.data.notIdentifiedName;
                    item.data.notIdentified = true;
                } else {
                    item.data.name = inventoryItem.itemTemplate.name;
                    delete item.data.notIdentified;
                }
                item.data.icon = inventoryItem.itemTemplate.data.icon;
                if (item.template.data.useUG) {
                    item.data.ug = 1; // FIXME: minUg/maxUg
                }
                if (item.template.data.quantifiable) {
                    item.data.quantity = quantity;
                    this.items.push(item);
                } else {
                    for (let j = 0; j < quantity; j++) {
                        let duplicate = new Item();
                        duplicate.template = item.template;
                        duplicate.data = new ItemData();
                        duplicate.data.name = item.data.name;
                        duplicate.data.notIdentified = item.data.notIdentified;
                        duplicate.data.icon = item.data.icon;
                        duplicate.data.ug = item.data.ug;
                        this.items.push(duplicate);
                    }
                }
            }
        }
    }

    updateFilteredMonster() {
        if (!this.filter) {
            return;
        }

        const monsterTypeId = this.selectedMonsterType && this.selectedMonsterType.id;
        const monsterSubCategoryId = this.selectedMonsterSubCategory && this.selectedMonsterSubCategory.id;

        return this.monsterTemplateService.searchMonster(this.filter, monsterTypeId, monsterSubCategoryId).subscribe((monsters) => {
            this.filteredMonsters = monsters;
        });
    }

    selectMonsterInAutocompleteList(monster: MonsterTemplate) {
        this.selectedMonsterTemplate = monster;

        this.form.reset({
            name: monster.name,
            at: monster.data.at,
            prd: monster.data.prd,
            esq: monster.data.esq,
            ev: monster.data.ev,
            maxEv: monster.data.ev,
            ea: monster.data.ea,
            maxEa: monster.data.ea,
            pr: monster.data.pr,
            pr_magic: monster.data.pr_magic,
            cou: monster.data.cou,
            dmg: monster.data.dmg,
            xp: monster.data.xp,
            resm: monster.data.resm || 0,
            note: monster.data.note,
        });

        this.randomMonsterInventory();
        return false;
    }

    openItemTemplateDialog(itemTemplate: ItemTemplate) {
        this.dialog.open(ItemTemplateDialogComponent, {
            data: {itemTemplate},
            autoFocus: false
        });
    }

    selectMonsterType(monsterType: MonsterTemplateType | 'none') {
        if (monsterType === 'none') {
            this.selectedMonsterType = undefined;
        } else {
            this.selectedMonsterType = monsterType;
            this.selectedMonsterSubCategory = undefined;
        }
        this.updateFilteredMonster()
    }

    selectMonsterSubCategory(monsterSubCategory: MonsterTemplateSubCategory | 'none') {
        if (monsterSubCategory === 'none') {
            this.selectedMonsterSubCategory = undefined;
        } else {
            this.selectedMonsterSubCategory = monsterSubCategory;
        }
        this.updateFilteredMonster()
    }

    ngOnInit(): void {
        this.monsterTemplateService.getMonsterTypes().subscribe(monsterTypes => {
            this.monsterTypes = monsterTypes;
        });
    }

    removeItemsFromMonsterInventory(items: Item[]) {
        for (let item of items) {
            let index = this.items.findIndex(x => x === item);
            if (index !== -1) {
                this.items.splice(index, 1);
            }
        }
    }
}
