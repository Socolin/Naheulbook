import {Component, Output, EventEmitter, SimpleChanges, OnChanges, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {getRandomInt, IconSelectorComponent, IconSelectorComponentDialogData, NhbkDialogService} from '../shared';
import {Character} from '../character';
import {Item} from '../item';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {Loot} from '../loot';
import {Monster} from '../monster';
import {MatDialog} from '@angular/material';
import {IconDescription} from '../shared/icon.model';

@Component({
    selector: 'create-item',
    styleUrls: ['./create-item.component.scss'],
    templateUrl: './create-item.component.html',
})
export class CreateItemComponent implements OnChanges {
    @Output() onAddItem: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('addItemDialog', {static: true})
    public addItemDialog: Portal<any>;
    public addItemOverlayRef: OverlayRef | undefined;

    public character: Character | undefined;
    public loot: Loot | undefined;
    public monster: Monster | undefined;
    public mode: 'normal' | 'gem' = 'normal';
    public newItem: Item = new Item();
    public gemOption: any = {};

    constructor(
        private _itemTemplateService: ItemTemplateService,
        private _nhbkDialogService: NhbkDialogService,
        private dialog: MatDialog
    ) {
    }

    onSelectTab(index: number) {
        if (index === 0) {
            this.setMode('normal');
        } else if (index === 1) {
            this.setMode('gem');
        }
    }

    openDialogForCharacter(character: Character) {
        this.loot = undefined;
        this.monster = undefined;
        this.character = character;
        this.openDialog();
    }

    openDialogForLoot(loot: Loot) {
        this.loot = loot;
        this.monster = undefined;
        this.character = undefined;
        this.openDialog();
    }

    openDialogForMonster(monster: Monster) {
        this.loot = undefined;
        this.monster = monster;
        this.character = undefined;
        this.openDialog();
    }

    openDialog() {
        this.newItem = new Item();
        this.addItemOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addItemDialog);
    }

    closeDialog() {
        if (!this.addItemOverlayRef) {
            return;
        }
        this.addItemOverlayRef.detach();
        this.addItemOverlayRef = undefined;
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
        } else {
            if (this.mode === 'gem') {
                this.randomGem();
            }
        }
    }

    selectItemTemplate(itemTemplate: ItemTemplate, input?) {
        this.newItem.template = itemTemplate;
        if (!this.newItem.data.notIdentified) {
            this.newItem.data.name = itemTemplate.name;
            if (input) {
                input.focus();
            }
        } else {
            if (this.newItem.template && this.newItem.template.data && this.newItem.template.data.notIdentifiedName) {
                this.newItem.data.name = this.newItem.template.data.notIdentifiedName;
            }
        }
        if (itemTemplate.data.icon) {
            this.newItem.data.icon = JSON.parse(JSON.stringify(itemTemplate.data.icon));
        } else {
            this.newItem.data.icon = undefined;
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
        } else {
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
        } else {
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

    openSelectIconDialog() {
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(IconSelectorComponent, {
            data: {icon: this.newItem.data.icon}
        });

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.newItem.data.icon = icon;
        })
    }

    updateGem() {
        if (this.gemOption['type'] && this.gemOption['number']) {
            const categoryTechName = this.gemOption['type'] === 'cut' ? 'CUT_GEM' : 'RAW_GEM';
            this._itemTemplateService.getItemTemplatesByCategoryTechName(categoryTechName).subscribe(itemTemplates => {
                const itemTemplate = itemTemplates.find(i => i.data.diceDrop === this.gemOption['number']);
                if (!itemTemplate) {
                    console.log('Could not find ' + this.gemOption['type'] + ' gem number ' + this.gemOption['number']);
                    return;
                }
                this.newItem.template = itemTemplate;
                this.newItem.data.icon = itemTemplate.data.icon;
                if (this.newItem.data.notIdentified) {
                    this.newItem.data.name = 'Pierre précieuse';
                } else {
                    this.newItem.data.name = itemTemplate.name;
                }
                this.newItem.data.description = itemTemplate.data.description;
                this.newItem.data.quantity = 1;
            });
        }
    }
}
