import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';

@Component({
    selector: 'origin',
    templateUrl: './origin.component.html',
    styleUrls: ['./origin.component.scss'],
})
export class OriginComponent {
    @Input() origin: Origin;
}
