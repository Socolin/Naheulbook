import {Component, Input} from '@angular/core';
import {Origin} from './origin.model';
import {GmModeService} from '../shared';

@Component({
    selector: 'origin',
    templateUrl: './origin.component.html',
    styleUrls: ['./origin.component.scss'],
    standalone: false
})
export class OriginComponent {
    @Input() origin: Origin;

    constructor(public readonly gmModeService: GmModeService) {
    }
}
