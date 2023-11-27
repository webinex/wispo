import { FilterRule, PagingRule, SortRule } from '@webinex/asky';
import { NotificationBase, QueryProp, WispoClient } from '../../core';

/**
 * Notifications list configuration
 */
export interface UseWispoListParams {
  /**
   * useWispoList might include total unread notifications for current user.
   * `total` would be 0 if `total !== true`
   */
  props?: Array<QueryProp | string>;

  /**
   * Filtering criteria
   * Default: undefined
   */
  filterRule?: FilterRule;

  /**
   * Sorting criteria
   * Default: undefined
   */
  sortRule?: SortRule[];

  /**
   * Paging criteria
   */
  pagingRule?: PagingRule;
}

export interface UseWispoListArgs<TNotification extends NotificationBase> extends UseWispoListParams {
  client: WispoClient<TNotification>;
}
