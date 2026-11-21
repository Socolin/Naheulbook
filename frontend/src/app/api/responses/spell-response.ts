/** Original wording of a block, with its detail lines. */
export type SpellText = {
    text: string;
    notes?: string[];
}

export type SpellTimeUnit = 'assault' | 'second' | 'minute' | 'hour' | 'day' | 'week' | 'combat';

export type SpellTime = SpellText & {
    kind?: 'fixed' | 'immediate' | 'permanent' | 'variable' | 'other';
    value?: number;
    dice?: string;
    unit?: SpellTimeUnit;
}

export type SpellCastingTime = SpellTime & {
    type?: 'incantation' | 'prayer' | 'ritual';
}

export type SpellCost = SpellText & {
    kind: 'fixed' | 'variable' | 'per_unit' | 'all_astral_points' | 'other';
    astralPoints?: number;
    minAstralPoints?: number;
    maxAstralPoints?: number;
    per?: string;
}

export type SpellTest = SpellText & {
    kind: 'simple' | 'confrontation' | 'none' | 'other';
    stat?: string;
    modifier?: number;
    /** The test is only needed until this level */
    requiredUntilLevel?: number;
    confrontation?: { caster?: string; target?: string };
    magicResistance?: string;
}

export type SpellRange = SpellText & {
    kind: 'contact' | 'distance' | 'zone' | 'cone' | 'variable' | 'sight' | 'unlimited' | 'self_area' | 'other';
    meters?: number;
}

export type SpellDamage = SpellText & {
    dice?: string;
    perTarget?: boolean;
    /** The INT damage bonus does not apply */
    fixed?: boolean;
    /** Wounds that ignore the armor */
    ignoresArmor?: boolean;
    byLevel?: { minLevel: number; maxLevel?: number; value: string }[];
}

export type SpellIngredient = {
    name: string;
    quantity?: number;
    unit?: string;
    text: string;
    itemTemplateId?: string;
}

export type SpellInvocationTest = SpellText & {
    required: boolean;
    caster?: string;
    target?: string;
}

export type SpellInvocation = {
    callTime?: SpellTime;
    callTest?: SpellTest;
    controlTest?: SpellInvocationTest;
    revokeTest?: SpellInvocationTest;
    maxCount?: number;
    maxCountDetails?: SpellText;
    /** Raw stat block of the creature (ev, int, at, prd, esq, cou, pr, xp, resm) */
    creatureStats?: Record<string, string>;
    monsterTemplateId?: number;
}

export type SpellData = {
    tags?: string[];
    usages?: string[];
    castingTime?: SpellCastingTime;
    duration?: SpellTime;
    cost?: SpellCost;
    test?: SpellTest;
    range?: SpellRange;
    area?: SpellText;
    damage?: SpellDamage;
    benefits?: string[];
    effects?: string[];
    restrictions?: string[];
    ingredients?: SpellIngredient[];
    ritualDetails?: string;
    /** What the spell allows at each level ("Niveau 2 : Rongeurs…"), as markdown */
    levelDetails?: string;
    /** "Mot de pouvoir" for mages, "Prière" for priests and paladins */
    powerWord?: string[];
    characteristics?: string[];
    invocation?: SpellInvocation;
    criticalSuccess?: string;
    criticalFailure?: string;
    /** Sentence about criticals found in the notes (it is also kept in the notes) */
    criticalNotes?: string;
    notes?: string;
    source?: { pdf: string; pages: number[] };
}

export type SpellKind = 'spell' | 'prodige' | 'ritual' | 'invocation' | 'discipline';

export type SpellResponse = {
    id: string;
    name: string;
    level?: number;
    levelLabel?: string;
    kind: SpellKind;
    category?: string;
    description: string;
    data: SpellData;
}

export type SpellBookKind = 'mage' | 'rare' | 'priest' | 'paladin';

export type SpellBookData = {
    description?: string;
    categories: string[];
    sourcePdf?: string;
}

export type SpellBookResponse = {
    id: string;
    techName: string;
    name: string;
    kind: SpellBookKind;
    jobId?: string;
    specialityId?: string;
    godId?: number;
    data: SpellBookData;
    spells: SpellResponse[];
}

export type SpellBooksResponse = {
    spellBooks: SpellBookResponse[];
}
