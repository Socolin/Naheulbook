import {Component, SimpleChanges} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router'

import {ModifierPipe} from "../shared";
import {LoginService} from "../user";

import {EffectService} from "./effect.service";
import {EffectCategory, Effect} from "./effect.model";

@Component({
    selector: 'effect-list',
    templateUrl: 'app/effect/effect-list.component.html',
    inputs: ["inputCategoryId"],
    pipes: [ModifierPipe]
})
export class EffectListComponent {
    public selectedCategory: EffectCategory;
    public categories: EffectCategory[];
    public effects: Effect[] = [];
    public editable: boolean = false;
    public inputCategoryId: number = 1;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _loginService: LoginService
        , private _effectService: EffectService) {
    }

    isOverlay(): boolean {
        return this._route.snapshot.url.length == 0 || this._route.snapshot.url[0].path !== 'database';
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
            for (let i = 0; i < this.categories.length; i++) {
                let category = this.categories[i];
                if (category.id == categoryId) {
                    this.selectedCategory = category;
                    this._effectService.getEffects(categoryId).subscribe(
                        effects => {
                            this.effects = effects;
                        }
                    );
                    break;
                }
            }
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
                this._router.routerState.queryParams.subscribe(params => {
                    let id = +params['id'];
                    this.loadCategory(id);
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
}
