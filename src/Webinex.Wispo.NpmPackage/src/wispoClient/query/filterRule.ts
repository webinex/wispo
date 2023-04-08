import { NotificationFieldValue } from './notificationField';

export const FilterOpertor = {
  EQ: '=' as '=',
  NOT_EQ: '!' as '!=',
  GTE: '>=' as '>=',
  GT: '>' as '>',
  LTE: '<=' as '<=',
  LT: '<' as '<',
  CONTAINS: 'contains' as 'contains',
  NOT_CONTAINS: '!contains' as '!contains',
  AND: 'and' as 'and',
  OR: 'or' as 'or',
  IN: 'in' as 'in',
  NOT_IN: '!in' as '!in',
};

export type FilterRule = ValueFilterRule | InFilterRule | LogicalFilterRule;

export interface ValueFilterRule {
  operator:
    | (typeof FilterOpertor)['EQ']
    | (typeof FilterOpertor)['NOT_EQ']
    | (typeof FilterOpertor)['GT']
    | (typeof FilterOpertor)['GTE']
    | (typeof FilterOpertor)['LT']
    | (typeof FilterOpertor)['LTE']
    | (typeof FilterOpertor)['CONTAINS']
    | (typeof FilterOpertor)['NOT_CONTAINS'];
  value: any;
  fieldId: NotificationFieldValue | string;
}

export interface InFilterRule {
  operator: (typeof FilterOpertor)['IN'] | (typeof FilterOpertor)['NOT_IN'];
  fieldId: NotificationFieldValue | string;
  values: any[];
}

export interface LogicalFilterRule {
  operator: (typeof FilterOpertor)['OR'] | (typeof FilterOpertor)['AND'];
  children: FilterRule[];
}
