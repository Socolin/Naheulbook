import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ItemTemplate} from './item-template.model';
import {God} from '../shared';

@Component({
    selector: 'app-item-template-details',
    templateUrl: './item-template-details.component.html',
    styleUrls: ['./item-template-details.component.scss']
})
export class ItemTemplateDetailsComponent implements OnInit {
    @Input() itemTemplate: ItemTemplate;
    @Input() originsName: { [originId: string]: string };
    @Input() jobsName: { [jobId: string]: string };
    @Input() godsByTechName: { [techName: string]: God };

    constructor() {
    }

    ngOnInit(): void {
    }

}
