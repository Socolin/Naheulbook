import {Component, OnInit, OnChanges, SimpleChanges, OnDestroy} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Observable} from 'rxjs';

import {NotificationsService} from '../notifications';

import {Group, CharacterInviteInfo} from '.';
import {GroupService} from './group.service';
import {Character, CharacterService, CharacterResume} from '../character';

import {LoginService} from '../user';
import {AutocompleteValue} from '../shared';
import {LocationService, Location} from '../location';
import {NhbkDateOffset} from '../date';
import {GroupActionService} from './group-action.service';
import {GroupData} from './group.model';
import {GroupWebsocketService} from './group.websocket.service';

@Component({
    templateUrl: 'group.component.html',
    providers: [GroupActionService, GroupWebsocketService],
})
export class GroupComponent implements OnInit, OnChanges, OnDestroy {
    public group: Group;
    public characters: Character[] = [];
    public selectedCharacter: Character;
    public filteredInvitePlayers: CharacterInviteInfo[] = [];
    public autocompleteLocationsCallback: Function;
    public currentTab: string = 'characters';

    /* Invitation tab */

    // Replace with modal
    public searchNameInvite: string;
    public selectedInviteCharacter: Character;
    //FIXME: Replace with autocomplete component
    public filteredUsers: Object[] = [];
    public filterSearchUser: string = null;

