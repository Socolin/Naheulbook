import {Component, Input} from '@angular/core';
import {CriticalData} from './useful-data.model';

@Component({
    selector: 'data-array',
    styleUrls: ['./data-array.component.scss'],
    templateUrl: './data-array.component.html',
    standalone: false
})
export class DataArrayComponent {
    @Input() labels: string[];
    @Input() datas: CriticalData[];
}
