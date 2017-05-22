import {Component, SimpleChanges, Input, OnInit, OnChanges, OnDestroy} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs/Rx';

import {LoginService} from '../user';

import {EffectService} from './effect.service';
import {EffectCategory, Effect} from './effect.model';
import {MdTabChangeEvent} from '@angular/material';

@Component({
    selector: 'effect-list',
    templateUrl: './effect-list.component.html',
    styleUrls: ['../shared/number-shadow.scss'],
})
export class EffectListComponent implements OnInit, OnChanges, OnDestroy {
    @Input() inputCategoryId = null;
    @Input() isOverlay = false;

    public categories: EffectCategory[];
    public effects: {[categoryId: number]: Effect[]} = {};
    public editable = false;
    public sub: Subscription;
    public currentTabIndex = 0;

    constructor(private _router: Router
        , private _loginService: LoginService
        , private _effectService: EffectService) {
    }

    selectCategory(category: EffectCategory): boolean {
        if (this.isOverlay) {
            this.loadCategory(category.id);
        } else {
            this._router.navigate(['/database/effects'], {queryParams: {id: category.id}});
        }
        return false;
    }

    selectCategoryId(categoryId: number) {
        for (let i = 0; i < this.categories.length; i++) {
            if (this.categories[i].id === categoryId) {
                this.selectTab(i);
                break;
            }
        }
    }

    selectTab(index: number) {
        this.selectCategory(this.categories[index]);
        this.currentTabIndex = index;
    }

    editEffect(effect: Effect) {
        this._router.navigate(['/database/edit-effect', effect.id]);
    }

    loadCategory(categoryId: number): void {
        if (categoryId) {
            this._effectService.getEffects(categoryId).subscribe(
                effects => {
                    this.effects[categoryId] = effects;
                }
            );
        }
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('inputCategoryId' in changes && !changes['inputCategoryId'].isFirstChange()) {
            this.selectCategoryId(+changes['inputCategoryId'].currentValue);
        }
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
            if (this.isOverlay) {
                this.selectCategoryId(this.inputCategoryId);
            } else {
                this.sub = this._router.routerState.root.queryParams.subscribe(params => {
                    let id = +params['id'];
                    if (!id) {
                        id = this.categories[0].id;
                    }
                    this.selectCategoryId(id);
                });
            }
        });
        this._loginService.loggedUser.subscribe(
            user => {
                this.editable = user && user.admin;
            },
            err => {
                this.editable = false;
                console.log(err);
            });
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
