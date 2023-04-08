import { FilterRule, NotificationListFieldValue, SortRule } from './query';

/**
 * Arguments for fetching notifications
 */
export interface NotificationListRequest {
  /**
   * Specifies what properties might be returned by server
   */
  include?: Array<NotificationListFieldValue | string>;

  /**
   * Filter criteria
   */
  filter?: FilterRule;

  /**
   * Sorting criteria
   */
  sort?: SortRule[];

  /**
   * When specified, first `skip` notifications would be skipped
   */
  skip?: number;

  /**
   * When specified, result would contain next `take` after `skip` notifications
   */
  take?: number;
}
