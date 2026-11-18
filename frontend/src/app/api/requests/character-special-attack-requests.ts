export type AddCharacterSpecialAttackRequest = {
    specialAttackId: string;
}

export type UseCharacterSpecialAttackRequest = {
    success: boolean;
}

export type UpdateCharacterSpecialAttackRequest = {
    successCount?: number;
    resetCooldowns?: boolean;
}
