import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ItemTemplate} from './item-template.model';
import {Router} from '@angular/router';

import {God} from '../shared';

@Component({
    selector: 'item-template',
    styleUrls: ['./item-template.component.scss'],
    templateUrl: './item-template.component.html',
})
export class ItemTemplateComponent {
    @Input() itemTemplate: ItemTemplate;
    @Input() editable: boolean;
    @Input() originsName: {[originId: number]: string};
    @Input() jobsName: {[jobId: number]: string};
    @Input() godsByTechName: {[techName: string]: God};
    @Input() actions: string[];
    @Output() onAction = new EventEmitter<{action: string, data: any}>();

    constructor(private _router: Router) {
    }

    editItem(item: ItemTemplate) {
        this._router.navigate(['/database/edit-item', item.id], {queryParams: {}});
    }

    hasAction(actionName: string) {
        return this.actions && this.actions.indexOf(actionName) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }
}
