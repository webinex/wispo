export const NotificationField = {
  ID: 'id',
  RECIPIENT_ID: 'recipientId',
  SUBJECT: 'subject',
  BODY: 'body',
  IS_READ: 'isRead',
  CREATED_AT: 'createdAt',
  READ_BY_ID: 'readById',
  READ_AT: 'readAt',
} as const;

export type NotificationFieldValue = (typeof NotificationField)[keyof typeof NotificationField];
