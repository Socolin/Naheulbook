import {Component, Output, EventEmitter, SimpleChanges, OnChanges, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {ItemService} from '../item';
import {Character, Item} from '../character';
import {ItemTemplate} from '../item';
import {Loot} from '../loot';
import {getRandomInt} from '../shared/random';
import {Monster} from '../monster/monster.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    selector: 'create-item',
    styleUrls: ['./create-item.component.scss'],
    templateUrl: './create-item.component.html',
})
export class CreateItemComponent implements OnChanges {
    @Output() onAddItem: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('addItemDialog')
    public addItemDialog: Portal<any>;
    public addItemOverlayRef: OverlayRef;

    public character: Character;
    public loot: Loot;
    public monster: Monster;
    public mode = 'normal';
    public newItem: Item = new Item();
    public gemOption: any = {};

    constructor(private _itemService: ItemService,
                private _nhbkDialogService: NhbkDialogService) {
    }

    onSelectTab(index: number) {
        if (index === 0) {
            this.setMode('normal');
        }
        else if (index === 1) {
            this.setMode('gem');
        }
    }

    openDialogForCharacter(character: Character) {
        this.loot = null;
        this.monster = null;
        this.character = character;
        this.openDialog();
    }

    openDialogForLoot(loot: Loot) {
        this.loot = loot;
        this.monster = null;
        this.character = null;
        this.openDialog();
    }

    openDialogForLootMonster(loot: Loot, monster: Monster) {
        this.loot = loot;
        this.monster = monster;
        this.character = null;
        this.openDialog();
    }

    openDialogForMonster(monster: Monster) {
        this.loot = null;
        this.monster = monster;
        this.character = null;
        this.openDialog();
    }

    openDialog() {
        this.newItem = new Item();
        this.addItemOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addItemDialog);
    }

    closeDialog() {
        this.addItemOverlayRef.detach();
    }

    addItem(keepOpen: boolean) {
        if (this.mode === 'gem') {
            this.newItem.data.ug = this.gemOption['ug'];
        }
        this.onAddItem.emit({
            monster: this.monster,
            loot: this.loot,
            character: this.character,
            item: this.newItem
        });

        let notIdentified = this.newItem.data.notIdentified;

        this.newItem = new Item();
        if (notIdentified) {
            this.newItem.data.notIdentified = true;
        }

        if (!keepOpen) {
            this.closeDialog();
        }
    }

    selectItemTemplate(itemTemplate: ItemTemplate, input) {
        this.newItem.template = itemTemplate;
        if (!this.newItem.data.notIdentified) {
            this.newItem.data.name = itemTemplate.name;
            input.focus();
        } else {
            if (this.newItem.template && this.newItem.template.data && this.newItem.template.data.notIdentifiedName) {
                this.newItem.data.name = this.newItem.template.data.notIdentifiedName;
            }
        }
        this.newItem.data.description = itemTemplate.data.description;
        if (itemTemplate.data.quantifiable) {
            this.newItem.data.quantity = 1;
        }
    }

    setItemNotIdentified() {
        this.newItem.data.notIdentified = true;
        if (this.mode === 'gem') {
            this.newItem.data.name = 'Pierre précieuse';
        }
        else {
            if (this.newItem.template && this.newItem.template.data && this.newItem.template.data.notIdentifiedName) {
                this.newItem.data.name = this.newItem.template.data.notIdentifiedName;
            }
        }
    }

    setItemIdentified() {
        delete this.newItem.data.notIdentified;
        if (this.newItem.template && this.newItem.template.name) {
            this.newItem.data.name = this.newItem.template.name;
        }
    }

    updateItemIdentified() {
        if (this.newItem.data.notIdentified) {
            this.setItemNotIdentified();
        }
        else {
            this.setItemIdentified();
        }
    }

    setMode(m: string) {
        let oldItem = this.newItem;
        this.newItem = new Item();
        this.newItem.data.notIdentified = oldItem.data.notIdentified;
        this.mode = m;
        if (this.mode === 'gem') {
            this.randomGem();
        }
    }

    randomGem() {
        if (!('maxUg' in this.gemOption)) {
            this.gemOption['maxUg'] = 2;
        }
        let type = getRandomInt(1, 2);
        if (type === 1) {
            this.setGemOption('type', 'raw');
        } else {
            this.setGemOption('type', 'cut');
        }
        let maxUg = 1;
        if ('maxUg' in this.gemOption) {
            maxUg = this.gemOption['maxUg'];
        }
        this.setGemOption('ug', getRandomInt(1, maxUg));
        this.setGemOption('number', getRandomInt(1, 20));
    }

    setGemRandomUg(dice: number) {
        this.setGemOption('ug', getRandomInt(1, dice));
        this.setGemOption('maxUg', dice);
    }


    setGemOption(optName: string, value: any) {
        this.gemOption[optName] = value;
        this.updateGem();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('gemOption' in changes) {
            this.updateGem();
        }
    }

    updateGem() {
        if (this.gemOption['type'] && this.gemOption['number']) {

            this._itemService.getGem(this.gemOption['type'], this.gemOption['number']).subscribe(
                itemTemplate => {
                    this.newItem.template = itemTemplate;
                    if (this.newItem.data.notIdentified) {
                        this.newItem.data.name = 'Pierre précieuse';
                    } else {
                        this.newItem.data.name = itemTemplate.name;
                    }
                    this.newItem.data.description = itemTemplate.data.description;
                    this.newItem.data.quantity = 1;
                }
            );
        }
    }
}