    /* History tab */

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , private _loginService: LoginService
        , private _groupService: GroupService
        , private _locationService: LocationService
        , private _notification: NotificationsService
        , private _actionService: GroupActionService
        , private _groupWebsocketService: GroupWebsocketService
        , private _characterService: CharacterService) {
    }

    /* Useful action (top right) */

    refreshData() {
        this.loadGroup(this.group.id);
    }

    startCombat() {
        this.changeGroupValue('inCombat', true);
    }

    endCombat() {
        this.changeGroupValue('inCombat', false);
    }

    /* Players tab */

    activeAllCharacter(active: boolean) {
        for (let i = 0; i < this.characters.length; i++) {
            let character = this.characters[i];
            this._characterService.changeGmData(character.id, 'active', active).subscribe(
                change => {
                    character.active = change.value;
                    this._actionService.emitAction('reorderFighters', this.group);
                }
            );
        }
    }

    toggleActiveCharacter(character) {
        this._characterService.changeGmData(character.id, 'active', !character.active).subscribe(
            change => {
                character.active = change.value;
                this._actionService.emitAction('reorderFighters', this.group);
            }
        );
    }

    takeOwnership(character) {
        this._characterService.changeGmData(character.id, 'owner', 0).subscribe(
            change => {
                character.user = change.value;
                this._notification.success('Modification personnage', 'Ce personnage vous appartient a présent');
            }
        );
    }

    setOwnership(user) {
        let character = this.selectedCharacter;
        this._characterService.changeGmData(character.id, 'owner', user.id).subscribe(
            change => {
                character.user = change.value;
                this._notification.success('Modification personnage', 'Ce personnage a changé de propriétaire');
            }
        );
    }

    addTime(dateOffset: NhbkDateOffset) {
        this._groupService.addTime(this.group.id, dateOffset).subscribe(
            data => {
                this.group.data = data;
            }
        );
    }

    changeGroupLocation(location: Location) {
        this._groupService.editGroupValue(this.group.id, 'location', location.id).subscribe(
            () => {
                this._notification.info('Lieu', this.group.location.name + ' -> ' + location.name);
                this.group.location = location;
            }
        );
    }

    updateLocationListAutocomplete(filter: string) {
        return this._locationService.searchLocations(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    /* Invitation tab */

    updateFilteredPlayer() {
        if (!this.searchNameInvite) {
            this.filteredInvitePlayers = [];
            return;
        }
        this._characterService.searchPlayersForInvite(this.searchNameInvite, this.group.id).subscribe(
            res => {
                this.filteredInvitePlayers = res;
            }
        );
    }

    selectInviteCharacter(character) {
        this.selectedInviteCharacter = character;
        return false;
    }

    _removeFromInvited(character): void {
        let idx = this.group.invited.findIndex(char => char.id === character.id);
        if (idx !== -1) {
            this.group.invited.splice(idx, 1);
        }
    }

    _removeFromInvites(character): void {
        let idx = this.group.invites.findIndex(char => char.id === character.id);
        if (idx !== -1) {
            this.group.invites.splice(idx, 1);
        }
    }

    cancelInvite(character): boolean {
        this._characterService.cancelInvite(character.id, this.group.id).subscribe(
            res => {
                this._removeFromInvited(res.character);
                this._removeFromInvites(res.character);
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
            }
        );
        return false;
    }

    updateSearchUser() {
        if (this.filterSearchUser) {
            this._loginService.searchUser(this.filterSearchUser).subscribe(
                res => {
                    this.filteredUsers = res;
                }
            );
        } else {
            this.filteredUsers = [];
        }
    }

    createNpc() {
        this._router.navigate(['/character/create'], {queryParams: {isNpc: true, groupId: this.group.id}});
        return false;
    }

    changeGroupValue(key: string, value: any) {
        this._groupService.editGroupValue(this.group.id, key, value).subscribe(
            data => {
                if (key === 'debilibeuk') {
                    this._notification.info('Debilibeuk', this.group.data[key] + ' -> ' + value);
                } else if (key === 'mankdebol') {
                    this._notification.info('Mankdebol', this.group.data[key] + ' -> ' + value);
                } else if (key === 'inCombat') {
                    this._notification.info('Mode combat: ', this.group.data[key] + ' -> ' + value);
                    if (value) {
                        this._actionService.emitAction('onStartCombat', this.group);
                    }
                    else {
                        this._actionService.emitAction('onStopCombat', this.group);
                    }
                } else if (key === 'date') {
                    this._notification.info('Date', 'Date changé');
                }
                this.group.data = data;
            }
        );
    }

    loadGroup(id: number) {
        this._characterService.getGroup(id).subscribe(
            group => {
                if (!group.data) {
                    group.data = new GroupData();
                }
                this.group = group;

                let toLoad: Observable<Character>[] = [];
                for (let i = 0; i < group.characters.length; i++) {
                    let char = group.characters[i];
                    toLoad.push(this._characterService.getCharacter(char.id));
                }

                Observable.forkJoin(toLoad).subscribe((characters: Character[]) => {
                    this.characters = characters;
                    for (let i = 0; i < this.characters.length; i++) {
                        let character = this.characters[i];
                        character.onUpdate.subscribe(c => {
                            // FIXME: Copy reference may not be the best ?
                            this.characters[i] = c;
                            this._actionService.emitAction('reorderFighters', this.group);
                        });
                    }
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
                        }
                    );
                    this.registerWs();
                });
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error('Erreur', errJson.error_code, {timeOut: -1});
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error('Erreur', 'Erreur');
                }
            }
        );
    }

    registerWs() {
        this._groupWebsocketService.register(this.group.id);
        this._groupWebsocketService.registerNotifyFunction(message => this._notification.info('Groupe', message));
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('group' in changes) {
            this._actionService.emitAction('reorderFighters', this.group);
        }
    }

    ngOnDestroy(): void {
        this._groupWebsocketService.unregister(this.group.id);
    }

    selectTab(tab: string) {
        this.currentTab = tab;
        window.location.hash = tab;
        return false;
    }

    ngOnInit() {
        this._route.params.subscribe(
            params => {
                let id = +params['id'];
                this.loadGroup(id);
            }
        );
        this._route.fragment.subscribe(value => {
            if (value) {
                this.currentTab = value;
            }
        });
        this.autocompleteLocationsCallback = this.updateLocationListAutocomplete.bind(this);
    }
}
