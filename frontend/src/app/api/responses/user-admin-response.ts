export interface UserAdminResponse {
    id: number;
    username?: string;
    displayName?: string;
    adminLevel: number;
    facebookId?: string;
    googleId?: string;
    microsoftId?: string;
    twitterId?: string;
}
