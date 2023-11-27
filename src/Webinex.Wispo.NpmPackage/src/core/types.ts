import { FilterRule, PagingRule, SortRule } from '@webinex/asky';

export const SIGNALR_MESSAGE = {
  /**
   * Sent when new notification created.
   * args[0] - notifications
   */
  New: 'wispo://NEW',

  /**
   * Sent when notifications marked as read.
   * args[0] - ids
   */
  Read: 'wispo://READ',
} as const;

export interface NotificationBase {
  id: string;
  type: string;
  recipientId: string;
  read: boolean;
  createdAt: string;
  readById?: string;
  readAt?: string;
}

export type QueryProp = 'totalUnreadCount' | 'totalCount' | 'totalMatchCount' | 'items';

export interface Query {
  props?: Array<QueryProp | string>;
  filterRule?: FilterRule;
  sortRule?: SortRule[];
  pagingRule?: PagingRule;
}

export interface QueryResult<TNotification extends NotificationBase> {
  /**
   * Notifications which match get criteria
   * This value will be null if args.props doesn't contain 'items'
   */
  items: TNotification[] | null;

  /**
   * Total notifications count for current user.
   * This value will be null if `args.props doesn't contain 'totalCount'
   */
  totalCount: number | null;

  /**
   * Unread notifications count for current user.
   * This value will be null if args.props doesn't contain 'totalUnreadCount'
   */
  totalUnreadCount: number | null;

  /**
   * Total notifications matching filter criteria.
   * This value will be null if args.props doesn't contain 'totalMatchCount'
   */
  totalMatchCount: number | null;
}
