import {Component, Input, OnChanges, SimpleChanges, ChangeDetectionStrategy} from '@angular/core';
import {IconDescription} from './icon.model';
import { NgClass } from '@angular/common';
import { MatIcon } from '@angular/material/icon';

const defaultIcon: IconDescription = {
    name: 'uncertainty',
    color: '#000000',
    rotation: 0
};
@Component({
    selector: 'icon',
    templateUrl: './icon.component.html',
    styleUrls: ['./icon.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [NgClass, MatIcon]
})
export class IconComponent {
    get displayedIcon() {
        return this.icon || defaultIcon;
    }
    @Input() icon?: IconDescription;
    @Input() size = '32px';
    @Input() enchanted?: boolean;
    @Input() notIdentified?: boolean;
    @Input() shownToGm?: boolean;
}
