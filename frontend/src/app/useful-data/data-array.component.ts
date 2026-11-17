import {Component, Input, ChangeDetectionStrategy} from '@angular/core';
import {CriticalData} from './useful-data.model';

@Component({
    selector: 'data-array',
    styleUrls: ['./data-array.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    templateUrl: './data-array.component.html'
})
export class DataArrayComponent {
    @Input() labels: string[];
    @Input() datas: CriticalData[];
}
