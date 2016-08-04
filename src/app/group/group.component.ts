import {Component, OnInit, OnChanges} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {Group, GroupService, CharacterInviteInfo} from '../group';
import {Character, CharacterService, CharacterColorSelectorComponent, CharacterComponent} from '../character';
import {Monster, MonsterService, MonsterTemplate, MonsterColorSelectorComponent} from '../monster';

import {LoginService} from '../user';
import {EffectListComponent} from '../effect';
import {SkillListComponent} from '../skill';
import {Fighter} from './group.model';
import {MonsterEditableFieldComponent} from './monster-editable-field.component';
import {ValueEditorComponent, AutocompleteInputComponent, AutocompleteValue} from '../shared';
import {TargetSelectorComponent} from './target-selector.component';
import {LocationService, Location} from '../location';
import {Observable} from 'rxjs';
import {CharacterResume} from '../character/character.model';


@Component({
    moduleId: module.id,
    templateUrl: 'group.component.html',
    directives: [CharacterComponent
        , MonsterEditableFieldComponent
        , CharacterColorSelectorComponent
        , MonsterColorSelectorComponent
        , TargetSelectorComponent
        , EffectListComponent
        , ValueEditorComponent
        , AutocompleteInputComponent
        , SkillListComponent],
    styles: [`
        .even_row {
            background-color: #f9f9f9;
        }
        .combat_row_selected {
            background-color: #e3e3e3!important;
        }
    `],
    providers: [CharacterService, GroupService]
})
export class GroupComponent implements OnInit, OnChanges {
    public group: Group;
    public characters: Character[] = [];
    public newMonster: Monster = new Monster();
    public selectedCharacter: Character;
    public filteredInvitePlayers: CharacterInviteInfo[] = [];
    public charAndMonsters: Fighter[] = [];
    public deadMonsters: Monster[] = [];
    private autocompleteLocationsCallback: Function;
    private selectedCombatRow: number = 0;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , private _loginService: LoginService
        , private _groupService: GroupService
        , private _locationService: LocationService
        , private _notification: NotificationsService
        , private _monsterService: MonsterService
        , private _characterService: CharacterService) {
    }

    private currentPanel: string = null;
    private currentSubPanel: string = null;
    private selectedSubPanels = {};

    toggleDisplayPanel(name: string) {
        if (this.currentPanel === name) {
            this.currentPanel = null;
        } else {
            if (name) {
                this.currentSubPanel = this.selectedSubPanels[name];
            }
            this.currentPanel = name;
        }
        return false;
    }

    selectSubPanel(name) {
        this.currentSubPanel = name;
        this.selectedSubPanels[this.currentPanel] = name;
        return false;
    }

    private effectsCategoryId = 1;

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        this.currentPanel = 'effects';
        return false;
    }

    toggleActiveCharacter(character) {
        this._characterService.changeCharacterStat(character.id, 'active', !character.active).subscribe(
            change => {
                character.active = change.value;
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    takeOwnership(character) {
        this._characterService.changeCharacterStat(character.id, 'owner', 0).subscribe(
            change => {
                character.user = change.value;
                this._notification.success("Modification personnage", "Ce personnage vous appartient a présent");
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    setOwnership(user) {
        let character = this.selectedCharacter;
        this._characterService.changeCharacterStat(character.id, 'owner', user.id).subscribe(
            change => {
                character.user = change.value;
                this._notification.success("Modification personnage", "Ce personnage a changé de propriétaire");
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    public searchNameInvite: string;

    updateFilteredPlayer() {
        if (!this.searchNameInvite) {
            this.filteredInvitePlayers = [];
            return;
        }
        this._characterService.searchPlayersForInvite(this.searchNameInvite, this.group.id).subscribe(
            res => {
                this.filteredInvitePlayers = res;
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code);
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
                }
            }
        );
    }

    addMonster() {
        this._groupService.createMonster(this.group.id, this.newMonster).subscribe(
            monster => {
                this.newMonster = new Monster();
                this.group.monsters.push(monster);
                this.updateOrder();
            },
            err => {
                console.log(err.stack);
                this._notification.error("Erreur", "Erreur");
            }
        );
    }

    killMonster(monster: Monster) {
        this._groupService.killMonster(monster.id).subscribe(
            res => {
                monster.dead = res.dead;
                this.updateOrder();
            },
            err => {
                console.log(err.stack);
                this._notification.error("Erreur", "Erreur");
            }
        );
    }


    private monsterAutocompleteList: MonsterTemplate[] = [];
    private monsterAutocompleteShow = false;

    onMonsterNameChange(name) {
        this._monsterService.searchMonster(name).subscribe(
            res => {
                this.monsterAutocompleteList = res;
                this.monsterAutocompleteShow = true;
            },
            err => {
                console.log(err.stack);
                this._notification.error("Erreur", "Erreur");
            }
        );
    }

    selectMonsterInAutocompleteList(monster: MonsterTemplate) {
        this.monsterAutocompleteShow = false;
        this.newMonster.name = monster.name;
        this.newMonster.data.at = monster.data.at;
        this.newMonster.data.prd = monster.data.prd;
        this.newMonster.data.ev = monster.data.ev;
        this.newMonster.data.ea = 0;
        this.newMonster.data.pr = monster.data.pr;
        this.newMonster.data.cou = monster.data.cou;
        this.newMonster.data.dmg = monster.data.dmg;
        this.newMonster.data.xp = monster.data.xp;
        if (monster.data.resm) {
            this.newMonster.data.resm = monster.data.resm;
        } else {
            this.newMonster.data.resm = 0;
        }
        this.newMonster.data.note = monster.data.note;
        return false;
    }

    private selectedInviteCharacter: Character;

    selectInviteCharacter(character) {
        this.selectedInviteCharacter = character;
        return false;
    }

    _removeFromInvites(character) {
        for (let i = 0; i < this.group.invited.length; i++) {
            let char = this.group.invited[i];
            if (char.id === character.id) {
                this.group.invited.splice(i, 1);
                break;
            }
        }
    }

    _removeFromInvited(character) {
        for (let i = 0; i < this.group.invites.length; i++) {
            let char = this.group.invites[i];
            if (char.id === character.id) {
                this.group.invites.splice(i, 1);
                break;
            }
        }
    }

    cancelInvite(character) {
        this._characterService.cancelInvite(character.id, this.group.id).subscribe(
            res => {
                this._removeFromInvited(res.character);
                this._removeFromInvites(res.character);
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code);
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
                }
            }
        );
        return false;
    }

    inviteCharacter(character) {
        this._characterService.inviteCharacter(this.group.id, character.id).subscribe(
            res => {
                this.group.invited.push(res);
                for (let i = 0; i < this.filteredInvitePlayers.length; i++) {
                    let char = this.filteredInvitePlayers[i];
                    if (char.id === res.id) {
                        this.filteredInvitePlayers.splice(i, 1);
                        break;
                    }
                }
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code);
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
                }
            }
        );
        return false;
    }

    changeTarget(element, target) {
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'target', {id: target.id, isMonster: target.isMonster})
                .subscribe(
                    () => {
                        element.target = target;
                        this._notification.info("Monstre", "Cible changée");
                    },
                    err => {
                        console.log(err);
                        this._notification.error("Erreur", "Erreur serveur");
                    }
                );
        } else {
            this._characterService.changeCharacterStat(element.id, 'target', {
                id: target.id,
                isMonster: target.isMonster
            }).subscribe(
                change => {
                    element.target = change.value;
                    this._notification.info("Joueur", "Cible changée");
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    changeColor(element: Fighter, color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'color', color)
                .subscribe(
                    () => {
                        element.color = color;
                        this._notification.info("Monstre", "Couleur changé");
                    },
                    err => {
                        console.log(err);
                        this._notification.error("Erreur", "Erreur serveur");
                    }
                );
        } else {
            this._characterService.changeCharacterStat(element.id, 'color', color).subscribe(
                change => {
                    element.color = change.value;
                    this._notification.info("Joueur", "Couleur changée");
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    public filteredUsers: Object[] = [];
    public filterSearchUser: string = null;

    updateSearchUser() {
        if (this.filterSearchUser) {
            this._loginService.searchUser(this.filterSearchUser).subscribe(
                res => {
                    this.filteredUsers = res;
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        } else {
            this.filteredUsers = [];
        }
    }

    updateOrder() {
        let deadMonsters = [];
        let charAndMonsters: Fighter[] = [];
        for (let i = 0; i < this.characters.length; i++) {
            let character = this.characters[i];
            if (character.active) {
                let fighter = Fighter.createFromCharacter(character);
                fighter.chercheNoise = Character.hasChercherDesNoises(character);
                charAndMonsters.push(fighter);
            }
        }
        for (let i = 0; i < this.group.monsters.length; i++) {
            let monster = this.group.monsters[i];
            if (monster.dead) {
                deadMonsters.push(monster);
            } else {
                charAndMonsters.push(Fighter.createFromMonster(monster));
            }
        }
        deadMonsters.sort((first, second) => {
            let a = new Date(first.dead);
            let b = new Date(second.dead);
            return a > b ? -1 : a < b ? 1 : 0;
        });

        while (deadMonsters.length > 10) {
            deadMonsters.pop();
        }

        charAndMonsters.sort(function (first, second) {
            if (first.chercheNoise && !second.chercheNoise) {
                return -1;
            }
            else if (!first.chercheNoise && second.chercheNoise) {
                return 1;
            }
            else {
                let cou1 = first.stats.cou;
                let cou2 = second.stats.cou;

                if (cou1 > cou2) {
                    return -1;
                } else if (cou1 < cou2) {
                    return 1;
                } else {
                    let ad1 = first.stats.ad;
                    let ad2 = second.stats.ad;

                    if (ad1 > ad2) {
                        return -1;
                    } else if (ad1 < ad2) {
                        return 1;
                    } else {
                        return 0;
                    }
                }
            }
        });
        this.charAndMonsters = charAndMonsters;
        this.deadMonsters = deadMonsters;
    }

    public historyPage: number = 0;
    public currentDay: string = null;
    public history;
    public loadMore: boolean = true;

    loadHistory(next) {
        if (!next) {
            this.historyPage = 0;
            this.currentDay = null;
            this.history = [];
        }

        this._groupService.loadHistory(this.group.id, this.historyPage).subscribe(
            res => {
                if (res.length === 0) {
                    this.loadMore = false;
                    return;
                }
                this.loadMore = true;
                let logs = [];
                if (this.currentDay) {
                    logs = this.history[this.history.length - 1].logs;
                }
                for (let i = 0; i < res.length; i++) {
                    let l = res[i];
                    l.date = new Date(l.date);

                    let day = l.date.toString().substring(0, 15);
                    if (!this.currentDay || day !== this.currentDay) {
                        this.currentDay = day;
                        logs = [];
                        this.history.push({logs: logs, date: l.date});
                    }
                    logs.push(l);
                }
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
        this.historyPage++;
        return false;
    }

    public historyNewEntryText: string = null;
    public historyNewEntryGm: boolean = false;

    addLog() {
        this._groupService.addLog(this.group.id, this.historyNewEntryText, this.historyNewEntryGm).subscribe(
            () => {
                this.historyNewEntryText = null;
                this._notification.success("Historique", "Entrée ajoutée");
                this.loadHistory(0);
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
        return false;
    }

    ngOnChanges() {
        this.updateOrder();
    }

    refreshData() {
        this._characterService.getGroup(this.group.id).subscribe(
            res => {
                this.group = res;
                this.updateOrder();
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code, {timeOut: -1});
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
                }
            }
        );
    }

    createNpc() {
        this._router.navigate(['/character/create'], {queryParams: {isNpc: true, groupId: this.group.id}});
        return false;
    }

    changeGroupValue(key: string, value: any) {
        this._groupService.editGroupValue(this.group.id, key, value).subscribe(
            () => {
                if (key === 'debilibeuk') {
                    this._notification.info('Debilibeuk', this.group.data[key] + ' -> ' + value);
                    this.group.data[key] = value;
                } else if (key === 'mankdebol') {
                    this._notification.info('Mankdebol', this.group.data[key] + ' -> ' + value);
                    this.group.data[key] = value;
                }
            },
            err => {
                console.log(err);
                this._notification.error('Erreur', 'Erreur serveur');
            }
        );
    }

    changeGroupLocation(location: Location) {
        this._groupService.editGroupValue(this.group.id, 'location', location).subscribe(
            () => {
                this._notification.info('Lieu', this.group.location.name + ' -> ' + location.name);
                this.group.location = location;
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    updateLocationListAutocomplete(filter: string) {
        return this._locationService.searchLocations(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    ngOnInit() {
        this._route.params.subscribe(
            params => {
                let id = +params['id'];
                this._characterService.getGroup(id).subscribe(
                    group => {
                        if (!group.data) {
                            group.data = {};
                        }
                        this.group = group;

                        let toLoad: Observable<Character>[] = [];
                        for (let i = 0; i < group.characters.length; i++) {
                            let char = group.characters[i];
                            toLoad.push(this._characterService.getCharacter(char.id));
                        }
                        Observable.forkJoin(toLoad).subscribe((characters: Character[]) => {
                            this.characters = characters;
                            let charactersId: number[] = [];
                            for (let i = 0; i < group.invited.length; i++) {
                                charactersId.push(group.invited[i].id);
                            }
                            for (let i = 0; i < group.invites.length; i++) {
                                charactersId.push(group.invites[i].id);
                            }
                            this._characterService.loadCharactersResume(charactersId).subscribe(
                                (characterInvite: CharacterResume[]) => {
                                    for (let i = 0; i < group.invited.length; i++) {
                                        let char = group.invited[i];
                                        for (let j = 0; j < characterInvite.length; j++) {
                                            let c = characterInvite[j];
                                            if (c.id === char.id) {
                                                group.invited[i] = c;
                                            }
                                        }
                                    }
                                    for (let i = 0; i < group.invites.length; i++) {
                                        let char = group.invites[i];
                                        for (let j = 0; j < characterInvite.length; j++) {
                                            let c = characterInvite[j];
                                            if (c.id === char.id) {
                                                group.invites[i] = c;
                                            }
                                        }
                                    }
                                    this.updateOrder();
                                }
                            );
                        });
                    },
                    err => {
                        try {
                            let errJson = err.json();
                            this._notification.error("Erreur", errJson.error_code, {timeOut: -1});
                        } catch (e) {
                            console.log(err.stack);
                            this._notification.error("Erreur", "Erreur");
                        }
                    }
                );
            }
        );
        this.autocompleteLocationsCallback = this.updateLocationListAutocomplete.bind(this);

    }
}
