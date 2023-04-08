import { NotificationFieldValue } from './notificationField';

export type SortDir = 'asc' | 'desc';

export interface SortRule {
  fieldId: NotificationFieldValue;
  dir: SortDir;
}
