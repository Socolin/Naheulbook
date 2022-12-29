export interface UserAccessTokenResponse {
    id: string;
    name: string;
    dateCreated: string;
}

export interface UserAccessTokenResponseWithKey extends UserAccessTokenResponse {
    key: string;
}
