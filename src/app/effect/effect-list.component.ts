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
    @Input() inputCategoryId = 1;

    public categories: EffectCategory[];
    public effects: {[categoryId: number]: Effect[]} = {};
    public editable = false;
    public sub: Subscription;
    public currentTabIndex = 0;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _loginService: LoginService
        , private _effectService: EffectService) {
    }

    isOverlay(): boolean {
        return this._route.snapshot.url.length === 0 || this._route.snapshot.url[0].path !== 'database';
    }

    selectChange(changeEvent: MdTabChangeEvent) {
        this.selectCategory(this.categories[changeEvent.index]);
        this.currentTabIndex = changeEvent.index;
    }

    selectCategory(category: EffectCategory): boolean {
        if (this.isOverlay()) {
            this.loadCategory(category.id);
        } else {
            this._router.navigate(['/database/effects'], {queryParams: {id: category.id}});
        }
        return false;
    }

    editEffect(effect: Effect) {
        this._router.navigate(['/edit-effect', effect.id]);
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
            this.loadCategory(changes['inputCategoryId'].currentValue);
        }
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(res => {
            this.categories = res;
            if (this.isOverlay()) {
                this.loadCategory(this.inputCategoryId);
            } else {
                this.sub = this._router.routerState.root.queryParams.subscribe(params => {
                    let id = +params['id'];
                    if (!id) {
                        id = this.categories[0].id;
                    }
                    this.loadCategory(id);
                    for (let i = 0; i < this.categories.length; i++) {
                        if (this.categories[i].id === id) {
                            this.currentTabIndex = i;
                            break;
                        }
                    }
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
