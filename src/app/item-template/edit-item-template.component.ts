import {Component, OnInit, HostListener} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {NotificationsService} from '../notifications';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';

@Component({
    selector: 'edit-item-template',
    templateUrl: './edit-item-template.component.html',
})
export class EditItemTemplateComponent implements OnInit {
    public item: ItemTemplate;
    public saving = false;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _notification: NotificationsService
        , private _itemTemplateService: ItemTemplateService) {
    }

    @HostListener('window:keydown', ['$event'])
    keyboardInput(event: KeyboardEvent) {
        if (event.srcElement && event.srcElement.tagName === 'BODY') {
            if (event.code === 'Enter') {
                if (event.ctrlKey) {
                    this.edit(event.shiftKey);
                }
            }
        }
    }

    edit(showNext: boolean) {
        this.saving = true;
        this._itemTemplateService.editItemTemplate(this.item).subscribe(
            item => {
                this._notification.success('Objet', 'Objet bien sauvegarde: ' + item.name);
                this.saving = false;
                this._itemTemplateService.getSectionFromCategory(item.category).subscribe(
                    section => {
                        let sectionId: number;
                        if (section) {
                            sectionId = section.id;
                            this._itemTemplateService.clearItemSectionCache(section.id);
                        } else {
                            sectionId = 1;
                        }
                        setTimeout((function () {
                            if (showNext) {
                                this._router.navigate(['/database/edit-item', this.item.id + 1]);
                            } else {
                                this._router.navigate(['/database/items'], {queryParams: {id: sectionId}});
                            }
                        }).bind(this), 300);
                    }
                );
            }
        );
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            let id = +params['id'];
            this._itemTemplateService.getItem(id).subscribe(
                item => {
                    this.item = item;
                }
            );

        });
    }
}
