import {SpellBookKind, SpellKind, SpellResponse, SpellText} from '../api/responses';

export const SPELL_KIND_LABELS: Record<SpellKind, string> = {
    spell: 'Sort',
    prodige: 'Prodige',
    ritual: 'Rituel',
    invocation: 'Invocation',
    discipline: 'Discipline',
};

export const SPELL_BOOK_KIND_LABELS: Record<SpellBookKind, string> = {
    mage: 'Mages',
    rare: 'Livres rares',
    priest: 'Prêtres',
    paladin: 'Paladins',
};

/** Order and labels of the stats of an invocation stat block */
export const CREATURE_STATS: { key: string, label: string }[] = [
    {key: 'ev', label: 'EV'},
    {key: 'at', label: 'AT'},
    {key: 'prd', label: 'PRD'},
    {key: 'esq', label: 'ESQ'},
    {key: 'cou', label: 'COU'},
    {key: 'int', label: 'INT'},
    {key: 'pr', label: 'PR'},
    {key: 'resm', label: 'RM'},
    {key: 'xp', label: 'XP'},
];

/** A line of text from the book, with its "Niveau N :" prefix split so it can be highlighted */
export interface SpellLine {
    levelPrefix?: string;
    text: string;
}

export interface SpellQuickInfo {
    label: string;
    value: string;
    details: SpellLine[];
}

export interface SpellListSection {
    label: string;
    items: SpellLine[];
}

const LEVEL_PREFIX_RE = /^((?:Niveaux?|Niv\.)\s+\d+\+?(?:\s*(?:à|-|et|ou)\s*\d+|\s+et\s+(?:plus|supérieurs?))?\s*\+?\s*:)\s*(.*)$/;

export function toSpellLine(text: string): SpellLine {
    const match = LEVEL_PREFIX_RE.exec(text);
    if (!match) {
        return {text};
    }
    return {levelPrefix: match[1], text: match[2]};
}

export function normalizeForSearch(text: string): string {
    return text.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
}

/**
 * Text used to search a spell (name, category, tags, description, usages), without accents and in lower case
 */
export function spellSearchText(spell: SpellResponse): string {
    const data = spell.data;
    return normalizeForSearch([
        spell.name,
        spell.category,
        spell.levelLabel,
        SPELL_KIND_LABELS[spell.kind],
        spell.description,
        ...(data.tags ?? []),
        ...(data.usages ?? []),
    ].filter(x => x).join(' ').replace(/[*\\]/g, ''));
}

function castingTimeLabel(spell: SpellResponse): string {
    switch (spell.data.castingTime?.type) {
        case 'prayer':
            return 'Prière';
        case 'ritual':
            return 'Durée du rituel';
        default:
            return 'Incantation';
    }
}

function quickInfo(label: string, value?: SpellText): SpellQuickInfo | undefined {
    if (!value || (!value.text && !value.notes?.length)) {
        return undefined;
    }
    const details = [...(value.notes ?? [])];
    const headline = value.text || details.shift() || '';
    return {label, value: headline, details: details.map(toSpellLine)};
}

export function spellQuickInfos(spell: SpellResponse): SpellQuickInfo[] {
    const data = spell.data;
    return [
        quickInfo('Coût', data.cost),
        quickInfo('Épreuve', data.test),
        quickInfo(castingTimeLabel(spell), data.castingTime),
        quickInfo('Durée', data.duration),
        quickInfo('Portée', data.range),
        quickInfo('Aire d’effet', data.area),
        quickInfo('Dégâts', data.damage),
    ].filter((x): x is SpellQuickInfo => !!x);
}

export function spellListSections(spell: SpellResponse, bookKind: SpellBookKind): SpellListSection[] {
    const data = spell.data;
    const sections: { label: string, items: string[] }[] = [
        {label: 'Usages', items: data.usages ?? []},
        {label: 'Bénéfices', items: data.benefits ?? []},
        {label: 'Effets', items: data.effects ?? []},
        {label: 'Restrictions', items: data.restrictions ?? []},
        {label: 'Ingrédients', items: (data.ingredients ?? []).map(i => i.text)},
        {label: 'Caractéristiques', items: data.characteristics ?? []},
        {label: bookKind === 'mage' || bookKind === 'rare' ? 'Mot de pouvoir' : 'Prière', items: data.powerWord ?? []},
    ];
    return sections
        .filter(s => s.items.length)
        .map(s => ({label: s.label, items: s.items.map(toSpellLine)}));
}

export function invocationQuickInfos(spell: SpellResponse): SpellQuickInfo[] {
    const invocation = spell.data.invocation;
    if (!invocation) {
        return [];
    }
    const testInfo = (label: string, test?: { text: string, notes?: string[], required: boolean }) => {
        if (!test) {
            return undefined;
        }
        if (!test.required && !test.notes?.length) {
            return {label, value: test.text || 'Non', details: [] as SpellLine[]};
        }
        return quickInfo(label, test);
    };
    return [
        quickInfo('Durée d’invocation', invocation.callTime),
        quickInfo('Épreuve d’appel', invocation.callTest),
        testInfo('Épreuve de contrôle', invocation.controlTest),
        testInfo('Épreuve de révocation', invocation.revokeTest),
        quickInfo('Maximum d’invocation', invocation.maxCountDetails),
    ].filter((x): x is SpellQuickInfo => !!x);
}

export function creatureStats(spell: SpellResponse): { label: string, value: string }[] {
    const stats = spell.data.invocation?.creatureStats;
    if (!stats) {
        return [];
    }
    return CREATURE_STATS
        .filter(s => stats[s.key])
        .map(s => ({label: s.label, value: stats[s.key]}));
}
