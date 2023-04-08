export const NotificationListField = {
  TotalUnread: 'TotalUnread',
  Total: 'Total',
  TotalMatch: 'TotalMatch',
  Items: 'Items',
} as const;

export type NotificationListFieldValue = (typeof NotificationListField)[keyof typeof NotificationListField];
