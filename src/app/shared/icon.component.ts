import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {IconDescription} from './icon.model';

const defaultIcon: IconDescription = {
    name: 'uncertainty',
    color: '#000000',
    rotation: 0
};

@Component({
    selector: 'icon',
    templateUrl: './icon.component.html',
    styleUrls: ['./icon.component.scss']
})
export class IconComponent implements OnChanges {
    @Input() icon?: IconDescription = defaultIcon;
    @Input() size = '32px';
    @Input() enchanted: boolean;

    ngOnChanges(changes: SimpleChanges) {
        if ('icon' in changes) {
            if (!changes['icon'].currentValue) {
                this.icon = defaultIcon;
            }
        }
    }
}
