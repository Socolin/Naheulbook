import {Component, OnInit, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

import {IconService} from './icon.service';
import {removeDiacritics} from './remove_diacritics';
import {IconDescription} from './icon.model';

export interface IconSelectorComponentDialogData {
    icon?: IconDescription;
}

@Component({
    selector: 'icon-selector',
    templateUrl: './icon-selector.component.html',
    styleUrls: ['./icon-selector.component.scss'],
})
export class IconSelectorComponent implements OnInit {
    public filter?: string;
    public icons?: string[];
    public filteredIcons: string[];
    public defaultIcon: IconDescription = {
        name: 'uncertainty',
        color: '#000000',
        rotation: 0
    };
    public rotationIcons: IconDescription[] = [];
    public colors: string[] = [
        '#d6a090',
        '#fe3b1e',
        '#a12c32',
        '#fa2f7a',
        '#fb9fda',
        '#e61cf7',
        '#992f7c',
        '#47011f',
        '#051155',
        '#4f02ec',
        '#2d69cb',
        '#00a6ee',
        '#6febff',
        '#08a29a',
        '#2a666a',
        '#063619',
        '#000000',
        '#4a4957',
        '#8e7ba4',
        '#b7c0ff',
        '#acbe9c',
        '#827c70',
        '#5a3b1c',
        '#ae6507',
        '#f7aa30',
        '#f4ea5c',
        '#9b9500',
        '#566204',
        '#11963b',
        '#51e113',
        '#08fdcc',
        '#d6d6d6',
    ];

    public newIcon: IconDescription;

    constructor(
        private readonly dialogRef: MatDialogRef<IconSelectorComponentDialogData>,
        @Inject(MAT_DIALOG_DATA) public readonly data: IconSelectorComponentDialogData,
        private readonly iconService: IconService,
    ) {
        this.resetNewIcon();
    }

    resetNewIcon() {
        this.newIcon = {...this.data.icon || this.defaultIcon};
        this.filter = this.newIcon.name;
        this.updateFiltered();
        this.updateIconRotation();
    }

    updateFiltered() {
        if (!this.filter || !this.icons) {
            return;
        }

        let filtered: string[] = [];
        let cleanFilter = removeDiacritics(this.filter);
        for (let i = 0; i < this.icons.length; i++) {
            let iconName = this.icons[i];
            if (iconName.indexOf(cleanFilter) !== -1) {
                filtered.push(iconName);
                if (filtered.length === 48) {
                    break;
                }
            }
        }
        this.filteredIcons = filtered;
    }

    selectIcon(iconName: string) {
        this.newIcon.name = iconName;
        this.updateIconRotation();
    }

    selectColor(color: string) {
        this.newIcon.color = color;
        this.updateIconRotation();
    }

    selectRotation(rotation: number) {
        this.newIcon.rotation = rotation;
    }

    saveChange() {
        this.dialogRef.close(this.newIcon)
    }

    updateIconRotation() {
        this.rotationIcons = [
            {...this.newIcon, rotation: 0},
            {...this.newIcon, rotation: 90},
            {...this.newIcon, rotation: 180},
            {...this.newIcon, rotation: 270}
        ];
    }

    selectRandom() {
        if (!this.icons) {
            return;
        }

        this.filter = this.icons[Math.floor(Math.random() * this.icons.length)];
        this.updateFiltered();
    }

    isNewIconTheSame(): boolean {
        if (!this.data.icon) {
            return true;
        }

        return this.newIcon.name === this.data.icon.name
            && this.newIcon.color === this.data.icon.color
            && this.newIcon.rotation === this.data.icon.rotation;
    }

    ngOnInit() {
        this.iconService.getIcons().subscribe(
            icons => {
                this.icons = icons.sort((a, b) => a.length - b.length);
                this.updateFiltered();
            }
        );
    }
}
