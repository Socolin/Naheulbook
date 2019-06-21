export interface User {
    id: number;
    displayName: string;
    admin: boolean;
    linkedWithFb: boolean;
    linkedWithGoogle: boolean;
    linkedWithTwitter: boolean;
    linkedWithMicrosoft: boolean;
}

export interface JwtResponse {
    token: string;
    userInfo: User;
}
