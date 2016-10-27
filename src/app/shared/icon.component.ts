import {Component, Input, OnChanges, SimpleChanges} from "@angular/core";
import {IconDescription} from "./icon.model";

@Component({
    selector: 'icon',
    templateUrl: 'icon.component.html',
    styles: [
        `.rotat0:before {
            display: inline-block;
        }
        .rotate90:before {
            display: inline-block;
            transform: rotate(90deg) ;
            -webkit-transform: rotate(90deg) ;
            -moz-transform: rotate(90deg) ;
            -o-transform: rotate(90deg) ;
            -ms-transform: rotate(90deg) ;
        }
        .rotate180:before {
            display: inline-block;
            transform: rotate(180deg) ;
            -webkit-transform: rotate(180deg) ;
            -moz-transform: rotate(180deg) ;
            -o-transform: rotate(180deg) ;
            -ms-transform: rotate(180deg) ;
        }
        .rotate280:before {
            display: inline-block;
            transform: rotate(280deg) ;
            -webkit-transform: rotate(280deg) ;
            -moz-transform: rotate(280deg) ;
            -o-transform: rotate(280deg) ;
            -ms-transform: rotate(280deg) ;
        }
        `,
    ]
})
export class IconComponent implements OnChanges {
    @Input() icon: IconDescription;
    @Input() size: string;

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
