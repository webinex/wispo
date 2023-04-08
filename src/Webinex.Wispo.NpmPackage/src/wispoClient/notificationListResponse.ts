import { Notification } from './notification';

export interface NotificationListResponse {
  /**
   * Notifications which match get criteria
   * This value will be null if args.include doesn't contain Include.Items
   */
  items: Notification[];

  /**
   * Total notifications count for current user.
   * This value will be 0 if `args.includeTotal !== true`
   */
  total: number;

  /**
   * Unread notifications count for current user.
   * This value will be null if args.include doesn't contain Include.TotalUnread
   */
  totalUnread: number;

  /**
   * Total notifications matching filter criteria.
   * This value will be null if args.include doesn't contain Include.TotalMatch
   */
  totalMatch: number;
}
