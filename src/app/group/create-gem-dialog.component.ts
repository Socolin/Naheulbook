import {Component, Inject, OnInit} from '@angular/core';
import {Observer, Subject} from 'rxjs';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Item} from '../item';
import {ItemTemplateService} from '../item-template';
import {getRandomInt, IconSelectorComponent, IconSelectorComponentDialogData} from '../shared';
import {IconDescription} from '../shared/icon.model';
import {NhbkMatDialog} from '../material-workaround';

export interface CreateGemDialogDialog {
    onAdd: Observer<any>;
}

@Component({
    selector: 'app-create-gem-dialog',
    templateUrl: './create-gem-dialog.component.html',
    styleUrls: ['./create-gem-dialog.component.scss']
})
export class CreateGemDialogComponent implements OnInit {
    public newItem = new Item();
    public gemType: 'raw' | 'cut' = 'raw';
    public gemUg = 1;
    public maxUg = 2;
    public randomDiceNumber = 1;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly dialogRef: MatDialogRef<CreateGemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public readonly data: CreateGemDialogDialog,
    ) {
    }

    setGemType(gemType: 'raw' | 'cut') {
        this.gemType = gemType;
        this.updateGem();
    }

    setGemRandomUg(number: number) {
        this.gemUg = getRandomInt(1, number);
        this.maxUg = number;
        this.updateGem();
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
        const subCategoryTechName = this.gemType === 'cut' ? 'CUT_GEM' : 'RAW_GEM';
        this.itemTemplateService.getItemTemplatesBySubCategoryTechName(subCategoryTechName).subscribe(itemTemplates => {
            const itemTemplate = itemTemplates.find(i => i.data.diceDrop === this.randomDiceNumber);
            if (!itemTemplate) {
                console.log('Could not find ' + this.gemType + ' gem number ' + this.randomDiceNumber);
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
            this.newItem.data.ug = this.gemUg;
        });
    }

    setItemNotIdentified() {
        this.newItem.data.notIdentified = true;
        this.newItem.data.name = 'Pierre précieuse';
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

    randomGem() {
        let type = getRandomInt(1, 2);
        if (type === 1) {
            this.setGemType('raw');
        } else {
            this.setGemType('cut');
        }
        this.gemUg = getRandomInt(1, this.maxUg);
        this.randomDiceNumber = getRandomInt(1, 20);
        this.updateGem();
    }

    addItem(close: boolean) {
        this.data.onAdd.next(this.newItem);
        if (close) {
            this.dialogRef.close();
        }
    }

    ngOnInit() {
    }
}

export function openCreateGemDialog(dialog: NhbkMatDialog, onAdd: (item: Item) => void) {
    const subject = new Subject<Item>();
    const dialogRef = dialog.open(CreateGemDialogComponent, {data: {onAdd: subject}});
    const subscription = subject.subscribe((item) => {
        onAdd(item);
    });

    dialogRef.afterClosed().subscribe(() => {
        subscription.unsubscribe();
    });
}
