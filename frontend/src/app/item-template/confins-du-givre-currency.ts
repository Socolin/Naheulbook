import {ZgarugCurrency} from '../api/shared/enums';

export type ZgarugCurrencyDefinition = {
    code: ZgarugCurrency;
    shortName: string;
    name: string;
    /** Price when bought at the "poste avancé" */
    buyPriceInGold: number;
    /** Price when sold back at the "poste avancé" */
    sellPriceInGold: number;
};

/** Zgarug teeth, the currency used in the Confins du Givre (see "Commerce et prix des denrées") */
export const zgarugCurrencies: ZgarugCurrencyDefinition[] = [
    {code: 'pz', shortName: 'PZ', name: 'Petite dent zgarug', buyPriceInGold: 5, sellPriceInGold: 4},
    {code: 'mz', shortName: 'MZ', name: 'Dent moyenne zgarug', buyPriceInGold: 50, sellPriceInGold: 40},
    {code: 'gz', shortName: 'GZ', name: 'Grosse dent zgarug', buyPriceInGold: 200, sellPriceInGold: 150},
];

export const zgarugCurrencyByCode: Record<ZgarugCurrency, ZgarugCurrencyDefinition> = Object.fromEntries(
    zgarugCurrencies.map(c => [c.code, c])
) as Record<ZgarugCurrency, ZgarugCurrencyDefinition>;
