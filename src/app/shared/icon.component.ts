import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {IconDescription} from './icon.model';

@Component({
    selector: 'icon',
    templateUrl: './icon.component.html',
    styleUrls: ['./icon.component.scss']
})
export class IconComponent implements OnChanges {
    @Input() icon: IconDescription;
    @Input() size = '32px';
    @Input() enchanted: boolean;

    public defaultIcon: IconDescription = {
        name: 'uncertainty',
        color: '#000000',
        rotation: 0
    };

    ngOnChanges(changes: SimpleChanges) {
        if ('icon' in changes) {
            if (!changes['icon'].currentValue) {
                this.icon = this.defaultIcon;
            }
        }
    }
}
