import {ChangeDetectionStrategy, Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {forkJoin, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {MatCard, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle} from '@angular/material/card';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {UserAdminResponse} from '../api/responses';
import {SearchUserByIdentifiersRequest} from '../api/requests';
import {LoginService} from '../user';
import {NhbkMatDialog} from '../material-workaround';
import {NotificationsService} from '../notifications';
import {AdminUserService} from './admin-user.service';
import {DeleteUserDialogComponent, DeleteUserDialogData} from './delete-user-dialog.component';

type IdentifierType = keyof SearchUserByIdentifiersRequest;

const SUPER_ADMIN_LEVEL = 2;

@Component({
    templateUrl: './admin-users.component.html',
    styleUrls: ['./admin-users.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [FormsModule, MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatFormField, MatLabel, MatInput, MatSelect, MatOption, MatButton, MatIcon, MatProgressSpinner]
})
export class AdminUsersComponent {
    public readonly identifierTypes: { value: IdentifierType | 'all', label: string }[] = [
        {value: 'all', label: 'Tous les identifiants'},
        {value: 'facebookId', label: 'Facebook'},
        {value: 'googleId', label: 'Google'},
        {value: 'microsoftId', label: 'Microsoft'},
        {value: 'twitterId', label: 'Twitter'},
        {value: 'username', label: 'Nom d\'utilisateur'},
    ];
    public identifierType: IdentifierType | 'all' = 'all';
    public identifier = '';

    public loading = false;
    public deletingUserId?: number;
    /** `undefined` until a first search is done */
    public users?: UserAdminResponse[];

    constructor(
        private readonly adminUserService: AdminUserService,
        private readonly loginService: LoginService,
        private readonly dialog: NhbkMatDialog,
        private readonly notifications: NotificationsService,
    ) {
    }

    get isSuperAdmin(): boolean {
        return (this.loginService.currentLoggedUser?.adminLevel ?? 0) >= SUPER_ADMIN_LEVEL;
    }

    search() {
        const identifier = this.identifier.trim();
        if (!identifier) {
            return;
        }

        this.loading = true;
        this.searchUsers(identifier).subscribe({
            next: users => {
                this.users = users;
                this.loading = false;
            },
            error: err => {
                this.loading = false;
                this.notifications.error('Erreur lors de la recherche', err);
            }
        });
    }

    openDeleteDialog(user: UserAdminResponse) {
        const dialogRef = this.dialog.open<DeleteUserDialogComponent, DeleteUserDialogData, boolean>(
            DeleteUserDialogComponent,
            {data: {user}}
        );
        dialogRef.afterClosed().subscribe(confirmed => {
            if (!confirmed) {
                return;
            }
            this.deleteUser(user);
        });
    }

    isCurrentUser(user: UserAdminResponse): boolean {
        return this.loginService.currentLoggedUser?.id === user.id;
    }

    private deleteUser(user: UserAdminResponse) {
        this.deletingUserId = user.id;
        this.adminUserService.deleteUser(user.id).subscribe({
            next: () => {
                this.deletingUserId = undefined;
                this.users = this.users?.filter(u => u.id !== user.id);
                this.notifications.success('Utilisateur supprimé', `L'utilisateur #${user.id} a été supprimé`);
            },
            error: err => {
                this.deletingUserId = undefined;
                this.notifications.error('Erreur lors de la suppression de l\'utilisateur', err);
            }
        });
    }

    /**
     * The API matches all the given identifiers, so searching in all of them requires one search per identifier
     */
    private searchUsers(identifier: string): Observable<UserAdminResponse[]> {
        if (this.identifierType !== 'all') {
            return this.adminUserService.searchByIdentifiers({[this.identifierType]: identifier});
        }

        const types = this.identifierTypes
            .map(t => t.value)
            .filter((t): t is IdentifierType => t !== 'all');
        return forkJoin(types.map(type => this.adminUserService.searchByIdentifiers({[type]: identifier}))).pipe(
            map(results => {
                const usersById = new Map<number, UserAdminResponse>();
                for (const user of results.flat()) {
                    usersById.set(user.id, user);
                }
                return [...usersById.values()].sort((a, b) => a.id - b.id);
            })
        );
    }
}
