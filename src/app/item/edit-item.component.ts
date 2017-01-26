import {Component, OnInit, HostListener} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {NotificationsService} from '../notifications';

import {ItemTemplate} from './item-template.model';
import {ItemService} from './item.service';

@Component({
    selector: 'edit-item',
    templateUrl: './edit-item.component.html',
})
export class EditItemComponent implements OnInit {
    private item: ItemTemplate;
    private saving: boolean = false;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _notification: NotificationsService
        , private _itemService: ItemService) {
    }

    @HostListener('window:keydown', ['$event'])
    keyboardInput(event: KeyboardEvent) {
        if (event.srcElement.tagName === 'BODY') {
            if (event.code === 'Enter') {
                if (event.ctrlKey) {
                    this.edit(event.shiftKey);
                }
            }
        }
    }

    edit(showNext: boolean) {
        this.saving = true;
        this._itemService.editItemTemplate(this.item).subscribe(
            item => {
                this._notification.success('Objet', 'Objet bien sauvegarde: ' + item.name);
                this.saving = false;
                this._itemService.getSectionFromCategory(item.category).subscribe(
                    section => {
                        console.log(section);
                        let sectionId = null;
                        if (section) {
                            sectionId = section.id;
                            this._itemService.clearItemSectionCache(section.id);
                        } else {
                            sectionId = 1;
                        }
                        setTimeout((function () {
                            if (showNext) {
                                this._router.navigate(['/edit-item', this.item.id + 1]);
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
            this._itemService.getItem(id).subscribe(
                item => {
                    this.item = item;
                }
            );

        });
    }
}
