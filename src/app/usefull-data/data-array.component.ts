import {Component, Input} from '@angular/core';
import {CriticalData} from './usefull-data.model';

@Component({
    selector: 'data-array',
    styleUrls: ['./data-array.component.scss'],
    templateUrl: './data-array.component.html',
})
export class DataArrayComponent {
    @Input() labels: string[];
    @Input() datas: CriticalData[];
}
