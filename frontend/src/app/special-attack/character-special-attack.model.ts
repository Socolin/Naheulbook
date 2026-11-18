import {CharacterSpecialAttackResponse, SpecialAttackCooldown, SpecialAttackResponse} from '../api/responses';

/**
 * A special attack known by a character, with its running cooldowns.
 * The cooldowns use the same duration fields as the modifiers, so they are decremented by the group duration updates
 * (time added by the GM, end of combat) and the special attack becomes available again automatically.
 */
export class CharacterSpecialAttack {
    specialAttack: SpecialAttackResponse;
    successCount: number;
    cooldowns: SpecialAttackCooldown[];

    static fromResponse(response: CharacterSpecialAttackResponse): CharacterSpecialAttack {
        const characterSpecialAttack = new CharacterSpecialAttack();
        characterSpecialAttack.update(response);
        return characterSpecialAttack;
    }

    update(response: CharacterSpecialAttackResponse) {
        this.specialAttack = response.specialAttack;
        this.successCount = response.data.successCount ?? 0;
        this.cooldowns = response.data.cooldowns ?? [];
    }

    get canFail(): boolean {
        return !!this.specialAttack.data.failureCooldown;
    }

    get hasUsageLimit(): boolean {
        return !!this.specialAttack.data.cooldown;
    }

    /**
     * Same rule as the backend: no running failure cooldown, and less running cooldowns than the uses allowed per period.
     */
    get isAvailable(): boolean {
        if (this.cooldowns.some(c => c.failure)) {
            return false;
        }
        return this.cooldowns.length < (this.specialAttack.data.usesPerCooldown ?? 1);
    }

    get remainingUses(): number {
        return Math.max(0, (this.specialAttack.data.usesPerCooldown ?? 1) - this.cooldowns.length);
    }

    /**
     * The cooldown that has to expire for the special attack to be available again
     */
    get blockingCooldown(): SpecialAttackCooldown | undefined {
        const failureCooldown = this.cooldowns.find(c => c.failure);
        if (failureCooldown) {
            return failureCooldown;
        }
        if (this.isAvailable) {
            return undefined;
        }
        return [...this.cooldowns].sort((a, b) => CharacterSpecialAttack.remaining(a) - CharacterSpecialAttack.remaining(b))[0];
    }

    /**
     * Decrement the cooldowns matching the duration update, and remove the expired ones.
     * @return true when something changed and must be saved
     */
    updateDuration(durationType: string, data: number | unknown): boolean {
        if (typeof data !== 'number') {
            return false;
        }

        let changed = false;
        for (const cooldown of this.cooldowns) {
            if (cooldown.durationType !== durationType) {
                continue;
            }
            switch (cooldown.durationType) {
                case 'time':
                    cooldown.currentTimeDuration = Math.max(0, (cooldown.currentTimeDuration ?? 0) - data);
                    changed = true;
                    break;
                case 'combat':
                    cooldown.currentCombatCount = Math.max(0, (cooldown.currentCombatCount ?? 0) - data);
                    changed = true;
                    break;
            }
        }

        if (changed) {
            this.cooldowns = this.cooldowns.filter(c => CharacterSpecialAttack.remaining(c) > 0);
        }
        return changed;
    }

    static remaining(cooldown: SpecialAttackCooldown): number {
        switch (cooldown.durationType) {
            case 'time':
                return cooldown.currentTimeDuration ?? 0;
            case 'combat':
                return cooldown.currentCombatCount ?? 0;
            case 'lap':
                return cooldown.currentLapCount ?? 0;
            default:
                return 1;
        }
    }
}
