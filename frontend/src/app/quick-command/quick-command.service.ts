import {Injectable} from '@angular/core';
import {QuickAction, CommandSuggestionType} from './quick-action.model';
import {Router} from '@angular/router';
import {GmModeService} from '../shared';

@Injectable({
    providedIn: 'root',
})
export class QuickCommandService {
    private actions: { [categoryId: string]: QuickAction[] } = {};
    private baseActions: (QuickAction & { isGm: boolean })[] = [
        this.createPageSuggestion('Vos personnages', ['/player', 'character', 'list'], 'person', undefined, false),
        this.createPageSuggestion('Vos groupes', ['/gm', 'group', 'list'], 'group', undefined, false),
        this.createPageSuggestion('Database: Compétences', ['/database', 'skills'], 'game-icon-gears', 'game-icon', true),
        this.createPageSuggestion('Database: Métiers', ['/database', 'jobs'], 'game-icon-diploma', 'game-icon', false),
        this.createPageSuggestion('Database: Origines', ['/database', 'origins'], 'game-icon-ogre', 'game-icon', false),
        this.createPageSuggestion('Database: Bestiaires', ['/database', 'monsters'], 'game-icon-dragon-head', 'game-icon', true),
        this.createPageSuggestion('Database: Objets', ['/database', 'items'], 'game-icon-swap-bag', 'game-icon', true),
        this.createPageSuggestion('Database: Effets', ['/database', 'effects'], 'game-icon-broken-bone', 'game-icon', true),
        this.createPageSuggestion('Database: Cartographie', ['/map'], 'game-icon-compass', 'game-icon', false),
    ]

    private allActions: QuickAction[];

    constructor(
        private readonly router: Router,
        private readonly gmModeService: GmModeService
    ) {
        this.updateAllActions();
        this.gmModeService.gmMode.subscribe(gmMode => {
            this.updateAllActions();
        })
    }

    getAllActions(): QuickAction[] {
        return this.allActions;
    }

    registerActions(id: string, actions: QuickAction[]) {
        this.actions[id] = actions;
        this.updateAllActions();
    }

    unregisterActions(id: string) {
        delete this.actions[id];
        this.updateAllActions();
    }

    private updateAllActions() {
        let allActions: QuickAction[] = this.baseActions.filter(a => !a.isGm || (a.isGm && this.gmModeService.gmModeSnapshot));
        allActions = allActions
            .concat(Object.values(this.actions).reduce((acc, val) => acc.concat(val), []))
            .sort((a, b) => a.priority - b.priority);
        this.allActions = allActions;
    }

    private createPageSuggestion(name: string, route: string[], icon: string, fontSet: string | undefined, isGm: boolean): QuickAction & { isGm: boolean } {
        return {
            action: () => this.router.navigate(route),
            icon: icon,
            displayText: name,
            iconFontSet: fontSet,
            type: CommandSuggestionType.Page,
            canBeUsedInRecent: true,
            priority: 100,
            isGm: isGm
        }
    }
}
