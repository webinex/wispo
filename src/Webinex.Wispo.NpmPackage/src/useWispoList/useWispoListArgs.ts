import { WispoClient, FilterRule, SortRule } from '../wispoClient';

/**
 * Notifications list configuration
 */
export interface UseWispoListParams {
  /**
   * useWispoList might include total unread notifications for current user.
   * `total` would be 0 if `total !== true`
   */
  include?: string[];

  /**
   * Filtering criteria
   * Default: undefined
   */
  filter?: FilterRule;

  /**
   * Sorting criteria
   * Default: undefined
   */
  sort?: SortRule[];

  /**
   * useWispoList `items` might skip first `skip` items
   */
  skip?: number;

  /**
   * useWispoList `items` might include not more than `take` items
   */
  take?: number;
}

export interface UseWispoListArgs extends UseWispoListParams {
  client: WispoClient;
}
