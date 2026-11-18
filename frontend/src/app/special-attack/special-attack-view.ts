import {SpecialAttackResponse} from '../api/responses';

/**
 * Text from the special attack cards: a first line, optionally followed by detail lines starting with "- ".
 */
export interface FormattedText {
    headline?: string;
    details: string[];
    empty: boolean;
}

export interface SpecialAttackView {
    id: string;
    name: string;
    requiredLevel?: number;
    range?: string;
    xp?: number;
    learning?: string;
    usableBy?: FormattedText;
    test?: FormattedText;
    usage?: FormattedText;
    sections: { label: string, text: FormattedText }[];
    criticalFailure?: FormattedText;
    criticalSuccess?: FormattedText;
    criticalNotes?: FormattedText;
}

const EMPTY_VALUES = ['aucun', 'aucune', 'aucun effet', 'non'];

export function normalizeForSearch(text: string): string {
    return text.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
}

/**
 * Text used to search a special attack (name, who can use it, effects, range), without accents and in lower case
 */
export function specialAttackSearchText(specialAttack: SpecialAttackResponse): string {
    const data = specialAttack.data;
    return normalizeForSearch([specialAttack.name, data.usableBy, data.effects, data.range].filter(x => x).join(' '));
}

export function formatText(text?: string): FormattedText | undefined {
    if (!text) {
        return undefined;
    }
    const headlines: string[] = [];
    const details: string[] = [];
    for (const line of text.split('\n').map(l => l.trim()).filter(l => l)) {
        if (line.startsWith('- ')) {
            details.push(line.substring(2));
        } else {
            headlines.push(line);
        }
    }
    const headline = headlines.length ? headlines.join(' ') : undefined;
    return {
        headline,
        details,
        empty: !details.length && !!headline && EMPTY_VALUES.includes(headline.toLowerCase()),
    };
}

export function toSpecialAttackView(specialAttack: SpecialAttackResponse): SpecialAttackView {
    const data = specialAttack.data;
    const sections: { label: string, text: FormattedText }[] = [];
    for (const [label, value] of [['Effets', data.effects], ['Dégâts', data.damage], ['Conditions', data.conditions]]) {
        const text = formatText(value);
        if (text && !text.empty) {
            sections.push({label: label!, text});
        }
    }
    return {
        id: specialAttack.id,
        name: specialAttack.name,
        requiredLevel: data.requiredLevel,
        range: data.range,
        xp: data.xp,
        learning: data.learning,
        usableBy: formatText(data.usableBy),
        test: formatText(data.test),
        usage: formatText(data.usage),
        sections,
        criticalFailure: formatText(data.criticalFailure),
        criticalSuccess: formatText(data.criticalSuccess),
        criticalNotes: formatText(data.criticalNotes),
    };
}
