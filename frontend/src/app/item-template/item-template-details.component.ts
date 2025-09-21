import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ItemTemplate} from './item-template.model';
import {God} from '../shared';
import { MatIcon } from '@angular/material/icon';
import { MatChipSet, MatChip } from '@angular/material/chips';
import { NhbkActionComponent } from '../action/nhbk-action.component';
import { MatDivider } from '@angular/material/list';
import { ModifierPipe } from '../shared/modifier.pipe';
import { PlusMinusPipe } from '../shared/plus-minus.pipe';
import { NhbkDateDurationPipe } from '../date/nhbk-duration.pipe';
import { ItemTemplateDataProtectionPipe } from './item-template-data-protection.pipe';
import { ItemPricePipe } from './item-price.pipe';

@Component({
    selector: 'app-item-template-details',
    templateUrl: './item-template-details.component.html',
    styleUrls: ['./item-template-details.component.scss'],
    imports: [MatIcon, MatChipSet, MatChip, NhbkActionComponent, MatDivider, ModifierPipe, PlusMinusPipe, NhbkDateDurationPipe, ItemTemplateDataProtectionPipe, ItemPricePipe]
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
