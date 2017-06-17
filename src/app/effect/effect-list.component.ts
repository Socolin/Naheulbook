import {
    Component, SimpleChanges, Input, OnInit, OnChanges, OnDestroy, Output, EventEmitter,
    ViewChild
} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs/Rx';

import {LoginService} from '../user';

import {EffectService} from './effect.service';
import {EffectCategory, Effect, EffectType} from './effect.model';
import {MdTabGroup} from '@angular/material';
import {isNullOrUndefined} from 'util';

@Component({
    selector: 'effect-list',
    templateUrl: './effect-list.component.html',
    styleUrls: ['../shared/number-shadow.scss', './effect-list.component.scss'],
})
export class EffectListComponent implements OnInit, OnChanges, OnDestroy {
    @Input() inputCategoryId = null;
    @Input() isOverlay = false;
    @Input() options: string[] = [];
    @Output() onAction = new EventEmitter<{action: string, data: any}>();

    public types: EffectType[];
    public selectedType: EffectType;
    public selectedCategory: EffectCategory;
    public effects: {[categoryId: number]: Effect[]} = {};
    public editable = false;
    public sub: Subscription;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _loginService: LoginService
        , private _effectService: EffectService) {
    }

    selectType(type: EffectType) {
        this.selectedType = type;
        if (this.selectedType.categories.length) {
            this.selectCategory(this.selectedType.categories[0]);
        }
    }

    selectCategory(category: EffectCategory): boolean {
        this.selectedCategory = category;
        this.loadCategory(category.id);
        return false;
    }

    selectCategoryId(categoryId: number) {
        let i = this.types.findIndex(t => t.categories.findIndex(c => c.id === categoryId) !== -1);
        if (i === -1) {
            return;
        }
        this.selectedType = this.types[i];
        let ci = this.selectedType.categories.findIndex(c => c.id === categoryId);
        this.selectCategory(this.selectedType.categories[ci]);
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

    createEffect(effect: Effect) {
        this._router.navigate(['/database/create-effect']);
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
        this._effectService.getEffectTypes().subscribe(types => {
            this.types = types;
            if (types.length) {
                this.selectType(types[0]);
            }
            if (this.isOverlay) {
                this.selectCategoryId(this.inputCategoryId);
            } else {
                if (!this._route.snapshot.data['id']) {
                    this.loadCategory(this.selectedCategory.id);
                }
                this.sub = this._route.queryParams.subscribe(params => {
                    this.selectCategoryId(+params['id']);
                });
            }
        });
        if (!this.isOverlay) {
            this._loginService.loggedUser.subscribe(
                user => {
                    this.editable = user && user.admin;
                });
        }
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
