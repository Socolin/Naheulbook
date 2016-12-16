import {Component, OnInit, Input, EventEmitter, Output, SimpleChanges, OnChanges} from '@angular/core';
import {IconService} from './icon.service';
import {removeDiacritics} from './remove_diacritics';
import {IconDescription} from './icon.model';

@Component({
    selector: 'icon-selector',
    templateUrl: './icon-selector.component.html',
    styles: [`
        .selector-tab-content {
            background: whitesmoke;
        }
        .selector-panel {
            position: absolute; 
            width: 80vw; 
            z-index: 100;
        }
        @media screen and (min-width: 768px) {
            .selector-panel{
                width: 40vw; 
            }
        }`],
})
export class IconSelectorComponent implements OnInit, OnChanges {
    @Input() icon: IconDescription;
    @Input() readonly: boolean;
    @Output() onChange: EventEmitter<IconDescription> = new EventEmitter<IconDescription>();
    public viewSelector: boolean = false;
    public tab: string = 'icon';
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
    public iconRot280: IconDescription;
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

    constructor(private _iconService: IconService) {
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

    selectTab(tab: string) {
        this.tab = tab;
    }

    selectIcon(iconName: string) {
        this.onChange.emit({
            name: iconName,
            color: this.icon.color,
            rotation: this.icon.rotation
        });
        this.hideSelector();
    }

    selectColor(color: string) {
        this.onChange.emit({
            name: this.icon.name,
            color: color,
            rotation: this.icon.rotation
        });
        this.hideSelector();
    }

    selectRotation(rotation: number) {
        this.onChange.emit({
            name: this.icon.name,
            color: this.icon.color,
            rotation: rotation
        });
        this.hideSelector();
    }

    showSelector(event: any) {
        if (!this.readonly) {
            this.viewSelector = true;
        }
        event.preventDefault();
    }

    hideSelector(event?: any) {
        this.viewSelector = false;
        if (event) {
            event.preventDefault();
        }
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
            name: this.icon.name,
            color: this.icon.color,
            rotation: 0
        };
        this.iconRot90 = {
            name: this.icon.name,
            color: this.icon.color,
            rotation: 90
        };
        this.iconRot180 = {
            name: this.icon.name,
            color: this.icon.color,
            rotation: 180
        };
        this.iconRot280 = {
            name: this.icon.name,
            color: this.icon.color,
            rotation: 280
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
                this.selectRandom();
            }
        );
    }
}
