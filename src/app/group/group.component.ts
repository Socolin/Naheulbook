import {Component, OnInit, OnChanges, SimpleChanges, OnDestroy, ViewChild} from '@angular/core';
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
import {MdTabChangeEvent, OverlayRef, Portal, Overlay, OverlayState} from '@angular/material';
import {User} from '../user/user.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    templateUrl: './group.component.html',
    styleUrls: ['./group.component.scss'],
    providers: [GroupActionService, GroupWebsocketService],
})
export class GroupComponent implements OnInit, OnChanges, OnDestroy {
    public group: Group;
    public characters: Character[] = [];

    public currentTabIndex: number = 0;
    public currentTab: string = 'infos';
    public tabs: Array<{hash: string}> = [
        {hash: 'infos'},
        {hash: 'characters'},
        {hash: 'combat'},
        {hash: 'loot'},
        {hash: 'history'},
    ];

    public autocompleteLocationsCallback: Function;

    @ViewChild('changeOwnershipDialog')
    public changeOwnershipDialog: Portal<any>;
    public changeOwnershipOverlayRef: OverlayRef;
    public selectedCharacter: Character;
    public changeOwnershipNewOwner: User;
    public filterSearchUser: string = null;
    public filteredUsers: Object[] = [];

    @ViewChild('inviteCharacterModal')
    public inviteCharacterModal: Portal<any>;
    public inviteCharacterOverlayRef: OverlayRef;
    public searchNameInvite: string;
    public filteredInvitePlayers: CharacterInviteInfo[] = [];
    public selectedInviteCharacter: Character;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , public _loginService: LoginService
        , private _groupService: GroupService
        , private _locationService: LocationService
        , private _notification: NotificationsService
        , private _actionService: GroupActionService
        , private _nhbkDialogService: NhbkDialogService
        , private _groupWebsocketService: GroupWebsocketService
        , private _characterService: CharacterService) {
    }


    /* Tabs */

    getTabIndexFromHash(hash: string): number {
        return this.tabs.findIndex(t => t.hash === hash);
    }

    selectTab(tabChangeEvent: MdTabChangeEvent): boolean {
        if (tabChangeEvent.index < this.tabs.length) {
            this.currentTab = this.tabs[tabChangeEvent.index].hash;
            window.location.hash = this.currentTab;
        }
        return false;
    }

    /* Infos tab */

    refreshData() {
        this.loadGroup(this.group.id);
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
                    this._actionService.emitAction('dateChanged', this.group);
                }
                this.group.data = data;
            }
        );
    }

    addTime(dateOffset: NhbkDateOffset) {
        this._groupService.addTime(this.group.id, dateOffset).subscribe(
            data => {
                this.group.data = data;
                this._actionService.emitAction('dateChanged', this.group);
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

    /* Characters tab */

    createNpc() {
        this._router.navigate(['/character/create'], {queryParams: {isNpc: true, groupId: this.group.id}});
        return false;
    }

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

    openChangeOwnershipDialog(character: Character) {
        this.selectedCharacter = character;

        this.changeOwnershipOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.changeOwnershipDialog);
    }

    closeChangeOwnershipDialog() {
        this.changeOwnershipOverlayRef.detach();
    }

    changeOwnershipConfirm() {
        this.setOwnership(this.changeOwnershipNewOwner);
        this.closeChangeOwnershipDialog();
    }

    openInviteCharacterModal() {
        this.inviteCharacterOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.inviteCharacterModal);
    }

    closeInviteCharacterModal() {
        this.inviteCharacterOverlayRef.detach();
    }

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

    /* Combat tab */

    startCombat() {
        this.changeGroupValue('inCombat', true);
    }

    endCombat() {
        this.changeGroupValue('inCombat', false);
    }


    /* Misc */

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

    ngOnInit() {
        this._route.params.subscribe(
            params => {
                let id = +params['id'];
                this.loadGroup(id);
            }
        );
        this._route.fragment.subscribe(value => {
            if (value) {
                this.currentTabIndex = this.getTabIndexFromHash(value);
                this.currentTab = value;
            }
        });
        this.autocompleteLocationsCallback = this.updateLocationListAutocomplete.bind(this);
    }
}
