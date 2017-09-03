import {Component, OnInit, Input, EventEmitter, Output, SimpleChanges, OnChanges, ViewChild} from '@angular/core';
import {Overlay, OverlayRef, OverlayState} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {IconService} from './icon.service';
import {removeDiacritics} from './remove_diacritics';
import {IconDescription} from './icon.model';

@Component({
    selector: 'icon-selector',
    templateUrl: './icon-selector.component.html',
    styleUrls: ['./icon-selector.component.scss'],
})
export class IconSelectorComponent implements OnInit, OnChanges {
    @Input() icon: IconDescription;
    @Input() size = '32px';
    @Input() readonly: boolean;
    @Output() onChange: EventEmitter<IconDescription> = new EventEmitter<IconDescription>();

    public filter: string;
    public icons: string[];
    public filteredIcons: string[];
    public defaultIcon: IconDescription = {
        name: 'uncertainty',
        color: '#000000',
        rotation: 0
    };
    public iconRot0: IconDescription;
    public iconRot90: IconDescription;
    public iconRot180: IconDescription;
    public iconRot270: IconDescription;
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
    ];

    @ViewChild('iconSelectorDialog')
    public iconSelectorDialog: Portal<any>;
    public iconSelectorOverlayRef: OverlayRef;

    public newIcon: IconDescription = new IconDescription();

    constructor(private _iconService: IconService
                , private _overlay: Overlay) {
    }

    resetNewIcon() {
        if (this.icon && this.icon.name) {
            this.filter = this.icon.name;
        }
        else {
            this.filter = this.defaultIcon.name;
        }
        this.updateFiltered();

        this.newIcon.name = this.icon.name;
        this.newIcon.color = this.icon.color;
        this.newIcon.rotation = this.icon.rotation;
    }

    openIconSelectorDialog() {
        this.resetNewIcon();

        if (this.readonly) {
            return;
        }

        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.iconSelectorDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.iconSelectorOverlayRef = overlayRef;
    }

    closeIconSelectorDialog() {
        this.iconSelectorOverlayRef.detach();
    }

    updateFiltered() {
        if (!this.filter) {
            return;
        }

        let filtered: string[] = [];
        let cleanFilter = removeDiacritics(this.filter);
        for (let i = 0; i < this.icons.length; i++) {
            let iconName = this.icons[i];
            if (iconName.indexOf(cleanFilter) !== -1) {
                filtered.push(iconName);
                if (filtered.length === 30) {
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
        this.onChange.emit(this.newIcon);
        this.closeIconSelectorDialog();
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('icon' in changes) {
            if (!changes['icon'].currentValue) {
                this.icon = this.defaultIcon;
            }
            this.updateIconRotation();
        }
    }

    updateIconRotation() {
        this.iconRot0 = {
            name: this.newIcon.name,
            color: this.newIcon.color,
            rotation: 0
        };
        this.iconRot90 = {
            name: this.newIcon.name,
            color: this.newIcon.color,
            rotation: 90
        };
        this.iconRot180 = {
            name: this.newIcon.name,
            color: this.newIcon.color,
            rotation: 180
        };
        this.iconRot270 = {
            name: this.newIcon.name,
            color: this.newIcon.color,
            rotation: 270
        };
    }

    selectRandom() {
        if (!this.icons) {
            return;
        }

        this.filter = this.icons[Math.floor(Math.random() * this.icons.length)];
        this.updateFiltered();
    }


    ngOnInit() {
        this._iconService.getIcons().subscribe(
            icons => {
                this.icons = icons;
            }
        );
    }
}
