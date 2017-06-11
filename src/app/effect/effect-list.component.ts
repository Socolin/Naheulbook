import {
    Component, SimpleChanges, Input, OnInit, OnChanges, OnDestroy, Output, EventEmitter,
    ViewChild
} from '@angular/core';
import {Router} from '@angular/router';
import {Subscription} from 'rxjs/Rx';

import {LoginService} from '../user';

import {EffectService} from './effect.service';
import {EffectCategory, Effect} from './effect.model';
import {MdTabGroup} from '@angular/material';
import {isNullOrUndefined} from 'util';

@Component({
    selector: 'effect-list',
    templateUrl: './effect-list.component.html',
    styleUrls: ['../shared/number-shadow.scss'],
})
export class EffectListComponent implements OnInit, OnChanges, OnDestroy {
    @Input() inputCategoryId = null;
    @Input() isOverlay = false;
    @Input() options: string[] = [];
    @Output() onAction = new EventEmitter<{action: string, data: any}>();

    public categories: EffectCategory[];
    public effects: {[categoryId: number]: Effect[]} = {};
    public editable = false;
    public sub: Subscription;
    private firstSelect = true;

    @ViewChild('effectTypesTabGroup')
    public effectTypesTabGroup: MdTabGroup;

    constructor(private _router: Router
        , private _loginService: LoginService
        , private _effectService: EffectService) {
    }

    getCategoryIndex(categoryId: number) {
        return this.categories.findIndex(c => c.id === categoryId);
    }

    selectCategory(category: EffectCategory): boolean {
        this.loadCategory(category.id);
        return false;
    }

    selectCategoryId(categoryId: number) {
        let i = this.getCategoryIndex(categoryId);
        if (i === -1) {
            i = 0;
        }
        if (this.firstSelect) {
            this.effectTypesTabGroup.selectedIndex = i;
            this.firstSelect = false;
        }
        this.selectCategory(this.categories[i]);
    }

    selectTab(index: number) {
        let category = this.categories[index];
        if (!this.isOverlay) {
            this._router.navigate(['/database/effects'], {queryParams: {id: category.id}});
        }
        this.selectCategory(category);
    }

    hasOption(option: string): boolean {
        return this.options.indexOf(option) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }

    editEffect(effect: Effect) {
        this._router.navigate(['/database/edit-effect', effect.id]);
    }

    loadCategory(categoryId: number): void {
        if (!isNullOrUndefined(categoryId)) {
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
                    this.selectCategoryId(+params['id']);
                });
            }
        });
        this._loginService.loggedUser.subscribe(
        user => {
            this.editable = user && user.admin;
        });
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
