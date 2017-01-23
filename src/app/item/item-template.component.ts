import {Component, Input} from '@angular/core';
import {ItemTemplate} from './item-template.model';
import {Router} from '@angular/router';

@Component({
    selector: 'item-template',
    styleUrls: ['./item-template.component.scss'],
    templateUrl: './item-template.component.html',
})
export class ItemTemplateComponent {
    @Input() item: ItemTemplate;
    @Input() editable: boolean;
    @Input() originsName: {[originId: number]: string};
    @Input() jobsName: {[jobId: number]: string};

    constructor(private _router: Router) {
    }

    editItem(item: ItemTemplate) {
        this._router.navigate(['/edit-item', item.id], {queryParams: {}});
    }
}
