import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';

@Component({
    selector: 'app-origin-gm-info',
    templateUrl: './origin-gm-info.component.html',
    styleUrls: ['./origin-gm-info.component.scss']
})
export class OriginGmInfoComponent {
    @Input() origin: Origin;
}
